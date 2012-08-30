using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Lunohod.Xge;
using System.Text;
using Microsoft.Xna.Framework;

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
		
		private XLevelInfo GetSeriesLevelInfo(int seriesIndex, int levelIndex)
		{
			if (seriesIndex >= game.GameObject.LevelSeries.Count)
				return null;
			
			var series = game.GameObject.LevelSeries[seriesIndex];
			
			if (levelIndex >= series.Levels.Count)
				return null;
			
			return series.Levels[levelIndex];
		}
		
		public string GetSeriesLevelName(int seriesIndex, int levelIndex)
		{
			var levelInfo = GetSeriesLevelInfo(seriesIndex, levelIndex);
			
			return levelInfo != null ? levelInfo.Name : "";
		}
		
		public double GetSeriesLevelCount(int seriesIndex)
		{
			return game.GameObject.LevelSeries[seriesIndex].Levels.Count;
		}

        public double GetSeriesUnlockedLevelNumber(int seriesIndex)
        {
            var levels = game.GameObject.LevelSeries[seriesIndex].Levels;
            int i = 0;
            for (; i < levels.Count - 1; i++)
            {
                if (game.ScoreFile.LevelScoreDict[levels[i].Id].Time == 0)
                    break;
            }
            return i;
        }
		
		public double GetSeriesLevelStars(int seriesIndex, int levelIndex)
		{
			var levelInfo = GetSeriesLevelInfo(seriesIndex, levelIndex);

			if (levelInfo == null)
				return 0;
			
			var score = game.ScoreFile.LevelScoreDict[levelInfo.Id];
			
			return score.NumberOfStars;
		}
		
		public double GetSeriesLevelScore(int seriesIndex, int levelIndex)
		{
			var levelInfo = GetSeriesLevelInfo(seriesIndex, levelIndex);

			if (levelInfo == null)
				return 0;
			
			var score = game.ScoreFile.LevelScoreDict[levelInfo.Id];

			return score.TotalScore;
		}

		public bool GetSeriesLevelHasBadge(int seriesIndex, int levelIndex)
		{
			var levelInfo = GetSeriesLevelInfo(seriesIndex, levelIndex);

			if (levelInfo == null)
				return false;
			
			var score = game.ScoreFile.LevelScoreDict[levelInfo.Id];

			return score.HasBadge;
		}
		
		public double GetSeriesScore(int seriesIndex)
		{
			double result = 0;

			game.ScoreFile.LevelScores.ForEach(s => {
				if (s.SeriesNumber == seriesIndex)
					result += s.TotalScore;
			});

			return result;
		}
		public double GetSeriesStars(int seriesIndex)
		{
			double result = 0;

			game.ScoreFile.LevelScores.ForEach(s => {
				if (s.SeriesNumber == seriesIndex)
					result += s.NumberOfStars;
			});

			return result;
		}
		public double GetSeriesAvailableStars(int seriesIndex)
		{
			return game.GameObject.LevelSeries[seriesIndex].Levels.Count * 3;
		}
		
		public void StartSeriesLevel(int seriesIndex, int levelIndex)
		{
			var levelInfo = GetSeriesLevelInfo(seriesIndex, levelIndex);

			if (levelInfo == null)
				return;
			
			game.LoadLevel(levelInfo);
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
		
		public double GetCommandsPerSecond()
		{
			return ((LevelEngine)this.GetScreen().ScreenEngine).CommandsPerSecond;
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
		public double Ceiling(double num)
		{
			return Math.Ceiling(num);
		}
		public double Truncate(double num)
		{
			return Math.Truncate(num);
		}
		public double Clamp (double value, double min, double max)
		{
			if (value < min)
				return min;
			else if (value > max)
				return max;
			
			return value;
		}
		public double Max(double value, double max)
		{
			return Math.Max(value, max);
		}
		public double Min(double value, double min)
		{
			return Math.Min(value, min);
		}
		public double Abs(double value)
		{
			return Math.Abs(value);
		}
		public double IIf(bool condition, double trueValue, double falseValue)
		{
			return condition ? trueValue : falseValue;
		}
		public string IIfStr(bool condition, string trueValue, string falseValue)
		{
			return condition ? trueValue : falseValue;
		}
		public void WriteLine(string message, params object[] pars)
		{
			System.Diagnostics.Debug.WriteLine(message, pars);
		}

		public bool IsMute
		{
			get { return GameEngine.Instance.IsMute; }
			set { GameEngine.Instance.IsMute = value; }
		}

		public void OpenUrl(string url)
		{
#if IPHONE
            MonoTouch.UIKit.UIApplication.SharedApplication.OpenUrl(new MonoTouch.Foundation.NSUrl(url));
#elif WINDOWS
            System.Diagnostics.Process.Start(url);
#endif
        }

		public override void GetProperty(string propertyName, out Func<bool> getter, out Action<bool> setter)
		{
			switch(propertyName)
			{
				case "IsMute" : getter = () => this.IsMute; setter = (v) => this.IsMute = v; break;
				default:
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}

		public override void GetMethod(string methodName, out Func<List<Expression>, bool> method)
		{
            switch (methodName)
            {
				case "GetSeriesLevelHasBadge": method = (ps) => GetSeriesLevelHasBadge(ps[0].GetIntValue(), ps[1].GetIntValue()); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
		
		public override void GetMethod(string methodName, out Func<List<Lunohod.Xge.Expression>, double> method)
		{
            switch (methodName)
            {
                case "Rnd": method = (ps) => Rnd(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "Ceiling": method = (ps) => Ceiling(ps[0].GetNumValue()); break;
                case "Truncate": method = (ps) => Truncate(ps[0].GetNumValue()); break;
                case "RndX": method = (ps) => RndX(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "RndY": method = (ps) => RndY(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "Round": method = (ps) => Round(ps[0].GetNumValue(), ps.Count < 2 ? 0 : ps[1].GetIntValue()); break;
                case "Clamp": method = (ps) => Clamp(ps[0].GetNumValue(), ps[1].GetNumValue(), ps[2].GetNumValue()); break;
                case "Min": method = (ps) => Min(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "Max": method = (ps) => Max(ps[0].GetNumValue(), ps[1].GetNumValue()); break;
                case "Abs": method = (ps) => Abs(ps[0].GetNumValue()); break;
				case "IIf": method = (ps) => IIf(ps[0].GetBoolValue(), ps[1].GetNumValue(), ps[2].GetNumValue()); break;
				case "GetSeriesLevelStars": method = (ps) => GetSeriesLevelStars(ps[0].GetIntValue(), ps[1].GetIntValue()); break;
                case "GetSeriesUnlockedLevelNumber": method = (ps) => GetSeriesUnlockedLevelNumber(ps[0].GetIntValue()); break;
				case "GetCommandsPerSecond": method = (ps) => GetCommandsPerSecond(); break;
				case "GetSeriesLevelCount": method = (ps) => GetSeriesLevelCount(ps[0].GetIntValue()); break;
				case "GetSeriesScore": method = (ps) => GetSeriesScore(ps[0].GetIntValue()); break;
				case "GetSeriesStars": method = (ps) => GetSeriesStars(ps[0].GetIntValue()); break;
				case "GetSeriesAvailableStars": method = (ps) => GetSeriesAvailableStars(ps[0].GetIntValue()); break;
                case "GetSeriesLevelScore": method = (ps) => GetSeriesLevelScore(ps[0].GetIntValue(), ps[1].GetIntValue()); break;
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
                case "WriteLine": method = (ps) => WriteLine(ps[0].GetStrValue(), ps.GetRange(1, ps.Count - 1).Select(p => p.GetObjValue()).ToArray()); break;
                case "OpenUrl": method = (ps) => OpenUrl(ps[0].GetStrValue()); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
	}
}

