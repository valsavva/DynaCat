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

		public override void Initialize(InitializeParameters p, XResourceBundle r)
		{
			base.Initialize(p, r);
			
//			string fileName = Path.Combine(
//				Path.Combine(p.Game.Content.RootDirectory, r.RootFolder), this.Source
//			);
			
			string fileName = Path.Combine(r.RootFolder, this.Source);
			
			image = p.Game.Content.Load<Texture2D>(fileName);
		}
		
		public override void Dispose()
		{
			base.Dispose();
			
			image.Dispose();
		}
	}
}

