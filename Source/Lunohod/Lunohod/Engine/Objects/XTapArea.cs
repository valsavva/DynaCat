using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Lunohod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace Lunohod.Objects
{
	
    [XmlType("TapArea")]
	public class XTapArea : XElement
	{
		public XTapArea()
		{
		}
		
		[XmlAttribute]
		public string Event;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.Game.ScreenEngine.tapAreas.Add(this);
		}
	}
}

