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
    [XmlRoot("Level")]
    public class XLevel : XScreen
    {
		[XmlIgnore]
		public List<XElement> Exploding { get; private set; }
		
		public override void Initialize(InitializeParameters p)
		{
			this.Exploding = new List<XElement>();

			base.Initialize(p);
			
			this.EnqueueEvent(GameEventType.LevelLoaded);
		}
    }
}
