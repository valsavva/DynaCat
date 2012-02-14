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
    [XmlType("Image")]
    public class XImage : XElement
    {
		private XTextureResource texture;

        public XImage()
        {
        }

        [XmlAttribute]
        public string TextureId;
		
		[XmlAttribute]
		public bool Stretch = true;
		
		protected Rectangle? SourceRectangle;
		
		public override void Initialize (InitializeParameters p)
		{
			base.Initialize (p);

			this.texture = (XTextureResource)p.ScreenEngine.RootComponent.FindDescendant(this.TextureId);
			
			if (this.texture == null)
				throw new InvalidOperationException(string.Format(
					"Texture was not found. ImageID:{0} TextureId:{1}", this.Id, this.TextureId
                ));
		}
		
        private float screenRotation;
		private Color actualBackColor;
		
        public override void Update(UpdateParameters p)
        {
            base.Update(p);
			
            this.GetScreenBounds();
            actualBackColor = this.PropState.BackColor * this.PropState.Opacity;
			screenRotation = MathHelper.ToRadians(this.PropState.Rotation);
        }

		public override void Draw(DrawParameters p)
		{
			if (this.PropState.ScreenBounds.HasValue)
			{
				if ((this.Bounds.X != 0 || this.Bounds.Y != 0) && this.Bounds.Width == 0 && this.Bounds.Height == 0)
				{
					// we're using location
					this.tmpVector1.X = this.PropState.ScreenBounds.Value.X;
					this.tmpVector1.Y = this.PropState.ScreenBounds.Value.Y;
					
					if (screenRotation != 0 || this.Origin != Vector2.Zero || this.PropState.Scale != Vector2.One)
						p.SpriteBatch.Draw(this.texture.Image, this.tmpVector1, this.SourceRectangle, actualBackColor, screenRotation, this.Origin, this.PropState.Scale, SpriteEffects.None, 0);
					else
						p.SpriteBatch.Draw(this.texture.Image, this.tmpVector1, this.SourceRectangle, actualBackColor);
				}
				else
				{
					if (screenRotation != 0 || this.Origin != Vector2.Zero)
						p.SpriteBatch.Draw(this.texture.Image, this.PropState.ScreenBounds.Value, this.SourceRectangle, actualBackColor, screenRotation, this.Origin, SpriteEffects.None, 0);
					else
						p.SpriteBatch.Draw(this.texture.Image, this.PropState.ScreenBounds.Value, this.SourceRectangle, actualBackColor);
				}
			}
			base.Draw(p);
		}
    }
}
