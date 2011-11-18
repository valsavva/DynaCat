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

			font = p.Game.Content.Load<SpriteFont>(fileName);
		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}

