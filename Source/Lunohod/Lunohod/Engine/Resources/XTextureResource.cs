using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	[XmlType("Texture")]
	public class XTextureResource : XResource
	{
		private Texture2D image;
		
		public XTextureResource ()
		{
		}

		public Texture2D Image
		{
			get { return image; }
		}

        public System.Drawing.RectangleF? SourceRectangle;

		public override void InitializeMainThread(InitializeParameters p)
		{
			base.InitializeMainThread(p);

            XSpriteSheetResource spriteSheet = null;

            if (p.Game.GlobalSpriteSheets != null)
            {
                foreach (XSpriteSheetResource tmpSheet in p.Game.GlobalSpriteSheets)
                {
                    if (tmpSheet.Map.ContainsKey(this.Source))
                    {
                        spriteSheet = tmpSheet;
                        break;
                    }
                }

            }

            if (spriteSheet != null)
            {
                this.SourceRectangle = spriteSheet.Map[this.Source];
                this.image = spriteSheet.Image;
            }
            else
                image = LoadResource<Texture2D>(p.Game.Content, "TextureProcessor", "png", "xnb");
		}

		public override void Dispose()
		{
			//image.Dispose();

			base.Dispose();
		}
	}
}

