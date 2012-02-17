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
		
		public string GetLevelId(string si)
		{
			int i = int.Parse(si);
			
			if (i >= game.GameObject.Levels.Length)
				return "";
			
			var level = game.GameObject.Levels[i];
			
			return level.Id;
		}
		
		public void StartLevel(string si)
		{
			int i = int.Parse(si);
			
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
			game.EnqueueEvent(new GameEvent(GameEventType.CloseCurrentScreen, GameEngine.Instance.CurrentUpdateTime) { IsInstant = true });
		}
		
		public void EndCurrentLevel()
		{
			this.CloseCurrentScreen();
		}
		
		public float RndX(string startStr, string endStr)
		{
			return this.Rnd(startStr, endStr) * this.game.Scale.X;
		}
		public float RndY(string startStr, string endStr)
		{
			return this.Rnd(startStr, endStr) * this.game.Scale.Y;
		}
		public float Rnd(string startStr, string endStr)
		{
			float start = float.Parse(startStr, CultureInfo.InvariantCulture);
			float end = float.Parse(endStr, CultureInfo.InvariantCulture);
			
			return start + (float)random.NextDouble() * (start - end);
		}
	}
}

