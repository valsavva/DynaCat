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
		private double availablePoints;
		private double score;
		private double time;
		private double health;
		
		[XmlAttribute]
		public double AvaliablePoints { get { return availablePoints; } set { availablePoints = value; Recalculate(); } }
		[XmlAttribute]
		public double Score { get { return score; } set { score = value; Recalculate(); } }
		[XmlAttribute]
		public double Time { get { return time; } set { time = value; Recalculate(); } }
		[XmlAttribute]
		public double Health { get { return health; } set { health = value; Recalculate(); } }
		
		[XmlIgnore]
		public double ScoreRatio;
		[XmlIgnore]
		public double TimeBonus;
		[XmlIgnore]
		public double HealthBonus;
		[XmlIgnore]
		public double TotalScore;
		[XmlIgnore]
		public double NumberOfStars
		{
			get
			{
				if (ScoreRatio >= StarScoreRatio3)
					return 3;
				else if (ScoreRatio >= StarScoreRatio2)
					return 2;
				else if (ScoreRatio >= StarScoreRatio1)
					return 1;
				else
					return 0;
			}
		}
		[XmlIgnore]
		public bool HasBadge
		{
			get { return TimeBonus > 0 && HealthBonus > 0; }
		}
		
		public XLevelScore()
		{
		}
		
		public XLevelScore(XLevelInfo info)
			: base(info)
		{
			Recalculate();
		}
		
		public void CopyTo(XLevelScore other)
		{
			other.availablePoints = this.availablePoints;
			other.score = this.score;
			other.time = this.time;
			other.health = this.health;
			
			other.Recalculate();
		}
		
		private void Recalculate()
		{
			ScoreRatio = Score / (AvaliablePoints != 0 ? AvaliablePoints : 1);
			TimeBonus = ScoreRatio >= StarScoreRatio3 && Time <= TimeBonusThreshold ? Score * TimeBonusRatio : 0;
			HealthBonus = ScoreRatio >= StarScoreRatio3 && Health >= HealthBonusThreshold ? (Score + TimeBonus) * HealthBonusRatio : 0;
			TotalScore = Score + TimeBonus + HealthBonus;
		}
		
		public void Save()
		{
			var levelEngine = (LevelEngine)this.GetRoot().ScreenEngine;
			levelEngine.SaveNewScore(this);
		}
		
		public override void ReadXml(XmlReader reader)
		{
			this.Id = reader["Id"];
			this.AvaliablePoints = reader.ReadAttrAsFloat("AvailablePoints");
			this.Score = reader.ReadAttrAsFloat("Score");
			this.Time = reader.ReadAttrAsFloat("Time");
			this.Health = reader.ReadAttrAsFloat("Health");
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
                case "ScoreRatio": getter = () => ScoreRatio; setter = (v) => ScoreRatio = v; break;
                case "TimeBonus": getter = () => TimeBonus; setter = (v) => TimeBonus = v; break;
                case "HealthBonus": getter = () => HealthBonus; setter = (v) => HealthBonus = v; break;
                case "TotalScore": getter = () => TotalScore; setter = (v) => TotalScore = v; break;
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

