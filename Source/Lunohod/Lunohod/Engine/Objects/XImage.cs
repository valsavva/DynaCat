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
		private Texture2D image;
		
        public XImage()
        {
            this.zBackColor = "#FFFFFFFF";
        }

        [XmlAttribute]
        public string Source;

		public override void Initialize (InitializeParameters p)
		{
			base.Initialize (p);
			
			byte opacityByte = (byte)(Math.Round(this.Opacity * 255));
			this.BackColor = new Color(opacityByte, opacityByte, opacityByte, opacityByte);
			
			string fileName = Path.Combine(p.Game.screenEngine.PrivateContentFolder, this.Source);
			
			image = p.Game.Content.Load<Texture2D>(fileName);
		}
		
		public override void Draw(DrawParameters p)
		{
			p.SpriteBatch.Draw(this.image, this.Bounds.ToXnaRectangle(), this.BackColor);
		}
    }
}
