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
    [XmlType("LevelSettings")]
	public class XLevelSettings : XObject
	{
		[XmlAttribute]
		public int Dificulty = -1;
		[XmlAttribute]
		public int BombCount = -1;
		[XmlAttribute]
		public string ExplosionClass;
		
		public XLevelSettings()
		{
		}
	}
}

