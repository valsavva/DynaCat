using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;

namespace Lunohod.Objects
{
    [XmlType("Layer")]
    public class XLayer : XElement
    {
        [XmlAttribute]
        public string Name;
	}
}
