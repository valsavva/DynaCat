using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Lunohod.Objects
{
	public enum XImageStretchMode
	{
		Auto = 0,
		ActualSize = 1,
		Stretch = 2,
		Tile = 3
	}

    [XmlType("Image")]
    public class XImage : XElement
    {
		protected XTextureResource texture;

        public XImage()
        {
        }

        [XmlAttribute]
        public string TextureId;
		[XmlAttribute]
		public XImageStretchMode StretchMode;
		[XmlAttribute]
		public XFlipEffects FlipEffects;
		
		protected System.Drawing.RectangleF? SourceRectangle;
		
		public override void Initialize (InitializeParameters p)
		{
			base.Initialize (p);

            if (string.IsNullOrEmpty(this.TextureId))
                throw new InvalidOperationException(string.Format(
                    "TextureId must be specified. Image: '{0}'", this.Id
                ));

			this.texture = (XTextureResource)this.FindGlobal(this.TextureId);
			
			if (this.texture == null)
				throw new InvalidOperationException(string.Format(
					"Texture was not found. Image: '{0}' TextureId: '{1}'", this.Id, this.TextureId
                ));
		}
		
        private double screenRotation;
		private Color actualBackColor;

        public override void Update(UpdateParameters p)
        {
            base.Update(p);
			
            this.GetScreenBounds();
            actualBackColor = this.PropState.BackColor * (float)this.PropState.Opacity;
			screenRotation = MathHelper.ToRadians((float)this.PropState.Rotation);
        }

		public override void Draw(DrawParameters p)
		{
			if (this.PropState.ScreenBounds.HasValue)
			{
				// first resolve the actual way we want to draw the image
				XImageStretchMode actualStretchMode = this.StretchMode;

				if (actualStretchMode == XImageStretchMode.Auto)
				{
					if ((this.Bounds.X != 0 || this.Bounds.Y != 0) && this.Bounds.Width == 0 && this.Bounds.Height == 0)
						actualStretchMode = XImageStretchMode.ActualSize;
					else
						actualStretchMode = XImageStretchMode.Stretch;
				}

				// draw it
				if (actualStretchMode == XImageStretchMode.ActualSize)
					DrawActualSize(p);
				else if (actualStretchMode == XImageStretchMode.Stretch)
					DrawStretched(p);
				else
					DrawTiles(p);
			}
			base.Draw(p);
		}

		void DrawActualSize(DrawParameters p)
		{
			// we're using location
			this.tmpVector1.X = this.PropState.ScreenBounds.Value.X;
			this.tmpVector1.Y = this.PropState.ScreenBounds.Value.Y;
			
			p.SpriteBatch.Draw(this.texture.Image, this.tmpVector1, this.SourceRectangle, actualBackColor, (float)screenRotation, this.Origin, this.PropState.Scale, (SpriteEffects)this.FlipEffects, this.PropState.Depth + p.NextSystemImageDepth());
		}

		void DrawStretched(DrawParameters p)
		{
			p.SpriteBatch.Draw(this.texture.Image, this.PropState.ScreenBounds.Value, this.SourceRectangle, actualBackColor, screenRotation, this.Origin, (SpriteEffects)this.FlipEffects, this.PropState.Depth + p.NextSystemImageDepth());
		}

		void DrawTiles(DrawParameters p)
		{
			var depth = this.PropState.Depth + p.NextSystemImageDepth();
			var imageRes = this.texture.Image.Bounds;
			Rectangle screenBounds = Rectangle.Empty;
			this.PropState.ScreenBounds.Value.ToRectangle(ref screenBounds);

			// fill whole rows and cols first
			int wholeCols = screenBounds.Width / imageRes.Width;
			int wholeRows = screenBounds.Height / imageRes.Height;

			Vector2 loc = new Vector2(screenBounds.X, screenBounds.Y);

			for(int row = 0; row < wholeRows; row++)
			{
				loc.X = screenBounds.X;
				for(int col = 0; col < wholeCols; col++)
				{
					p.SpriteBatch.Draw(this.texture.Image, loc, null, actualBackColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
					loc.X += imageRes.Width;
				}
				loc.Y += imageRes.Height;
			}


            // fill margins
            int rightMargin = screenBounds.Width % imageRes.Width;
            int bottomMargin = screenBounds.Height % imageRes.Height;
            Rectangle sourceBounds = imageRes;

            if (rightMargin != 0)
            {
                loc.X = screenBounds.X + wholeCols * imageRes.Width;
                loc.Y = screenBounds.Y;

                sourceBounds.Width = rightMargin;

                for (int row = 0; row < wholeRows; row++)
                {
                    p.SpriteBatch.Draw(this.texture.Image, loc, sourceBounds, actualBackColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                    loc.Y += imageRes.Height;
                }


                if (bottomMargin != 0)
                {
                    sourceBounds.Height = bottomMargin;
                    p.SpriteBatch.Draw(this.texture.Image, loc, sourceBounds, actualBackColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                }
            }


            if (bottomMargin != 0)
            {
                loc.X = screenBounds.X;

                sourceBounds.Width = imageRes.Width;

                for (int col = 0; col < wholeCols; col++)
                {
                    p.SpriteBatch.Draw(this.texture.Image, loc, sourceBounds, actualBackColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, depth);
                    loc.X += imageRes.Width;
                }
            }

        }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsString("TextureId", ref this.TextureId);
            reader.ReadAttrAsEnum<XImageStretchMode>("StretchMode", ref this.StretchMode);
			reader.ReadAttrAsEnum<XFlipEffects>("FlipEffects", ref this.FlipEffects);

            base.ReadXml(reader);
        }

        internal override void ReplaceParameter(string par, string val)
        {
            if (this.TextureId != null)
                this.TextureId.Replace(par, val);

            base.ReplaceParameter(par, val);
        }
    }
}
