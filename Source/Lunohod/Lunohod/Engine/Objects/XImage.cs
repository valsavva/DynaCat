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
		
		public override void Draw(	DrawParameters p)
		{
			var screenBounds = this.GetScreenBounds();
			var backColor = this.BackColor * this.GetScreenOpacity();
			
			if (this.Stretch || this.Bounds.IsEmpty)
			{
				if (this.UseRotation)
				{
					var screenRotation = MathHelper.ToRadians(this.GetScreenRotation());
					p.SpriteBatch.Draw(this.texture.Image, screenBounds, this.SourceRectangle, backColor, screenRotation, this.Origin, SpriteEffects.None, 0);
				}
				else
					p.SpriteBatch.Draw(this.texture.Image, screenBounds, this.SourceRectangle, backColor);
			}
			else
			{
				this.location.X = screenBounds.X;
				this.location.Y = screenBounds.Y;
				
				if (this.UseRotation)
				{
					var screenRotation = MathHelper.ToRadians(this.GetScreenRotation());
					p.SpriteBatch.Draw(this.texture.Image, this.location, this.SourceRectangle, backColor, screenRotation, this.Origin, 1, SpriteEffects.None, 0);
				}
				else
					p.SpriteBatch.Draw(this.texture.Image, this.location, this.SourceRectangle, backColor);
			}
		}
    }
}
