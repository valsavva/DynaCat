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
	[XmlType("Game")]
	public class XGame : XElement
	{
		public XGame()
		{
		}
		
		[XmlArray]
		public XLevel[] Levels;
	}
}

