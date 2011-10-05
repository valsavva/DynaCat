using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	abstract public class XComponent : IDisposable
	{
		protected bool isDisposed = false;
		
		public XComponent ()
		{
		}
		
		[XmlIgnore]
		public XComponent Parent {get; private set;}
		
		public virtual XComponent[] Subcomponents {get; set;}
		
		public void InitHierarchy()
		{
			if (this.Subcomponents != null)
				foreach(var subcomponent in this.Subcomponents)
				{
					subcomponent.Parent = this;
					subcomponent.InitHierarchy();
				}
		}
		
		public virtual void Initialize(InitializeParameters p)
		{
			if (this.Subcomponents != null)
				foreach(var subcomponent in this.Subcomponents)
					subcomponent.Initialize(p);
		}
		
		public virtual void Update(UpdateParameters p)
		{
			if (this.Subcomponents != null)
				foreach(var subcomponent in this.Subcomponents)
					subcomponent.Update(p);
		}
		
		public virtual void Draw(DrawParameters p)
		{
			if (this.Subcomponents != null)
				foreach(var child in this.Subcomponents)
					child.Draw(p);
		}
		
		public T GetComponent<T>() where T : XComponent
		{
			if (this.Subcomponents == null)
				return null;
			
			return (T)this.Subcomponents.FirstOrDefault(c => c.GetType() == typeof(T));
		}
		
		public IEnumerable<T> GetComponents<T>() where T : XComponent
		{
			if (this.Subcomponents == null)
				return null;
			
			return this.Subcomponents.Where(c => c.GetType() == typeof(T)).Cast<T>();
		}
		
		
		#region IDisposable implementation
		public virtual void Dispose()
		{
			isDisposed = true;
			
			if (this.Subcomponents != null)
				foreach(var subcomponent in this.Subcomponents)
					subcomponent.Dispose();
		}
		#endregion
	}
}

