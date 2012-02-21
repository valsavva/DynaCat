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

		private XLevel levelObject;
		private XClass explosionClass;
		private int bombCounter = 0;
		
		private Dictionary<GameEvent, RadioWave> waves;

        private List<Tuple<XElement, System.Drawing.RectangleF, float>> colliders = new List<Tuple<XElement, System.Drawing.RectangleF, float>>();
        
        public LevelEngine(GameEngine gameEngine, string name)
			: base(gameEngine, name)
		{
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
        
        public override Type RootComponentType { get { return typeof(XLevel); } }
		
		public override void Initialize()
		{
			base.Initialize();
			
			this.waves = new Dictionary<GameEvent, RadioWave>();

			this.levelObject = (XLevel)this.RootComponent;
			
			if (!string.IsNullOrEmpty(this.levelObject.DefaultSettings.ExplosionClass))
			{
				this.explosionClass = this.levelObject.FindDescendant(this.levelObject.DefaultSettings.ExplosionClass) as XClass;
				if (this.explosionClass == null)
					throw new InvalidOperationException(string.Format("Explosion class could not be found: '{0}'", this.levelObject.DefaultSettings.ExplosionClass));
			}
		}
		
		public override void Update(GameTime gameTime)
		{
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
			if (this.levelObject.DefaultSettings.BombCount == -1)
				return true;
			
			return this.bombCounter < this.levelObject.DefaultSettings.BombCount;
		}
		
		public override void ProcessEvent(GameTime gameTime, GameEvent e)
		{
			bool signalReachedHero = true;
			
			if (!e.IsInstant)
			{
				double signalTraveledDistance = this.tower.SignalSpeed * (gameTime.TotalGameTime - e.Time).TotalSeconds;
				signalReachedHero = signalTraveledDistance >= this.hero.DistanceToTower;

				if (signalReachedHero)
					waves.Remove(e);
				else if (!waves.ContainsKey(e))
					waves.Add(e, new RadioWave(e.Time));
			}
			
			if (signalReachedHero)
			{
				e.IsHandled = true;
				
				switch (e.EventType)
				{
					case GameEventType.Up : this.hero.Direction = Direction.VectorUp; break;
					case GameEventType.Down : this.hero.Direction = Direction.VectorDown; break;
					case GameEventType.Left : this.hero.Direction = Direction.VectorLeft; break;
	                case GameEventType.Right: this.hero.Direction = Direction.VectorRight; break;
	                case GameEventType.Stop: this.hero.Direction = Direction.VectorStop; break;
	                case GameEventType.Explosion: PlantBomb(); break;
					default: e.IsHandled = false; break;
				}
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
			instance.Initialize(new InitializeParameters() { Game = this.game, ScreenEngine = this.game.ScreenEngine });
			
			this.bombCounter++;
		}
	}
}	
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Threading;

namespace Lunohod
{
    public class GameEngine
    {
        private Canvas container;
        private XLevel level;

        private XLayer actionLayer;
        private XTower tower;
        public XHero hero;
        public List<XElement> obstacles;

        public Queue<ActionInfo> actions = new Queue<ActionInfo>();

        private int frequency = 60;
        private Dispatcher dispatcher = Dispatcher.CurrentDispatcher;

        private static Dictionary<Key, XHeroMoveType> KeyMapping = new Dictionary<Key, XHeroMoveType> 
        {
            { Key.Left, XHeroMoveType.Left },
            { Key.Right, XHeroMoveType.Right },
            { Key.Up, XHeroMoveType.Up },
            { Key.Down, XHeroMoveType.Down },
        };

        public GameEngine(Canvas container, XLevel level)
        {
            this.container = container;
            this.level = level;
        }

        public void container_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point location = e.GetPosition(this.container);
            location.X -= hero.Window.Width / 2;
            location.Y -= hero.Window.Height / 2;
            hero.SetLocation(location);
        }

        public void container_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                hero.Move = XHeroMoveType.None;
            }
            else
            {
                XHeroMoveType move = XHeroMoveType.None;

                if (KeyMapping.TryGetValue(e.Key, out move))
                {
                    if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                        hero.Move = move;
                    else
                        EnqueueAction(move);
                }

            }
        }

        private void EnqueueAction(XHeroMoveType move)
        {
            var moveInfo = new ActionInfo { Move = move, StartTime = DateTime.Now };
            moveInfo.Circle.SetBounds(new RectangleF(this.tower.Bounds.ToRect().Center(), new Point(1, 1)));
            this.actionLayer.Window.Children.Add(moveInfo.Circle);
            actions.Enqueue(moveInfo);
        }

        private void DequeueAction(ActionInfo action)
        {
            actionLayer.Window.Children.Remove(action.Circle);
            actions.Dequeue();
        }

        public void StartLevel()
        {
            CreateLevelWindow();

            ThreadPool.QueueUserWorkItem(Loop);            
        }

        private void Loop(object o)
        {
            while (true)
            {
                dispatcher.Invoke((Action)LoopOnMainThread, null);

                Thread.Sleep((int)(1.0 / frequency * 1000));
            }
        }

        private void LoopOnMainThread()
        {
            try
            {
                AnalyzeActions();
                ExecuteMove();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: \n" + ex.ToString());
            }
        }

        private void AnalyzeActions()
        {
            if (actions.Count == 0)
                return;

            RectangleF towerRect = tower.Bounds.ToRect();
            RectangleF heroRect = hero.Bounds.ToRect();
            double heroToTowerDistance = towerRect.Location.DistanceTo(heroRect.Location);

            while (actions.Count > 0)
            {
                var action = actions.Peek();
                var actionRadius = tower.ActionRadius(action);
                action.SetRadius(tower, actionRadius);
                if (actionRadius > heroToTowerDistance)
                {
                    hero.Move = action.Move;
                    DequeueAction(action);
                }
                else
                    break;
            }
        }


        // removes actions that the hero escapes by using teleport
        public void DequeuePastActions()
        {
            RectangleF towerRect = tower.Bounds.ToRect();
            RectangleF heroRect = hero.Bounds.ToRect();
            double heroToTowerDistance = towerRect.Location.DistanceTo(heroRect.Location);

            while (actions.Count > 0)
            {
                var action = actions.Peek();
                var actionRadius = tower.ActionRadius(action);
                if (actionRadius > heroToTowerDistance)
                {
                    DequeueAction(action);
                }
                else
                    break;
            }
        }

        private void ExecuteMove()
        {
            if (hero.Move == XHeroMoveType.None)
                return;

            double d = hero.Speed / frequency;
            RectangleF newBounds = hero.Bounds.ToRect();
            Point newLocation = newBounds.Location;

            if (hero.Move == XHeroMoveType.Left)
                newLocation.X -= d;
            else if (hero.Move == XHeroMoveType.Right)
                newLocation.X += d;
            else if (hero.Move == XHeroMoveType.Up)
                newLocation.Y -= d;
            else if (hero.Move == XHeroMoveType.Down)
                newLocation.Y += d;

            newBounds.Location = newLocation;

            var obstacle = (
                from o in obstacles
                let bounds = o.Bounds.ToRect()
                let intersect = RectangleF.Intersect(bounds, newBounds)
                where !intersect.IsEmpty
                orderby intersect.Area() descending
                select o
            ).FirstOrDefault();

            if (obstacle != null)
                obstacle.ProcessCollision(this, newBounds);
            else
                hero.SetBounds(newBounds);
        }

        private void CreateLevelWindow()
        {
            level.Window = new Canvas() {
                Width = level.Width,
                Height = level.Height
            };

            container.Children.Add(level.Window);

            int zIndex = 0;
            foreach (var layer in level.Layers)
            {
                CreateLayerWindow(zIndex++, layer);
            }
        }

        private void CreateLayerWindow(int zIndex, XLayer layer)
        {
            layer.CreateWindow(level.Window);

            Canvas.SetZIndex(layer.Window, zIndex);

            if (layer.Name == "action")
            {
                actionLayer = layer;
                tower = (XTower)layer.Elements.First(e => e is XTower);
                hero = (XHero)layer.Elements.First(e => e is XHero);
                obstacles = layer.Elements.Where(e => e is XBlock).ToList();
            }
        }
    }
}
*/