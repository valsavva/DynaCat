using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
    [XmlInclude(typeof(XImage))]
    public class XElement : XComponent
    {
		private Color? backColor = null;
		
		[XmlIgnore]
        public Rectangle Bounds;
        [XmlIgnore]
        public Color BackColor
		{
			get { return this.backColor ?? ((XElement)this.Parent).BackColor; }
			set { this.backColor = value; }
		}
        [XmlAttribute]
        public float Opacity = 1.0f;
		
		public bool OverridesBackColor
		{
			get { return this.backColor.HasValue; }
		}
		
		public Rectangle GetScreenBounds()
		{
			if (this.Parent == null)
				return this.Bounds;
			
			var screenBounds = ((XElement)this.Parent).GetScreenBounds();

			if (this.Bounds.IsEmpty == false)
			{
				screenBounds.Offset(this.Bounds.X, this.Bounds.Y);
				screenBounds.Width = this.Bounds.Width;
				screenBounds.Height = this.Bounds.Height;
			}
			return screenBounds;
		}

		[XmlAttribute("Bounds")]
		public string zBounds
		{
			set { this.Bounds = value.ToRect(); }
			get { return Utility.ToBounds(this.Bounds); }
		}
        [XmlAttribute("BackColor")]
        public string zBackColor
		{
			set { this.BackColor = value.ToColor(); }
			get { return Utility.ToString(this.BackColor); }
		}
		
        [XmlElement(ElementName = "Tower", Type = typeof(XTower))]
        [XmlElement(ElementName = "Hero", Type = typeof(XHero))]
        [XmlElement(ElementName = "Image", Type = typeof(XImage))]
        [XmlElement(ElementName = "Block", Type = typeof(XBlock))]
        public override List<XComponent> Subcomponents { get; set; }

        public virtual void ProcessCollision(LevelEngine engine, Rectangle newBounds)
        {
            var hero = engine.hero;
            hero.AlignToBoundaryOf(this, hero.Move);
            hero.Move = XHeroMoveType.None;
        }
    }
}
