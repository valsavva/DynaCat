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

			if (this.image != null)
				return;

			for(int i = 0; i < p.Game.SpriteSheets.Count; i++)
			{
				var spriteSheet = p.Game.SpriteSheets[i];

				if (spriteSheet.Map.ContainsKey(this.Source))
				{
					this.SourceRectangle = spriteSheet.Map[this.Source];
					this.image = spriteSheet.Image;
					return;
				}
			}

			try
			{

	        	image = LoadResource<Texture2D>(p.Content, "TextureProcessor", "png", "xnb");

			}catch(Exception ex)
			{
				ex = ex;

				throw;
			}
		}
	}
}

