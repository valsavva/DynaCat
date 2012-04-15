using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    [XmlType("Tower")]
    public class XTower : XElement
    {
        [XmlAttribute]
        public double SignalSpeed { get; set; }
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.LevelEngine.Tower = this;
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.SignalSpeed = reader.ReadAttrAsFloat("SignalSpeed");

            base.ReadXml(reader);
        }
		
		public override void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			switch (propertyName)
			{
                case "SignalSpeed": getter = () => SignalSpeed; setter = (v) => SignalSpeed = v; break;
				default :
					base.GetProperty(propertyName, out getter, out setter); break;
			}
		}
    }
}
