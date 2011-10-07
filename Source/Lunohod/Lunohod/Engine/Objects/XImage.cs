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

		public override void Initialize (InitializeParameters p)
		{
			base.Initialize (p);

			this.BackColor *= this.Opacity;
			this.texture = p.Resources.Textures[this.TextureId];
		}
		
		public override void Draw(	DrawParameters p)
		{
			if (this.Stretch || this.Bounds.IsEmpty)
				p.SpriteBatch.Draw(this.texture.Image, this.GetScreenBounds(), null, this.BackColor);
			else
			{
				var screenBounds = this.GetScreenBounds();
				this.location.X = screenBounds.X;
				this.location.Y = screenBounds.Y;
				p.SpriteBatch.Draw(this.texture.Image, this.location, null, this.BackColor);
			}
		}
    }
}
