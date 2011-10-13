using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("Hero")]
    public class XHero : XElement
    {
        [XmlAttribute]
        public float DefaultSpeed;
		[XmlIgnore]
		public float Speed;
		
		[XmlIgnore]
		public Vector2 Direction;

        [XmlAttribute("Direction")]
        public string zDirection
		{
			get { return this.Direction.ToStr(); }
			set { this.Direction = value.ToVector2(); }
		}
		
        public void AlignToBoundaryOf(XElement e, Vector2 direction)
        {
        }
    }
}
