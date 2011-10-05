using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	[XmlType("Resources")]	
	public class XResourceBundle : IDisposable
	{
		public XResourceBundle()
		{
		}
		
		[XmlAttribute("RootFolder")]
		public string RootFolder;
		
        [XmlElement(ElementName = "Texture", Type = typeof(XTextureResource))]
		public XResource[] Resources;
		
		[XmlIgnore]
		public Dictionary<string, XTextureResource> Textures = new Dictionary<string, XTextureResource>();
		
		public void Initialize(InitializeParameters p)
		{
			foreach(var o in this.Resources)
			{
				o.Initialize(p, this);
				
				if (o is XTextureResource)
				{
					this.Textures.Add(o.Id, (XTextureResource)o);
				}
			}
		}
		
		public void Dispose()
		{
			foreach(var o in this.Resources)
				o.Dispose();
		}
	}
}

