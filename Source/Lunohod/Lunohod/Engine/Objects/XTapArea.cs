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
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);

			var touches = p.Game.Touches;
			
			for(int i = 0; i < touches.Count; i++)
			{
				var touch = touches[i];
				
				if (touch.State != TouchLocationState.Pressed)
					continue;
				if (!this.Bounds.Contains(touch.Position))
					continue;

				p.Game.EnqueueEvent(new GameEvent(this.Event, p.GameTime));
			}
		}
	}
}

