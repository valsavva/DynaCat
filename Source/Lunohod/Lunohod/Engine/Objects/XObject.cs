using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	abstract public class XObject
	{
		public XObject ()
		{
		}
		
		public virtual void Initialize(InitializeParameters p)
		{

		}
		
		public virtual void Update(UpdateParameters p)
		{

		}
	}
}

