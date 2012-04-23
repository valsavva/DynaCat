using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Lunohod.Xge;

namespace Lunohod.Objects
{
	public class XSystem : XObject
	{
		private GameEngine game;
		private static Random random = new Random();
		
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

			game.LoadLevel(game.GameObject.Levels[levelIndex]);
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
			
			game.LoadLevel(series.Levels[levelIndex]);
		}
		
        public void StartNextLevel()
        {
            this.EnqueueEvent(GameEventType.StartNextLevel);
        }

		public void StartScreen(string fileName)
		{
			game.LoadScreen(fileName);
		}
		
		public void CloseCurrentScreen()
		{
			this.EnqueueEvent(GameEventType.CloseCurrentScreen);
		}

        public void RestartLevel()
        {
            this.EnqueueEvent(GameEventType.RestartLevel);
        }

        public void AbandonLevel()
        {
            this.EnqueueEvent(GameEventType.AbandonLevel);
        }

        public void EndLevel()
        {
            this.EnqueueEvent(GameEventType.EndLevel);
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
		public double Round(double num, int digits)
		{
			return Math.Round(num, digits);
		}
		public double IIf(bool condition, double trueValue, double falseValue)
		{
			return condition ? trueValue : falseValue;
		}
		public string IIfStr(bool condition, string trueValue, string falseValue)
		{
			return condition ? trueValue : falseValue;
		}
		
		public override void GetMethod(string methodName, out Func<List<Lunohod.Xge.Expression>, double> method)
		{
            switch (methodName)
            {
                case "Rnd": method = (ps) => Rnd(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "RndX": method = (ps) => RndX(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "RndY": method = (ps) => RndY(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "Round": method = (ps) => Round(ps[0].GetNumValue(), ps.Count < 2 ? 0 : ps[1].GetIntValue()); break;
				case "IIf": method = (ps) => IIf(ps[0].GetBoolValue(), ps[1].GetNumValue(), ps[2].GetNumValue()); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
		
		public override void GetMethod(string methodName, out Func<List<Expression>, string> method)
		{
            switch (methodName)
            {
                case "Str": method = (ps) => Str(ps[0].GetObjValue()); break;
                case "GetLevelName": method = (ps) => GetLevelName(ps[0].GetIntValue()); break;
                case "GetSeriesLevelName": method = (ps) => GetSeriesLevelName(ps[0].GetIntValue(), ps[1].GetIntValue()); break;
                case "GetSeriesName": method = (ps) => GetSeriesName(ps[0].GetIntValue()); break;
				case "IIfStr": method = (ps) => IIfStr(ps[0].GetBoolValue(), ps[1].GetStrValue(), ps[2].GetStrValue()); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
		
		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
                case "StartLevel": method = (ps) => StartLevel(ps[0].GetIntValue()); break;
                case "RestartLevel": method = (ps) => RestartLevel(); break;
				case "StartSeriesLevel": method = (ps) => StartSeriesLevel(ps[0].GetIntValue(), ps[1].GetIntValue()); break;
                case "StartScreen": method = (ps) => StartScreen(ps[0].GetStrValue()); break;
				case "StartSeries": method = (ps) => this.StartSeries(ps[0].GetIntValue()); break;
                case "CloseCurrentScreen": method = (ps) => CloseCurrentScreen(); break;
                case "AbandonLevel": method = (ps) => AbandonLevel(); break;
                case "EndLevel": method = (ps) => EndLevel(); break;
                case "StartNextLevel": method = (ps) => StartNextLevel(); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
	}
}

