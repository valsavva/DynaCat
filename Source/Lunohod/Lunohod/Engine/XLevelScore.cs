using System;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    [XmlType("LevelScore")]
	public class XLevelScore : XLevelInfo
	{
		private bool calculated;

		private double availablePoints;
		private double score;
		private double time;
		private double health;


		private double scoreRatio;
		private double timeBonus;
		private double healthBonus;
		private double totalScore;
		private double numberOfStars;
		private bool hasBadge;

		public XLevelScore()
		{
		}

		public XLevelScore(XLevelInfo info)
			: base(info)
		{
		}
		
		[XmlAttribute]
		public double AvaliablePoints { get { return availablePoints; } set { availablePoints = value; calculated = false; } }
		[XmlAttribute]
		public double Score { get { return score; } set { score = value; calculated = false; } }
		[XmlAttribute]
		public double Time { get { return time; } set { time = value; calculated = false; } }
		[XmlAttribute]
		public double Health { get { return health; } set { health = value; calculated = false; } }
		
		[XmlIgnore]
		public double ScoreRatio
		{
			get { if (!calculated) Recalculate(); return scoreRatio; }
		}
		[XmlIgnore]
		public double TimeBonus
		{
			get { if (!calculated) Recalculate(); return timeBonus; }
		}
		[XmlIgnore]
		public double HealthBonus
		{
			get { if (!calculated) Recalculate(); return healthBonus; }
		}
		[XmlIgnore]
		public double TotalScore
		{
			get { if (!calculated) Recalculate(); return totalScore; }
		}
		[XmlIgnore]
		public double NumberOfStars
		{
			get { if (!calculated) Recalculate(); return numberOfStars; }
		}
		[XmlIgnore]
		public bool HasBadge
		{
			get { return TimeBonus > 0 && HealthBonus > 0; }
		}

		public void CopyTo(XLevelScore other)
		{
			other.availablePoints = this.availablePoints;
			other.score = this.score;
			other.time = this.time;
			other.health = this.health;
			
			other.calculated = false;
		}
		
		private void Recalculate()
		{
			if (calculated)
				return;

			// get max score whenever we get a chance
			var screen = this.GetRoot() as XScreen;
			if (screen != null)
			{
				var levelEngine = screen.ScreenEngine as LevelEngine;
				if (levelEngine != null)
					availablePoints = levelEngine.CountLevelPoints();
			}

			scoreRatio = score / (availablePoints != 0 ? availablePoints : 1);

			if (scoreRatio >= StarScoreRatio3)
				numberOfStars = 3;
			else if (scoreRatio >= StarScoreRatio2)
				numberOfStars = 2;
			else if (scoreRatio >= StarScoreRatio1)
				numberOfStars = 1;
			else
				numberOfStars = 0;

			timeBonus = numberOfStars == 3 && time <= TimeBonusThreshold ? score * (TimeBonusRatio + TimeExtraBonusRatio * (TimeBonusThreshold - time)) : 0;
			healthBonus = numberOfStars == 3 && health >= HealthBonusThreshold ? (score + timeBonus) * HealthBonusRatio : 0;
			totalScore = score + timeBonus + healthBonus;

			calculated = true;
		}
		
		public void Save()
		{
			var levelEngine = (LevelEngine)this.GetScreen().ScreenEngine;
			levelEngine.SaveNewScore(this);
		}
		
		public override void ReadXml(XmlReader reader)
		{
			this.Id = reader["Id"];
			reader.ReadAttrAsFloat("AvailablePoints", ref availablePoints);
			reader.ReadAttrAsFloat("Score", ref score);
			reader.ReadAttrAsFloat("Time", ref time);
			reader.ReadAttrAsFloat("Health", ref health);
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("LevelScore");
			
			writer.WriteAttributeString("Id", this.Id);
			writer.WriteAttributeString("AvailablePoints", this.AvaliablePoints.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Score", this.Score.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Time", this.Time.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("Health", this.Health.ToString(CultureInfo.InvariantCulture));
			
			writer.WriteEndElement();
		}
		
		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "AvaliablePoints": getter = () => AvaliablePoints; setter = (v) => AvaliablePoints = v; break;
                case "Score": getter = () => Score; setter = (v) => Score = v; break;
                case "Time": getter = () => Time; setter = (v) => Time = v; break;
                case "Health": getter = () => Health; setter = (v) => Health = v; break;
                case "ScoreRatio": getter = () => ScoreRatio; setter = null; break;
                case "TimeBonus": getter = () => TimeBonus; setter = null; break;
                case "HealthBonus": getter = () => HealthBonus; setter = null; break;
                case "TotalScore": getter = () => TotalScore; setter = null; break;
                case "NumberOfStars": getter = () => NumberOfStars; setter = null; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
		
		public override void GetProperty(string propertyName, out Func<bool> getter, out Action<bool> setter)
		{
			switch (propertyName)
			{
                case "HasBadge": getter = () => HasBadge; setter = null; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
		
		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
                case "Save": method = (ps) => Save(); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
	}
}

