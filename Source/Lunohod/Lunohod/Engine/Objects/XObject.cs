using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	abstract public class XObject : IDisposable
	{
		protected bool isDisposed = false;
		private SignalContainer signalContainer = null;
		
		[XmlAttribute]
		public string Id { get; set;}
		
		[XmlAttribute]
		public string Class { get; set; }

		[XmlIgnore]
		public XObject Parent { get; set; }
		
		public virtual XObject Copy()
		{
			var result = (XObject)this.MemberwiseClone();
			result.Subcomponents = new List<XObject>(this.Subcomponents.Count);
			for(int i = 0; i < this.Subcomponents.Count; i++)
			{
				result.Subcomponents.Add(this.Subcomponents[i].Copy());
			}
			result.InitHierarchy();
			
			return result;
		}
		
		// Game
        [XmlElement(ElementName = "Resources", Type = typeof(XResourceBundle))]
        [XmlElement(ElementName = "Dashboard", Type = typeof(XDashboard))]
        [XmlElement(ElementName = "TapArea", Type = typeof(XTapArea))]
        [XmlElement(ElementName = "Include", Type = typeof(XInclude))]

        // Media
        [XmlElement(ElementName = "Music", Type = typeof(XMusic))]
        [XmlElement(ElementName = "Sound", Type = typeof(XSound))]
		
        // Dashboard
        [XmlElement(ElementName = "Viewport", Type = typeof(XViewport))]

        // Level
        [XmlElement(ElementName = "Layer", Type = typeof(XLayer))]
		
		// Triggers
		[XmlElement(ElementName = "DistanceTrigger", Type = typeof(XDistanceTrigger))]
		[XmlElement(ElementName = "BoolTrigger", Type = typeof(XBoolTrigger))]
		[XmlElement(ElementName = "NumTrigger", Type = typeof(XNumTrigger))]
		
		// Classes and states
        [XmlElement(ElementName = "Class", Type = typeof(XClass))]
        [XmlElement(ElementName = "State", Type = typeof(XState))]

		// Characters
        [XmlElement(ElementName = "Tower", Type = typeof(XTower))]
        [XmlElement(ElementName = "Hero", Type = typeof(XHero))]
        [XmlElement(ElementName = "Enemy", Type = typeof(XEnemy))]
		
		// Basic elements
		[XmlElement(ElementName = "Group", Type = typeof(XGroup))]
        [XmlElement(ElementName = "Image", Type = typeof(XImage))]
        [XmlElement(ElementName = "Block", Type = typeof(XBlock))]
        [XmlElement(ElementName = "Sprite", Type = typeof(XSprite))]
        [XmlElement(ElementName = "Sequence", Type = typeof(XSequence))]
        [XmlElement(ElementName = "NumAnimation", Type = typeof(XNumAnimation))]
        public List<XObject> Subcomponents { get; set; }
		
		public virtual void InitHierarchy()
		{
			// replace includes with their children
			var index = this.Subcomponents.FindIndex(c => c is XInclude);
			
			while (index != -1)
			{
				var include = this.Subcomponents[index];
				this.Subcomponents.RemoveAt(index);
				this.Subcomponents.InsertRange(index, include.Subcomponents);
				
				index = this.Subcomponents.FindIndex(c => c is XInclude);
			}
			
			// classes
			if (!string.IsNullOrEmpty(this.Class))
			{
				InitiazeFromClass();
				return;
			}
			else
				// set the parent property to the current node
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

		private void InitiazeFromClass()
		{
			XClass cls = (XClass)this.GetRoot().FindDescendant(this.Class);
			
			if (cls == null)
				throw new InvalidOperationException("Could not find class: " + this.Class);
			
			var instance = cls.CreateInstance(this);
			instance.InitHierarchy();
		}
		
		public virtual void Update(UpdateParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Update(p);
				}

			if (signalContainer != null)
				signalContainer.Clear();
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
		
		public virtual void DrawDebug(DrawParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Draw(p);
				}
		}
		
		public T GetComponent<T>() where T : XObject
		{
			if (this.Subcomponents == null)
				return null;
			
			return (T)this.Subcomponents.FirstOrDefault(c => c is T);
		}
		
		public IEnumerable<T> GetComponents<T>() where T : XObject
		{
			if (this.Subcomponents == null)
				return null;
			
			return this.Subcomponents.Where(c => c is T).Cast<T>();
		}
		
		public List<XObject> GetAllDescendants()
		{
			List<XObject> all = new List<XObject>();
			this.GetAllDescendants(all);
			return all;
		}

		private void GetAllDescendants(List<XObject> all)
		{
			if (this.Subcomponents == null)
				return;
			
			all.AddRange(this.Subcomponents);
			
			for(int i = 0; i < this.Subcomponents.Count; i++)
				this.Subcomponents[i].GetAllDescendants(all);
		}
				
		public XObject FindDescendant(string id)
		{
			if (this.Id == id)
				return this;
			
			if (this.Subcomponents == null)
				return null;
			
			XObject result = null;

			for(int i = 0; i < this.Subcomponents.Count; i++)
			{
				if ((result = this.Subcomponents[i].FindDescendant(id)) != null)
					return result;
			}
			
			return null;
		}
		
		public XObject GetRoot()
		{
			if (this.Parent == null)
				return this;
			return this.Parent.GetRoot();
		}
		
		public void GetTargetFromDescriptor(string descriptor, out XObject target, out string targetMember)
		{
			if (descriptor.Contains(".") || descriptor.Contains(":"))
			{
				var parts = descriptor.Split('.', ':');

				target = this.GetRoot().FindDescendant(parts[0]);
				
				if (target == null)
					throw new InvalidOperationException(string.Format("Could not find object with id [{0}]", parts[0]));
				targetMember = parts[1];
			}
			else
			{
				target = this.Parent;
				targetMember = descriptor;
			}
		}
		
		public SignalContainer GetSignalContainer()
		{
			return signalContainer ?? (signalContainer = new SignalContainer());
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

