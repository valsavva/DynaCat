using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace Lunohod.Objects
{
	public class XSystem : XObject
	{
		private GameEngine game;
		private static Random random = new Random();
		
		public XSystem()
		{
			this.Id = "system";
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.game = p.Game;
		}
		
		public string GetLevelId(int i)
		{
			if (i >= game.GameObject.Levels.Length)
				return "";
			
			var level = game.GameObject.Levels[i];
			
			return level.Id;
		}
		
		public void StartLevel(int i)
		{
			if (i >= game.GameObject.Levels.Length)
				return;

			game.LoadLevel(game.GameObject.Levels[i].Id);
		}
		
		public void StartScreen(string id)
		{
			game.LoadScreen(id);
		}
		
		public void CloseCurrentScreen()
		{
			this.EnqueueEvent(GameEventType.CloseCurrentScreen);
		}
		
		public void EndCurrentLevel()
		{
            this.EnqueueEvent(GameEventType.EndCurrentLevel);
		}

        public float RndX(float start, float end)
		{
			return this.Rnd(start, end) * this.game.Scale.X;
		}
		public float RndY(float start, float end)
		{
			return this.Rnd(start, end) * this.game.Scale.Y;
		}
		public float Rnd(float start, float end)
		{
			return start + (float)random.NextDouble() * (end - start);
		}
        public string Str(object o)
        {
            return o.ToString();
        }
	}
}

