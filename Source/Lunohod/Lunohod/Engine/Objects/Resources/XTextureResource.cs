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

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            image = LoadResource<Texture2D>(p.Game.Content, "TextureProcessor", "png", "xnb");
		}
		
		public override void Dispose()
		{
			//image.Dispose();

			base.Dispose();
		}
	}
}

