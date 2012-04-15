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
		public double StarScore1 = 0.01;
		[XmlAttribute]
		public double StarScore2 = 0.45;
		[XmlAttribute]
		public double StarScore3 = 0.90;
		[XmlAttribute]
		public double SpeedBonusThreshold = 10;
		[XmlAttribute]
		public double HealthBonusThreshold = 100;
		[XmlAttribute]
		public double SpeedBonus = 0.25;
		[XmlAttribute]
		public double HealthBonus = 0.25;

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Name = reader["Name"];
            this.File = reader["File"];
            reader.ReadAttrAsInt("BombCount", ref this.BombCount);
            reader.ReadAttrAsFloat("HeroHealth", ref this.HeroHealth);
            this.ExplosionClass = reader["ExplosionClass"];

            base.ReadXml(reader);
			
			if (string.IsNullOrEmpty(this.Name))
				this.Name = this.Id;
        }
	}
}

