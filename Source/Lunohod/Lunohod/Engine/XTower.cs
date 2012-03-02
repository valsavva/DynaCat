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
        public double SignalSpeed;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			p.LevelEngine.Tower = this;
		}
    }
}
