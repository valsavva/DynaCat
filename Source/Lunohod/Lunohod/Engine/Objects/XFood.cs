using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
    [XmlType("Food")]
	public class XFood : XElement
	{
		[XmlAttribute]
		public float Points;
	}
}

