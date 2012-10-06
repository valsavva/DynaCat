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
		
		public SpriteFont Font
		{
			get { return font; }
		}

		public override void InitializeMainThread(InitializeParameters p)
		{
			base.InitializeMainThread(p);

            font = LoadResource<SpriteFont>(p.Content, "FontDescriptionProcessor", "spritefont", "xnb");
		}
		
		public override void Dispose()
		{
			base.Dispose();
		}
	}
}

