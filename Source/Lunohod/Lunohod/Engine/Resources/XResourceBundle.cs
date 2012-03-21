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
		
        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.RootFolder = reader["RootFolder"];
            
            base.ReadXml(reader);
        }
	}
}

