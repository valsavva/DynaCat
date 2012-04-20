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
	public class XFood : XElement, IExploding, IHasPoints
	{
		[XmlAttribute]
		public double Points { get; set; }
        /// <inheritdoc />
        [XmlAttribute]
        public bool IsExploding { get; set; }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
			this.Points = reader.ReadAttrAsFloat("Points", 0);

            base.ReadXml(reader);
        }
    }
}

