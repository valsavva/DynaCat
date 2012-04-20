using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    [XmlType("LevelScore")]
	public class XLevelScore : XObject
	{
		[XmlAttribute]
		public double AvaliablePoints;
		[XmlAttribute]
		public double Score;
		[XmlAttribute]
		public double Time;
		[XmlAttribute]
		public double Health;
	}
}

