using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Globalization;

namespace Lunohod
{
	public class LevelEngine : ScreenEngine
	{
        private XHero hero;
        private XTower tower;

		private XClass explosionClass;
		
		private Dictionary<GameEvent, RadioWave> waves;

        private List<Tuple<XElement, System.Drawing.RectangleF, double>> colliders = new List<Tuple<XElement, System.Drawing.RectangleF, double>>();
		
		private int bombCounter;
		
		private EventRateCounter cps; 

        
        public LevelEngine(GameEngine gameEngine, ScreenEngine owner, XLevelInfo levelInfo, XLevelScore levelScore)
			: base(gameEngine, levelInfo.File, owner)
		{
            this.LevelInfo = levelInfo;
			this.LevelScore = levelScore;
			this.cps = new EventRateCounter(TimeSpan.FromSeconds(game.GameObject.CpsTimeSpan));
		}

        public XHero Hero
        {
            get { return hero; }
            set { hero = value; }
        }
        public XTower Tower
        {
            get { return tower; }
            set { tower = value; }
        }

		public XLevel LevelObject { get { return this.RootComponent as XLevel; } }
		
        public override Type RootComponentType { get { return typeof(XLevel); } }

        public XLevelInfo LevelInfo { get; private set; }
		public XLevelScore LevelScore { get; private set; }
		
		public double CommandsPerSecond { get { return cps.EventsPerSecond; } }
		
		public override void Initialize()
		{
			base.Initialize();
			
			// Calculate points available on this level
			CountLevelPoints();

			var li = (XLevelInfo)LevelInfo.Copy();
			var ls = (XLevelScore)LevelScore.Copy();
			
			li.Id = "levelInfo";
			ls.Id = "levelScore";
			
			this.RootComponent.Subcomponents.Insert(0, li);
			this.RootComponent.Subcomponents.Insert(0, ls);
			this.RootComponent.ResetComponentDictionary();
			
			// Make sure explosion class exists
			if (!string.IsNullOrEmpty(this.LevelInfo.ExplosionClass))
			{
				this.explosionClass = this.LevelObject.FindLocal(this.LevelInfo.ExplosionClass) as XClass;
				Debug.WriteLine("*** Explosion class could not be found: '{0}'", this.LevelInfo.ExplosionClass);
			}
			
			// Init waves
			this.waves = new Dictionary<GameEvent, RadioWave>();
		}

		private void CountLevelPoints()
		{
			this.LevelScore.AvaliablePoints = 0;
			this.LevelObject.TraveseTree(o => {
				if (o is IHasPoints && !(this.LevelInfo.BombCount == 0 && o is XEnemy))
					this.LevelScore.AvaliablePoints += ((IHasPoints)o).Points;
			});
		}
		
		public void SaveNewScore(XLevelScore newScore)
		{
			if (newScore.TotalScore < this.LevelScore.TotalScore)
				return;
			
			newScore.CopyTo(this.LevelScore);
			
			game.SaveScore();
		}

		private void UpdateCps(GameTime gameTime)
        {
            cps.Update(gameTime);

            bool eventIsSet = this.CurrentEvents.ContainsKey("system:cpsLimitExceeded");

            if (cps.EventsPerSecond > game.GameObject.CpsLimit)
            {
                if (!eventIsSet)
                    game.EnqueueEvent(new GameEvent("system:+cpsLimitExceeded", gameTime, true));
            }
            else
            {
                if (eventIsSet)
                    game.EnqueueEvent(new GameEvent("system:-cpsLimitExceeded", gameTime, true));
            }
        }

		public override void Update(GameTime gameTime)
		{
            UpdateCps(gameTime);

			base.Update(gameTime);
			
			foreach(var wave in waves.Values)
				wave.Update(updateParameters);
			
			ProcessCollisions();
		}
		
		public override void Draw(GameTime gameTime)
		{
			try 
			{
				PreDraw(gameTime);
				
				this.RootComponent.Draw(drawParameters);
				
				foreach(var wave in waves.Values)
					wave.Draw(drawParameters);

				PostDraw();				
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		public override void Unload()
		{
			base.Unload();

			this.hero = null;
			this.tower = null;
			this.waves = null;
		}

		#region Collisions

		public void ProcessCollisions()
		{
			// if not moving - we don't process collisions
            //if (this.hero.Direction == Direction.VectorStop)
            //    return;
			
			// find objects we collided with
			FindCollisions();
			
			if (colliders.Count() == 0)
				return;
			
			// sort them by the amounth of intersection (descending)
			colliders.Sort((t1, t2) => t2.Item3.CompareTo(t1.Item3));
			
			for(int i = 0; i < colliders.Count; i++)
			{
				// process collision with the object
				if (colliders[i].Item1.ProcessCollision(this, colliders[i].Item2))
					// quit if collision was processed or go to the next object
					break;
			}
		}

		void FindCollisions()
		{
			colliders.Clear();
			
			XElement obstacle;
			System.Drawing.RectangleF heroBounds = this.hero.Bounds;
			System.Drawing.RectangleF obstacleBounds;
			System.Drawing.RectangleF intersect;
			
			for(int i = 0; i < this.obstacles.Count; i++)
			{
				obstacle = this.obstacles[i];

                if (!obstacle.Enabled)
                    continue;

				obstacleBounds = obstacle.GetScreenBounds();
				
				Utility.Intersect(ref heroBounds, ref obstacleBounds, out intersect);
				
				if (intersect.Area() != 0)
				{
					colliders.Add(Tuple.Create(obstacle, intersect, intersect.Area()));
				}
			}
		}

#endregion

		public override bool EventAllowed(GameEvent e)
		{
			if (e.EventType == GameEventType.Explosion)
			{
				return this.Hero.BombCount == -1 || this.Hero.BombCount > 0;
			}
			
			return true;
		}
		
		public override void ProcessEvent(GameTime gameTime, GameEvent e)
		{
			bool signalReachedHero = true;
			
			if (!e.IsInstant)
			{
				bool firstProcess = !waves.ContainsKey(e);
				
				if (firstProcess)
				{
					cps.RecordEvent();

                    if (cps.EventsPerSecond > game.GameObject.CpsLimit)
                    {
                        e.IsHandled = true;
                        return;
                    }
				}
				
                double signalTraveledDistance = this.tower.SignalSpeed * (gameTime.TotalGameTime - e.Time).TotalSeconds;
				signalReachedHero = signalTraveledDistance >= this.hero.DistanceToTower;

                if (signalReachedHero)
                    waves.Remove(e);
                else
                {
                    if (firstProcess)
                        waves.Add(e, new RadioWave(e.Time));
                    return;
                }
			}
			
			e.IsHandled = true;
				
			switch (e.EventType)
			{
                case GameEventType.Up: this.hero.Direction = Direction.VectorUp; this.hero.Speed = this.hero.DefaultSpeed; break;
                case GameEventType.Down: this.hero.Direction = Direction.VectorDown; this.hero.Speed = this.hero.DefaultSpeed; break;
                case GameEventType.Left: this.hero.Direction = Direction.VectorLeft; this.hero.Speed = this.hero.DefaultSpeed; break;
                case GameEventType.Right: this.hero.Direction = Direction.VectorRight; this.hero.Speed = this.hero.DefaultSpeed; break;
	            case GameEventType.Explosion: PlantBomb(); break;
			}

            base.ProcessEvent(gameTime, e);
		}

		private void PlantBomb()
		{
			if (this.explosionClass == null)
			{
				Debug.WriteLine("No explosion class defined.");
				return;
			}
			
			var heroParent = this.hero.Parent;
			
			// create a placeholder object for our bomb
			var explosionPlaceholder = new XExplosion {
				Id = "internal_bomb_" + bombCounter.ToString(CultureInfo.InvariantCulture),
				Class = this.explosionClass.Id,
				Bounds = ((XExplosion)this.explosionClass.TemplateObject).Bounds
			};
			// insert it right before hero
			heroParent.Subcomponents.InsertBefore(this.hero, explosionPlaceholder);
			// center it to hero's coordinates
			explosionPlaceholder.Center = this.hero.Center;
			// create instance in place of the placeholder
			var instance = explosionPlaceholder.InitiazeFromClass();
			// make sure that the new instance and all the new subtree is added to the dictionaries
			instance.TraveseAncestors(a => {
				a.AddSubtreeToComponentDict(instance);
			});
			// initialize the new instance and its subcomponents
			instance.Initialize(new InitializeParameters() { Game = this.game, ScreenEngine = this });
			
			if (this.Hero.BombCount > 0)
				this.Hero.BombCount--;
			
			bombCounter++;
		}
	}
}	
