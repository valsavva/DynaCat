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
		
		public string GetLevelName(string si)
		{
			int i = int.Parse(si);
			
			if (i >= game.GameObject.Levels.Length)
				return "";
			
			var level = game.GameObject.Levels[i];
			
			return level.Name;
		}
		
		public void StartLevel(string si)
		{
			int i = int.Parse(si);
			
			if (i >= game.GameObject.Levels.Length)
				return;

			game.LoadLevel(game.GameObject.Levels[i].File);
		}
		
		public void CloseCurrentScreen()
		{
			game.EnqueueEvent(new GameEvent(GameEventType.CloseCurrentScreen, GameEngine.Instance.CurrentUpdateTime) { IsInstant = true });
		}
		
		public void EndCurrentLevel()
		{
			this.CloseCurrentScreen();
		}
		
		public float Rnd(string startStr, string endStr)
		{
			if (this.game.Scale.X != this.game.Scale.Y)
				// Uneven scale, we need to change the engine to ensure that numbers scale properly on both axes.
				throw new InvalidCastException("Scewed scale. Cannot automatically transform the input number according to scale.");
				
				
			float start = float.Parse(startStr, CultureInfo.InvariantCulture) * this.game.Scale.X;
			float end = float.Parse(endStr, CultureInfo.InvariantCulture) * this.game.Scale.X;
			
			return start + (float)random.NextDouble() * (start - end);
		}
	}
}

