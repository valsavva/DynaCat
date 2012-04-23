using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    [XmlType("LevelInfo")]
	public class XLevelInfo : XObject
	{
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
		public double TimeBonus = 0.25;
		[XmlAttribute]
		public double HealthBonus = 0.25;

        public override void ReadXml(System.Xml.XmlReader reader)
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
			reader.ReadAttrAsFloat("TimeBonus", ref this.TimeBonus);
			reader.ReadAttrAsFloat("HealthBonus", ref this.HealthBonus);
			
            base.ReadXml(reader);
			
			if (string.IsNullOrEmpty(this.Name))
				this.Name = this.Id;
        }

		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "BombCount": getter = () => BombCount; setter = (v) => BombCount = (int)Math.Round(v); break;
                case "HeroHealth": getter = () => HeroHealth; setter = (v) => HeroHealth = v; break;
                case "StarScoreRatio1": getter = () => StarScoreRatio1; setter = (v) => StarScoreRatio1 = v; break;
                case "StarScoreRatio2": getter = () => StarScoreRatio2; setter = (v) => StarScoreRatio2 = v; break;
                case "StarScoreRatio3": getter = () => StarScoreRatio3; setter = (v) => StarScoreRatio3 = v; break;
                case "TimeBonusThreshold": getter = () => TimeBonusThreshold; setter = (v) => TimeBonusThreshold = v; break;
                case "HealthBonusThreshold": getter = () => HealthBonusThreshold; setter = (v) => HealthBonusThreshold = v; break;
                case "TimeBonus": getter = () => TimeBonus; setter = (v) => TimeBonus = v; break;
                case "HealthBonus": getter = () => HealthBonus; setter = (v) => HealthBonus = v; break;
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

