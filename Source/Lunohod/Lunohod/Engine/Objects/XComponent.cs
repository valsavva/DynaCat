using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	abstract public class XComponent : IDisposable
	{
		protected bool isDisposed = false;
		
		[XmlAttribute]
		public string Id { get; set;}

		[XmlIgnore]
		public XComponent Parent {get; set;}
		
		public virtual List<XComponent> Subcomponents {get; set;}
		
		public void InitHierarchy()
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Parent = this;
					subcomponent.InitHierarchy();
				}
		}
		
		public virtual void Initialize(InitializeParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Initialize(p);
				}
		}
		
		public virtual void Update(UpdateParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Update(p);
				}
		}
		
		public virtual void Draw(DrawParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Draw(p);
				}
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
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Dispose();
				}
		}
		#endregion
	}
}

