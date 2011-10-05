using System;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	abstract public class XObject : IDisposable
	{
		protected bool isDisposed = false;
		
		public XObject ()
		{
		}
		
		public virtual void Initialize(InitializeParameters p)
		{

		}
		
		public virtual void Update(UpdateParameters p)
		{

		}

		#region IDisposable implementation
		public virtual void Dispose()
		{
			isDisposed = true;
		}
		#endregion
	}
}

