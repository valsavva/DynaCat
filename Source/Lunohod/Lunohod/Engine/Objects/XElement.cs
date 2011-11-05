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
			get { return this.backColor ?? this.ParentElement.BackColor; }
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
		
		[XmlIgnore]
		public XElement ParentElement { get; set; }
		
		public bool UseRotation
		{
			get { 
				if (this.rotation.HasValue)
					return true;
				if (this.ParentElement == null)
					return false;
				return this.ParentElement.UseRotation;
			}
		}
		
		public bool UseBackColor
		{
			get { return this.backColor.HasValue; }
		}
		
		public float GetScreenRotation()
		{
			if (this.ParentElement == null)
				return this.Rotation;

			return this.ParentElement.Rotation + this.Rotation;
		}
		
		public float GetScreenOpacity()
		{
			if (this.ParentElement == null)
				return this.Opacity;

			return this.ParentElement.Opacity * this.Opacity;
		}
		
		public Rectangle GetScreenBounds()
		{
			if (this.ParentElement == null)
				tmpBounds = this.Bounds;
			else
			{
				tmpBounds = this.ParentElement.GetScreenBounds();
	
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
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.ParentElement = this.Parent as XElement;
		}
		
        public virtual void ProcessCollision(LevelEngine engine, Rectangle newBounds)
        {
			
        }
    }
}
