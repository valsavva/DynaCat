using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunohod;
using System.Xml;
using System.Xml.Schema;

namespace Lunohod.Objects
{
    [XmlRoot("Level")]
    [XmlSchemaProvider("MySchema")]
    public class XLevel : XScreen
    {
        public static XmlQualifiedName MySchema(XmlSchemaSet xs)
        {
            return new XmlQualifiedName("blah", @"http://www.w3.org/2001/XMLSchema");
        }
        
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
