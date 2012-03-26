using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	/// <summary>
	/// Represents Level settings. Zero or many settings can be specified for a single level.
	/// When many settings specified, all must have different values for the <see cref="Difficulty"/> attribute.
	/// </summary>
    [XmlType("LevelSettings")]
	public class XLevelSettings : XObject
	{
		/// <summary>
		/// These settings will be applied if the specified difficulty is the same as the difficulty chosen by the player.
		/// Default value is undefined.
		/// </summary>
		[XmlAttribute]
		public int Difficulty = -1;
		/// <summary>
		/// Specifies the number of explosives hero carries. Default value is -1 (unlimited).
		/// </summary>
		[XmlAttribute]
		public int BombCount = -1;
		/// <summary>
		/// Specifies the starting level of hero's health. Default value is 1.
		/// </summary>
		[XmlAttribute]
		public double HeroHealth = 1;
		/// <summary>
		/// The explosion class id that will be used to generate explosions on this level.
		/// </summary>
		[XmlAttribute]
		public string ExplosionClass;

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsInt("Difficulty", ref this.Difficulty);
            reader.ReadAttrAsInt("BombCount", ref this.BombCount);
            reader.ReadAttrAsFloat("HeroHealth", ref this.HeroHealth);
            this.ExplosionClass = reader["ExplosionClass"];

            base.ReadXml(reader);
        }
	}
}

