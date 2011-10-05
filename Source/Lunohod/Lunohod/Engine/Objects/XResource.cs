using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	public abstract class XResource : XComponent
	{
		public XResource()
		{
		}

		[XmlAttribute]
		public string Id {
			get;
			set;
		}
	}
}

