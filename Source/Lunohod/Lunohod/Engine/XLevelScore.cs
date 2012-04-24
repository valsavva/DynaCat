using System;
using System.Xml.Serialization;
using System.Xml;
using System.Globalization;

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
		
		public double ScoreRatio;
		public double TimeBonus;
		public double HealthBonus;
		public double TotalScore;
		
		public XLevelScore()
		{
			Recalculate();
		}
		public XLevelScore(XLevelInfo info)
			: base(info)
		{
			Recalculate();
		}
		
		private void Recalculate()
		{
			ScoreRatio = Score / (AvaliablePoints != 0 ? AvaliablePoints : 1);
			TimeBonus = ScoreRatio >= StarScoreRatio3 && Time <= TimeBonusThreshold ? Score * TimeBonusRatio : 0;
			HealthBonus = ScoreRatio >= StarScoreRatio3 && Health >= HealthBonusThreshold ? (Score + TimeBonus) * HealthBonusRatio : 0;
			TotalScore = Score + TimeBonus + HealthBonus;
		}
		
		public override void ReadXml(XmlReader reader)
		{
			base.ReadXml(reader);

			this.AvaliablePoints = reader.ReadAttrAsFloat("AvailablePoints");
			this.Score = reader.ReadAttrAsFloat("Score");
			this.Time = reader.ReadAttrAsFloat("Time");
			this.Health = reader.ReadAttrAsFloat("Health");
		}
		
		public override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement("LevelScore");

			base.WriteXml(writer);
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
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
	}
}

