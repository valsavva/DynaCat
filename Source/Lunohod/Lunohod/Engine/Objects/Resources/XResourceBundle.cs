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
	public class XResourceBundle : XObject
	{
		public XResourceBundle()
		{
		}
		
		[XmlAttribute("RootFolder")]
		public string RootFolder;
		
        [XmlElement(ElementName = "Font", Type = typeof(XFontResource))]
        [XmlElement(ElementName = "Texture", Type = typeof(XTextureResource))]
        [XmlElement(ElementName = "MusicFile", Type = typeof(XMusicResource))]
        [XmlElement(ElementName = "SoundFile", Type = typeof(XSoundResource))]
        public XObjectCollection ResourceSubcomponents { get { return this.Subcomponents; } set { this.Subcomponents = value; } }
	}
}

