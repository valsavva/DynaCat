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
	public class XResourceBundle : XComponent
	{
		public XResourceBundle()
		{
		}
		
		[XmlAttribute("RootFolder")]
		public string RootFolder;
		
        [XmlElement(ElementName = "Texture", Type = typeof(XTextureResource))]
		public override XComponent[] Subcomponents {get; set;}
		
		[XmlIgnore]
		public Dictionary<string, XTextureResource> Textures = new Dictionary<string, XTextureResource>();
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			this.Textures = this.GetComponents<XTextureResource>().ToDictionary(t => t.Id);
		}
	}
}

