using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using System.Drawing;

namespace Lunohod.Objects
{
    [XmlInclude(typeof(XImage))]
    public class XElement : XComponent
    {
		[XmlIgnore]
        public RectangleF Bounds;
        [XmlIgnore]
        public Color BackColor;
        [XmlAttribute]
        public float Opacity = 1.0f;

		[XmlAttribute("Bounds")]
		public string zBounds
		{
			set { this.Bounds = value.ToRect(); }
			get { return Utility.ToBounds(this.Bounds); }
		}
        [XmlAttribute("Color")]
        public string zBackColor
		{
			set { this.BackColor = value.ToColor(); }
			get { return Utility.ToString(this.BackColor); }
		}

        [XmlElement(ElementName = "Image", Type = typeof(XImage))]
        [XmlElement(ElementName = "Tower", Type = typeof(XTower))]
        [XmlElement(ElementName = "Hero", Type = typeof(XHero))]
        [XmlElement(ElementName = "Block", Type = typeof(XBlock))]
        public override XComponent[] Subcomponents { get; set; }

        public virtual void ProcessCollision(LevelEngine engine, RectangleF newBounds)
        {
            var hero = engine.hero;
            hero.AlignToBoundaryOf(this, hero.Move);
            hero.Move = XHeroMoveType.None;
        }
    }
}
