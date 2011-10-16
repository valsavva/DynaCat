using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;

namespace Lunohod.Objects
{
    [XmlType("Level")]
    public class XLevel : XElement
    {
        [XmlAttribute]
        public string Name;
		
        [XmlElement(ElementName = "Resources", Type = typeof(XResourceBundle))]
        [XmlElement(ElementName = "Layer", Type = typeof(XLayer))]
        public override List<XObject> Subcomponents { get; set; }
    }
}
