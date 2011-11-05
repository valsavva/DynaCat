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
    [XmlType("Image")]
    public class XImage : XElement
    {
		private XTextureResource texture;
		private Vector2 location;
		
        public XImage()
        {
        }

        [XmlAttribute]
        public string TextureId;
		
		[XmlAttribute]
		public bool Stretch;
		
		protected Rectangle? SourceRectangle;
		
		public override void Initialize (InitializeParameters p)
		{
			base.Initialize (p);

			this.texture = p.Resources.Textures[this.TextureId];
		}
		
		private Rectangle screenBounds;
		private Color actualBackColor;
		
		int c0, c1, c2;
		
		public override void Draw(	DrawParameters p)
		{
			screenBounds = this.GetScreenBounds();
			actualBackColor = this.BackColor * this.GetScreenOpacity();
            //var backColor = this.BackColor; backColor.A = (byte)(255.0f * this.GetScreenOpacity());
			
			
			if (this.Stretch || this.Bounds.IsEmpty)
			{
				if (this.UseRotation)
				{
					var screenRotation = MathHelper.ToRadians(this.GetScreenRotation());
					p.SpriteBatch.Draw(this.texture.Image, screenBounds, this.SourceRectangle, actualBackColor, screenRotation, this.Origin, SpriteEffects.None, 0);
				}
				else
					p.SpriteBatch.Draw(this.texture.Image, screenBounds, this.SourceRectangle, actualBackColor);
			}
			else
			{
				this.location.X = screenBounds.X;
				this.location.Y = screenBounds.Y;
				
				if (this.UseRotation)
				{
					var screenRotation = MathHelper.ToRadians(this.GetScreenRotation());
					p.SpriteBatch.Draw(this.texture.Image, this.location, this.SourceRectangle, actualBackColor, screenRotation, this.Origin, 1, SpriteEffects.None, 0);
				}
				else
					p.SpriteBatch.Draw(this.texture.Image, this.location, this.SourceRectangle, actualBackColor);
			}
		}
    }
}
