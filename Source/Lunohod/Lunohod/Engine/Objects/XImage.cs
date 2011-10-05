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
		
        public XImage()
        {
            this.zBackColor = "#FFFFFFFF";
        }

        [XmlAttribute]
        public string TextureId;

		public override void Initialize (InitializeParameters p)
		{
			base.Initialize (p);

			this.BackColor *= this.Opacity;
		}
		
		public override void Draw(DrawParameters p)
		{
			if (this.texture == null)
				this.texture = p.Game.screenEngine.Resources.Textures[this.TextureId];

			p.SpriteBatch.Draw(this.texture.Image, this.Bounds.ToXnaRectangle(), null, this.BackColor);
		}
    }
}
