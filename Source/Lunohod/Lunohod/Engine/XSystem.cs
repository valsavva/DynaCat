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
		
		public string GetLevelName(int levelIndex)
		{
			if (levelIndex >= game.GameObject.Levels.Count)
				return "";
			
			return game.GameObject.Levels[levelIndex].Name;
		}
		
		public void StartLevel(int levelIndex)
		{
			if (levelIndex >= game.GameObject.Levels.Count)
				return;

			game.LoadLevel(game.GameObject.Levels[levelIndex].File);
		}
		
		public string GetSeriesLevelName(int seriesIndex, int levelIndex)
		{
			if (seriesIndex >= game.GameObject.LevelSeries.Count)
				return "";
			
			var series = game.GameObject.LevelSeries[seriesIndex];
			
			if (levelIndex >= series.Levels.Count)
				return "";
			
			return series.Levels[levelIndex].Name;
		}
		
		public void StartSeriesLevel(int seriesIndex, int levelIndex)
		{
			if (seriesIndex >= game.GameObject.LevelSeries.Count)
				return;
			
			var series = game.GameObject.LevelSeries[seriesIndex];
			
			if (levelIndex >= series.Levels.Count)
				return;
			
			game.LoadLevel(series.Levels[levelIndex].File);
		}
		
		public void StartScreen(string fileName)
		{
			game.LoadScreen(fileName);
		}
		
		public void CloseCurrentScreen()
		{
			this.EnqueueEvent(GameEventType.CloseCurrentScreen);
		}
		
		public void EndCurrentLevel()
		{
            this.EnqueueEvent(GameEventType.EndCurrentLevel);
		}

		public string GetSeriesName(int i)
		{
			if (i >= game.GameObject.LevelSeries.Count)
				return "";
			
			return game.GameObject.LevelSeries[i].Name;
		}
		
		public void StartSeries(int i)
		{
			if (i >= game.GameObject.LevelSeries.Count)
				return;
			
			game.LoadScreen(game.GameObject.LevelSeries[i].File);
		}
		
		public double RndX(double start, double end)
		{
			return this.Rnd(start, end) * this.game.Scale.X;
		}
		public double RndY(double start, double end)
		{
			return this.Rnd(start, end) * this.game.Scale.Y;
		}
		public double Rnd(double start, double end)
		{
			return start + random.NextDouble() * (end - start);
		}
        public string Str(object o)
        {
            return o.ToString();
        }
	}
}

