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
    public class XElement : XObject
    {
		private Color? backColor = null;
		private float? rotation;
		private Rectangle tmpBounds;
		
		[XmlIgnore]
        public Rectangle Bounds;
		[XmlIgnore]
        public Point Location;
        [XmlIgnore]
        public Color BackColor
		{
			get { return this.backColor ?? ((XElement)this.Parent).BackColor; }
			set { this.backColor = value; }
		}
        [XmlAttribute]
        public float Opacity = 1.0f;
        [XmlAttribute]
        public float Rotation
		{
			get { return this.rotation ?? 0.0f; }
			set { this.rotation = value; }
		}
		[XmlIgnore]
		public Vector2 Origin;
		
		public bool UseRotation
		{
			get { 
				if (this.rotation.HasValue)
					return true;
				
				XElement parent = this.Parent as XElement;
				return parent == null ? false : parent.UseRotation;
			}
		}
		
		public bool OverridesBackColor
		{
			get { return this.backColor.HasValue; }
		}
		
		public float GetScreenRotation()
		{
			XElement parent = this.Parent as XElement;

			if (this.Parent == null)
				return this.Rotation;

			return parent.Rotation + this.Rotation;
		}
		
		public float GetScreenOpacity()
		{
			XElement parent = this.Parent as XElement;

			if (this.Parent == null)
				return this.Opacity;

			return parent.Opacity * this.Opacity;
		}
		
		public Rectangle GetScreenBounds()
		{
			XElement parent = this.Parent as XElement;

			if (parent == null)
				tmpBounds = this.Bounds;
			else
			{
				tmpBounds = parent.GetScreenBounds();
	
				if (this.Bounds.IsEmpty == false)
				{
					tmpBounds.Offset(this.Bounds.X, this.Bounds.Y);
					tmpBounds.Width = this.Bounds.Width;
					tmpBounds.Height = this.Bounds.Height;
				}
			}
			
			tmpBounds.Offset(this.Location);
			
			return tmpBounds;
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
			get { return this.BackColor.ToStr(); }
		}
		[XmlAttribute("Origin")]
		public string zOrigin
		{
			set { this.Origin = value.ToVector2(); }
			get { return this.Origin.ToStr(); }
		}
		[XmlAttribute("Location")]
		public string zLocation
		{
			set { this.Location = value.ToPoint(); }
			get { return this.Location.ToStr(); }
		}
		
        [XmlElement(ElementName = "Tower", Type = typeof(XTower))]
        [XmlElement(ElementName = "Hero", Type = typeof(XHero))]
        [XmlElement(ElementName = "Image", Type = typeof(XImage))]
        [XmlElement(ElementName = "Block", Type = typeof(XBlock))]
        [XmlElement(ElementName = "Sprite", Type = typeof(XSprite))]
        [XmlElement(ElementName = "IntValueAnimation", Type = typeof(XIntValueAnimation))]
        [XmlElement(ElementName = "FloatValueAnimation", Type = typeof(XFloatValueAnimation))]
        public override List<XObject> Subcomponents { get; set; }

        public virtual void ProcessCollision(LevelEngine engine, Rectangle newBounds)
        {
        }
    }
}
