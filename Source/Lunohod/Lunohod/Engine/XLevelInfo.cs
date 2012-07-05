using System;
using System.Xml.Serialization;
using System.Globalization;
using System.Xml;

namespace Lunohod.Objects
{
    [XmlType("LevelInfo")]
	public class XLevelInfo : XObject
	{
		public int SeriesNumber;
		public int LevelNumber;

        [XmlAttribute]
        public string Name;
		[XmlAttribute]
		public string File;
		/// <summary>
		/// Specifies the number of explosives hero carries. Default value is -1 (unlimited).
		/// </summary>
		[XmlAttribute]
		public int BombCount = -1;
		/// <summary>
		/// Specifies the starting level of hero's health. Default value is 1.
		/// </summary>
		[XmlAttribute]
		public double HeroHealth = 100;
		/// <summary>
		/// The explosion class id that will be used to generate explosions on this level.
		/// </summary>
		[XmlAttribute]
		public string ExplosionClass = "clsExplosion";
		
		[XmlAttribute]
		public double StarScoreRatio1 = 0.01;
		[XmlAttribute]
		public double StarScoreRatio2 = 0.45;
		[XmlAttribute]
		public double StarScoreRatio3 = 0.90;
		[XmlAttribute]
		public double TimeBonusThreshold = 20;
		[XmlAttribute]
		public double HealthBonusThreshold = 100;
		[XmlAttribute]
		public double TimeBonusRatio = 0.25;
		[XmlAttribute]
		public double TimeExtraBonusRatio = 0.03;
		[XmlAttribute]
		public double HealthBonusRatio = 0.25;

		
		public XLevelInfo()
		{
		}
		
		public XLevelInfo(XLevelInfo info)
		{
			info.CopyTo(this);
		}

		public void CopyTo(XLevelInfo other)
		{
			other.Id = this.Id;
			other.SeriesNumber = this.SeriesNumber;
			other.LevelNumber = this.LevelNumber;
			other.Name = this.Name;
			other.File = this.File;
			other.BombCount = this.BombCount;
			other.HeroHealth = this.HeroHealth;
			other.ExplosionClass = this.ExplosionClass;
			other.StarScoreRatio1 = this.StarScoreRatio1;
			other.StarScoreRatio2 = this.StarScoreRatio2;
			other.StarScoreRatio3 = this.StarScoreRatio3;
			other.TimeBonusThreshold = this.TimeBonusThreshold;
			other.HealthBonusThreshold = this.HealthBonusThreshold;
			other.TimeBonusRatio = this.TimeBonusRatio;
			other.TimeExtraBonusRatio = this.TimeExtraBonusRatio;
			other.HealthBonusRatio = this.HealthBonusRatio;
		}
		
        public override void ReadXml(XmlReader reader)
        {
            reader.ReadAttrAsString("Name", ref this.Name);
            reader.ReadAttrAsString("File", ref this.File);
            reader.ReadAttrAsInt("BombCount", ref this.BombCount);
            reader.ReadAttrAsFloat("HeroHealth", ref this.HeroHealth);
            reader.ReadAttrAsString("ExplosionClass", ref this.ExplosionClass);
            
			reader.ReadAttrAsFloat("StarScoreRatio1", ref this.StarScoreRatio1);
			reader.ReadAttrAsFloat("StarScoreRatio2", ref this.StarScoreRatio2);
			reader.ReadAttrAsFloat("StarScoreRatio3", ref this.StarScoreRatio3);
			reader.ReadAttrAsFloat("TimeBonusThreshold", ref this.TimeBonusThreshold);
			reader.ReadAttrAsFloat("HealthBonusThreshold", ref this.HealthBonusThreshold);
			reader.ReadAttrAsFloat("TimeBonusRatio", ref this.TimeBonusRatio);
			reader.ReadAttrAsFloat("TimeExtraBonusRatio", ref this.TimeExtraBonusRatio);
			reader.ReadAttrAsFloat("HealthBonusRatio", ref this.HealthBonusRatio);
			
            base.ReadXml(reader);
			
			if (string.IsNullOrEmpty(this.Name))
				this.Name = this.Id;
        }

		public override void WriteXml(XmlWriter writer)
		{
			base.WriteXml(writer);
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("File", this.File);
            writer.WriteAttributeString("BombCount", this.BombCount.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("HeroHealth", this.HeroHealth.ToString(CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ExplosionClass", this.ExplosionClass);
            
			writer.WriteAttributeString("StarScoreRatio1", this.StarScoreRatio1.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("StarScoreRatio2", this.StarScoreRatio2.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("StarScoreRatio3", this.StarScoreRatio3.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("TimeBonusThreshold", this.TimeBonusThreshold.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("HealthBonusThreshold", this.HealthBonusThreshold.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("TimeBonusRatio", this.TimeBonusRatio.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("TimeExtraBonusRatio", this.TimeExtraBonusRatio.ToString(CultureInfo.InvariantCulture));
			writer.WriteAttributeString("HealthBonusRatio", this.HealthBonusRatio.ToString(CultureInfo.InvariantCulture));
		}
		
		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "SeriesNumber": getter = () => SeriesNumber; setter = null; break;
                case "LevelNumber": getter = () => LevelNumber; setter = null; break;
                case "BombCount": getter = () => BombCount; setter = (v) => BombCount = (int)Math.Round(v); break;
                case "HeroHealth": getter = () => HeroHealth; setter = (v) => HeroHealth = v; break;
                case "StarScoreRatio1": getter = () => StarScoreRatio1; setter = (v) => StarScoreRatio1 = v; break;
                case "StarScoreRatio2": getter = () => StarScoreRatio2; setter = (v) => StarScoreRatio2 = v; break;
                case "StarScoreRatio3": getter = () => StarScoreRatio3; setter = (v) => StarScoreRatio3 = v; break;
                case "TimeBonusThreshold": getter = () => TimeBonusThreshold; setter = (v) => TimeBonusThreshold = v; break;
                case "HealthBonusThreshold": getter = () => HealthBonusThreshold; setter = (v) => HealthBonusThreshold = v; break;
                case "TimeBonusRatio": getter = () => TimeBonusRatio; setter = (v) => TimeBonusRatio = v; break;
                case "TimeExtraBonusRatio": getter = () => TimeExtraBonusRatio; setter = (v) => TimeExtraBonusRatio = v; break;
                case "HealthBonusRatio": getter = () => HealthBonusRatio; setter = (v) => HealthBonusRatio = v; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
		
		public override void GetProperty(string propertyName, out Func<string> getter, out Action<string> setter)
		{
			switch (propertyName)
			{
                case "Name": getter = () => Name; setter = (v) => Name = v; break;
                case "File": getter = () => File; setter = (v) => File = v; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
	}
}

