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
    /// <summary>
    /// Represents a base class for all graphical elements in XGAME
    /// </summary>
    [XmlRoot("Element")]
    //[XmlType("Element")]
    public class XElement : XObject
    {
		public struct ElementState
		{
			public int TransformCycle;
			public Matrix LocationTransform;
			public Matrix ScaleTransform;
		}
		
		public struct PropertiesState
		{
			public int PropCycle;
			public Vector2 Size;
			public float Rotation;
			public float Opacity;
			public Vector2 Scale;
			public Color BackColor;
			public System.Drawing.RectangleF? ScreenBounds;
		}
		
		private Vector2 rotationCenter;
		internal Color? backColor = null;
		private System.Drawing.RectangleF tmpBounds;
		protected Vector2 tmpVector1;
		protected Vector2 tmpVector2;
		
		private ElementState TransState;
		internal PropertiesState PropState;
		
		[XmlIgnore]
        public System.Drawing.RectangleF Bounds;
        [XmlAttribute]
        public float X { get { return this.Bounds.X; } set { this.Bounds.X = value; } }
        [XmlAttribute]
        public float Y { get { return this.Bounds.Y; } set { this.Bounds.Y = value; } }
        [XmlAttribute]
        public float Width { get { return this.Bounds.Width; } set { this.Bounds.Width = value; } }
        [XmlAttribute]
        public float Height { get { return this.Bounds.Height; } set { this.Bounds.Height = value; } }

        [XmlIgnore]
        public Vector2 Location
		{
			get { this.Bounds.ToVector2(ref tmpVector1); return tmpVector1; }
			set { this.Bounds.X = value.X; this.Bounds.Y = value.Y; }
		}
		[XmlIgnore]
        public Vector2 Center
		{
			get { this.Bounds.Center(ref tmpVector1); return tmpVector1; }
			set { this.Bounds.X = value.X - this.Bounds.Width / 2; this.Bounds.Y = value.Y - this.Bounds.Height / 2; }
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
		public Vector2 RotationCenter
		{
			get { return this.rotationCenter; }
			set { this.rotationCenter = this.Origin = value; }
		}
		[XmlIgnore]
		public Vector2 ScaleVector = Vector2.One;
		[XmlIgnore]
		public float Scale
		{
			get { return this.ScaleVector.X; }
			set { this.ScaleVector.X = this.ScaleVector.Y = value; }
		}
        [XmlIgnore]
        public XElement ParentElement;
		
		private void UpdateTransforms()
		{
			if (TransState.TransformCycle == this.updateCycle)
				return;
			
			// Initialize state
			if (this.ParentElement == null)
			{
				TransState.TransformCycle = this.updateCycle;
				TransState.LocationTransform = Matrix.Identity;
				TransState.ScaleTransform = Matrix.Identity;
			}
			else
			{
				this.ParentElement.UpdateTransforms();
				TransState = this.ParentElement.TransState;
			}			
			
			// Location transform
			if (this.Origin != Vector2.Zero)
				TransState.LocationTransform *= Matrix.CreateTranslation(-this.Origin.X, -this.Origin.Y, 0);
			
			if (this.Rotation != 0)
				TransState.LocationTransform *= Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation));
			// Scale transform
			if (this.ScaleVector != Vector2.One)
			{
				TransState.LocationTransform *= Matrix.CreateScale(this.ScaleVector.X, this.ScaleVector.Y, 1.0f);
				TransState.ScaleTransform *= Matrix.CreateScale(this.ScaleVector.X, this.ScaleVector.Y, 1.0f);
			}
			if (this.Bounds.X != 0 || this.Bounds.Y != 0 || this.RotationCenter != Vector2.Zero)
				TransState.LocationTransform *= Matrix.CreateTranslation(this.Bounds.X + this.RotationCenter.X, this.Bounds.Y + this.RotationCenter.Y, 0);
			
		}

		private void UpdateProps()
		{
			if (PropState.PropCycle == this.updateCycle)
				return;
			
			if  (this.ParentElement == null)
			{
				PropState.PropCycle = this.updateCycle;
				PropState.Opacity = 1.0f;
				PropState.Rotation = 0;
				PropState.Scale = Vector2.One;
				PropState.BackColor = Color.White;
			}
			else
			{
				this.ParentElement.UpdateProps();
				PropState = this.ParentElement.PropState;
			}
			
			PropState.ScreenBounds = null;
			//---
			PropState.Rotation += this.Rotation;
			PropState.Scale *= this.Scale;
			
			if (!this.Bounds.IsEmpty)
			{
				PropState.Size.X = this.Bounds.Width;
				PropState.Size.Y = this.Bounds.Height;
			}
			
			if (this.backColor.HasValue)
				PropState.BackColor = this.backColor.Value;
			
			PropState.Opacity *= this.Opacity;
		}
		
		internal System.Drawing.RectangleF GetScreenBounds()
		{
			if (this.ParentElement != null)
			{
				this.ParentElement.UpdateTransforms();
				
				this.UpdateProps();
			
				if (!PropState.ScreenBounds.HasValue)
				{
					tmpBounds = this.Bounds;
					
					if (this.ParentElement.TransState.LocationTransform != Matrix.Identity)
					{
						tmpVector1.X = this.Bounds.X + this.rotationCenter.X;
						tmpVector1.Y = this.Bounds.Y + this.rotationCenter.Y;
						Vector2.Transform(ref tmpVector1, ref this.ParentElement.TransState.LocationTransform, out tmpVector2);
						tmpBounds.X = tmpVector2.X;
						tmpBounds.Y = tmpVector2.Y;
					}

					if (this.ParentElement.TransState.ScaleTransform != Matrix.Identity)
					{
						Vector2.Transform(ref PropState.Size, ref this.ParentElement.TransState.ScaleTransform, out tmpVector2);
						tmpBounds.Width = tmpVector2.X;
						tmpBounds.Height = tmpVector2.Y;
					}
					else
					{
						tmpBounds.Width = PropState.Size.X;
						tmpBounds.Height = PropState.Size.Y;
					}
	
					PropState.ScreenBounds = tmpBounds;
				}
			}
			else
			{
				PropState.ScreenBounds = this.Bounds;
			}
			
			return PropState.ScreenBounds.Value;
		}

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.ParentElement = this.FindAncestor(o => o is XElement) as XElement;

            var exploding = this as IExploding;
			if (exploding != null &&  exploding.IsExploding)
			{
				((XLevel)this.GetRoot()).Exploding.Add(this);
			}
		}
		
		public bool Intersects(XElement e)
		{
			return this.GetScreenBounds().IntersectsWith(e.GetScreenBounds());
		}
		
        public virtual bool ProcessCollision(LevelEngine level, System.Drawing.RectangleF intersect)
        {
			return false;
        }
		
		public override void Draw(DrawParameters p)
		{
			base.Draw(p);

            if (p.Game.GameObject.ShowDebugInfo && p.Game.DrawDebugInfo)
            {
                DrawDebug(p);
            }
        }
		
		public void DrawDebug(DrawParameters p)
		{
			if (!(this is XImage) && !(this is XBlock))
				return;
			
			tmpBounds =  this.GetScreenBounds();
		
#if WINDOWS
			tmpVector1 = MouseProcessor.LastPosition;
#else
			tmpVector1 = TouchPanelProcessor.LastPosition;
#endif
			
			if (!tmpBounds.Contains((int)tmpVector1.X, (int)tmpVector1.Y))
				return;
			
			Color c = Color.Red * 0.3f;
			
			
			p.SpriteBatch.Draw(p.Game.BlankTexture, tmpBounds, c);
			
			if (!string.IsNullOrEmpty(this.Id))
			{
				tmpVector1.X = tmpBounds.X;
				tmpVector1.Y = tmpBounds.Y;
				
				p.SpriteBatch.DrawString(p.Game.SystemFont, this.Id, tmpVector1, Color.Yellow);
			}
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            Color color = Color.White;
            float f = 0;

            reader.ReadAttrAsFloat("Opacity", ref this.Opacity);
            reader.ReadAttrAsFloat("Rotation", ref this.Rotation);
            reader.ReadAttrAsVector2("ScaleVector", ref this.ScaleVector);

            reader.ReadAttrAsRectF("Bounds", ref this.Bounds);
            if (reader.ReadAttrAsColor("BackColor", ref color))
                this.BackColor = color;
            if (reader.ReadAttrAsVector2("Origin", ref tmpVector1))
                this.Origin = tmpVector1;
            if (reader.ReadAttrAsVector2("RotationCenter", ref tmpVector1))
                this.RotationCenter = tmpVector1;
            if (reader.ReadAttrAsFloat("Scale", ref f))
                this.Scale = f;
            if (reader.ReadAttrAsVector2("Location", ref tmpVector1))
                this.Location = tmpVector1;
            if (reader.ReadAttrAsVector2("Center", ref tmpVector1))
                this.Center = tmpVector1;

            base.ReadXml(reader);
        }
    }
}
