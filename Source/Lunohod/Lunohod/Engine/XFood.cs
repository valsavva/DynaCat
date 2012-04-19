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
	public class XFood : XElement, IExploding
	{
		[XmlAttribute]
		public double Points;
        /// <inheritdoc />
        [XmlAttribute]
        public bool IsExploding { get; set; }

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsFloat("Points", ref this.Points);

            base.ReadXml(reader);
        }

        public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
        {
            switch (propertyName)
            {
                case "Points": getter = () => Points; setter = (v) => Points = v; break;
                default:
                    base.GetProperty(propertyName, out getter, out setter); break;
            }
        }
    }
}

