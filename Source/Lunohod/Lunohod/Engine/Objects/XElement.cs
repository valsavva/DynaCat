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
		private System.Drawing.RectangleF tmpBounds;
		private Vector2 tmpVector;
		
		[XmlIgnore]
        public System.Drawing.RectangleF Bounds;
		[XmlIgnore]
        public Vector2 Location
		{
			get { this.Bounds.ToVector2(ref tmpVector); return tmpVector; }
			set { this.Bounds.X = value.X; this.Bounds.Y = value.Y; }
		}
        [XmlIgnore]
        public Color BackColor
		{
			get { return this.backColor ?? this.ParentElement.BackColor; }
			set { this.backColor = value; }
		}
        [XmlAttribute]
        public float Opacity = 1.0f;
        [XmlAttribute]
        public float Rotation;
		[XmlIgnore]
		public Vector2 Origin;
		
		[XmlIgnore]
		public XElement ParentElement { get; set; }
		
		public float GetScreenRotation()
		{
			if (this.ParentElement == null)
				return this.Rotation;
                                                              
			return this.ParentElement.GetScreenRotation() + this.Rotation;
		}
		
		public float GetScreenOpacity()
		{
			if (this.ParentElement == null)
				return this.Opacity;

			return this.ParentElement.Opacity * this.Opacity;
		}
		
		public System.Drawing.RectangleF GetScreenBounds()
		{
			if (this.ParentElement == null)
				tmpBounds = this.Bounds;
			else
			{
				tmpBounds = this.ParentElement.GetScreenBounds();
	
				if (!this.Bounds.IsZero())
				{
					tmpBounds.Offset(this.Bounds.X, this.Bounds.Y);
					tmpBounds.Width = this.Bounds.Width;
					tmpBounds.Height = this.Bounds.Height;
				}
			}
			
			return tmpBounds;
		}

		[XmlAttribute("Bounds")]
		public string zBounds
		{
			set { this.Bounds = value.ToRectF(); }
			get { return this.Bounds.ToStr(); }
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
			set { this.Location = value.ToVector2(); }
			get { return this.Location.ToStr(); }
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.ParentElement = this.FindAncestor(o => o is XElement) as XElement;
		}
		
		public bool Intersects(XElement e)
		{
			return this.GetScreenBounds().IntersectsWith(e.GetScreenBounds());
		}
		
		public System.Drawing.RectangleF Intersect(XElement e)
		{
			return System.Drawing.RectangleF.Intersect(this.GetScreenBounds(), e.GetScreenBounds());
		}
		
        public virtual bool ProcessCollision(LevelEngine level, System.Drawing.RectangleF intersect)
        {
			return false;
        }
		
		public override void Draw(DrawParameters p)
		{
			base.Draw(p);

            if (p.Game.DrawDebugInfo)
            {
                DrawDebug(p);
            }
        }
		
		public void DrawDebug(DrawParameters p)
		{
#if WINDOWS
#else
			return;
#endif
			if (!(this is XImage) && !(this is XBlock))
				return;
			
			tmpBounds =  this.GetScreenBounds();
		
#if WINDOWS
			tmpVector = MouseProcessor.LastPosition;
#else
			tmpVector = TouchPanelProcessor.LastPosition;
#endif
			
			if (!tmpBounds.Contains((int)tmpVector.X, (int)tmpVector.Y))
				return;
			
			Color c = Color.Red * 0.3f;
			
			
			p.SpriteBatch.Draw(p.Game.BlankTexture, tmpBounds, c);
			
			if (!string.IsNullOrEmpty(this.Id))
			{
				tmpVector.X = tmpBounds.X;
				tmpVector.Y = tmpBounds.Y;
				
				p.SpriteBatch.DrawString(p.Game.SystemFont, this.Id, tmpVector, Color.Yellow);
			}
		}
    }
}
