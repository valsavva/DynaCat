using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	public abstract class XResource : IDisposable
	{
		public XResource()
		{
		}

		[XmlAttribute]
		public string Id {
			get;
			set;
		}
		
		public virtual void Initialize(InitializeParameters p, XResourceBundle r)
		{
		}
		
		public virtual void Dispose()
		{
		}
	}
}

