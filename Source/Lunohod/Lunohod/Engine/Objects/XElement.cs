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
		private RectangleF? bounds;
		
		[XmlIgnore]
        public RectangleF Bounds
		{
			get {
				if (this.bounds.HasValue)
					return this.bounds.Value;
				else
					return ((XElement)this.Parent).Bounds;
			}
			set { this.bounds = value; }
		}
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
		
		public bool InheritsBounds
		{
			get {
				return this.bounds.HasValue == false;
			}
			set {
				if (value)
					this.bounds = null;
				else
					this.bounds = this.Bounds;
			}
		}
		
        [XmlElement(ElementName = "Image", Type = typeof(XImage))]
        [XmlElement(ElementName = "Tower", Type = typeof(XTower))]
        [XmlElement(ElementName = "Hero", Type = typeof(XHero))]
        [XmlElement(ElementName = "Block", Type = typeof(XBlock))]
        public override List<XComponent> Subcomponents { get; set; }

        public virtual void ProcessCollision(LevelEngine engine, RectangleF newBounds)
        {
            var hero = engine.hero;
            hero.AlignToBoundaryOf(this, hero.Move);
            hero.Move = XHeroMoveType.None;
        }
    }
}
