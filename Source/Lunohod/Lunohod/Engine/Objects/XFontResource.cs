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
	[XmlType("Font")]
	public class XFontResource : XResource
	{
		private SpriteFont font;
		
		[XmlAttribute]
        public string Source;
		
		public SpriteFont Font
		{
			get { return font; }
		}

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			XResourceBundle r = (XResourceBundle)this.Parent;
			
			string fileName = Path.Combine(r.RootFolder.Replace('/', Path.DirectorySeparatorChar), this.Source);

#if WINDOWS
            var sf = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), p.Game.Content.RootDirectory, fileName), "spritefont");
            var xnbFileName = Path.ChangeExtension(Path.Combine(Directory.GetCurrentDirectory(), p.Game.Content.RootDirectory, fileName), "xnb");
            var outputPath = Path.GetDirectoryName(sf);
            if (File.Exists(sf) && !File.Exists(xnbFileName))
            {
                using (Lunohod.ContentLoading.ContentBuilder b = new ContentLoading.ContentBuilder(outputPath))
                {
                    b.Add(sf, this.Source, "", "FontDescriptionProcessor");
                    b.Build();
                }
            }
#endif
            
            font = p.Game.Content.Load<SpriteFont>(fileName);
		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}

