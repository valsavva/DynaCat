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
	
    [XmlType("Explosion")]
	public class XExplosion : XElement
	{
		[XmlAttribute]
		public string Ranges;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
		}
	}
}

