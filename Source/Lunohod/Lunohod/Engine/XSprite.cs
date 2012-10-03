using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("Sprite")]
	public class XSprite : XImage
	{
		[XmlIgnore]
		public System.Drawing.RectangleF FrameBounds;
		[XmlAttribute]
		public int FrameCount;
        [XmlAttribute]
        public int CurrentFrame;

        private System.Drawing.RectangleF? originalSourceRectangle;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            int textureWidth = this.SourceRectangle.HasValue ? (int)this.SourceRectangle.Value.Width : this.texture.Image.Width;

            if (this.FrameBounds.Width * FrameCount > textureWidth)
                throw new InvalidOperationException("Specified FrameCount exceeds the Width of the texture");

            this.originalSourceRectangle = this.SourceRectangle;
		}
		
		public override void Update(UpdateParameters p)
		{
			if (this.FrameCount == 0)
			{
				this.tmpBounds.X = this.FrameBounds.X + this.CurrentFrame * this.FrameBounds.Width;
				this.tmpBounds.Y = this.FrameBounds.Y;
			}
			else
			{
				this.tmpBounds.X = this.FrameBounds.X + (this.CurrentFrame % this.FrameCount) * this.FrameBounds.Width;
				this.tmpBounds.Y = this.FrameBounds.Y + (this.CurrentFrame / this.FrameCount) * this.FrameBounds.Height;
			}
			
			this.tmpBounds.Width = this.FrameBounds.Width;
			this.tmpBounds.Height = this.FrameBounds.Height;

            if (originalSourceRectangle.HasValue)
                this.tmpBounds.Offset(originalSourceRectangle.Value.X, originalSourceRectangle.Value.Y);
			
			this.SourceRectangle = this.tmpBounds;
			
			base.Update(p);
		}
		
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsRectF("FrameBounds", ref this.FrameBounds);
            reader.ReadAttrAsInt("FrameCount", ref this.FrameCount);
            reader.ReadAttrAsInt("CurrentFrame", ref this.CurrentFrame);

            base.ReadXml(reader);
        }
		
		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "CurrentFrame": getter = () => CurrentFrame; setter = (v) => CurrentFrame = (int)Math.Round(v); break;
                case "FrameCount": getter = () => FrameCount; setter = (v) => FrameCount = (int)Math.Round(v); break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
	}
}

