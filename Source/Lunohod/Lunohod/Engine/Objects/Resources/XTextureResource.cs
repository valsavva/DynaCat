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

		[XmlAttribute]
        public string Source;
		
		public Texture2D Image
		{
			get { return image; }
		}

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			XResourceBundle r = (XResourceBundle)this.Parent;
			
			string fileName = Path.Combine(r.RootFolder.Replace('/', Path.DirectorySeparatorChar), this.Source);

#if WINDOWS
            var pngFileName = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), p.Game.Content.RootDirectory, fileName), "png");
            var xnbFileName = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), p.Game.Content.RootDirectory, fileName), "xnb");
            var outputPath = Path.GetDirectoryName(pngFileName);
            if (File.Exists(pngFileName) && !File.Exists(xnbFileName))
            {
                using (Lunohod.ContentLoading.ContentBuilder b = new ContentLoading.ContentBuilder(outputPath))
                {
                    b.Add(pngFileName, this.Source, "", "TextureProcessor");
                    b.Build();
                }
            }
#endif

			image = p.Game.Content.Load<Texture2D>(fileName);
		}
		
		public override void Dispose()
		{
			image.Dispose();

			base.Dispose();
		}
	}
}

