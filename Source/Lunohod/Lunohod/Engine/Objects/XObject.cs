using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
	abstract public class XObject : IDisposable
	{
		protected bool isDisposed;
		private Dictionary<string, SignalContainer> signalContainers;
		private XObjectCollection subcomponents;
		private Dictionary<string, XObject> componentDict;
		protected int updateCycle;

        [XmlAttribute]
        public string Id;

        [XmlAttribute]
        public string Class;

        [XmlAttribute]
        public string ClassParams;

        [XmlIgnore]
        public XObject Parent;

        [XmlAttribute]
        public bool Enabled = true;
		
		public virtual XObject Copy()
		{
			var result = (XObject)this.MemberwiseClone();
			result.Subcomponents = new XObjectCollection(this.Subcomponents.Count);
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
		
		// LevelSettings
        [XmlElement(ElementName = "LevelSettings", Type = typeof(XLevelSettings))]
		
        // Layer
        [XmlElement(ElementName = "Layer", Type = typeof(XLayer))]
		
		// Triggers
        [XmlElement(ElementName = "IntersectionTrigger", Type = typeof(XIntersectionTrigger))]
        [XmlElement(ElementName = "DistanceTrigger", Type = typeof(XDistanceTrigger))]
		[XmlElement(ElementName = "BoolTrigger", Type = typeof(XBoolTrigger))]
		[XmlElement(ElementName = "NumTrigger", Type = typeof(XNumTrigger))]
		
		// Classes
        [XmlElement(ElementName = "Class", Type = typeof(XClass))]

		// Characters
        [XmlElement(ElementName = "Tower", Type = typeof(XTower))]
        [XmlElement(ElementName = "Hero", Type = typeof(XHero))]
        [XmlElement(ElementName = "Enemy", Type = typeof(XEnemy))]
        [XmlElement(ElementName = "Food", Type = typeof(XFood))]
        [XmlElement(ElementName = "Explosion", Type = typeof(XExplosion))]

		// Iterator
        [XmlElement(ElementName = "Iterator", Type = typeof(XIterator))]

		// Sets
        [XmlElement(ElementName = "SequenceSet", Type = typeof(XSequenceSet))]
        [XmlElement(ElementName = "RandomSet", Type = typeof(XRandomSet))]
        [XmlElement(ElementName = "ParallelSet", Type = typeof(XParallelSet))]

        // Actions
        [XmlElement(ElementName = "Do", Type = typeof(XDo))]
        [XmlElement(ElementName = "Delay", Type = typeof(XDelay))]

        // Animation
        [XmlElement(ElementName = "NumAnimation", Type = typeof(XNumAnimation))]
        [XmlElement(ElementName = "KeyFrame", Type = typeof(XKeyFrame))]

		// Basic elements
		[XmlElement(ElementName = "Group", Type = typeof(XGroup))]
        [XmlElement(ElementName = "Image", Type = typeof(XImage))]
        [XmlElement(ElementName = "Text", Type = typeof(XText))]
        [XmlElement(ElementName = "Block", Type = typeof(XBlock))]
        [XmlElement(ElementName = "Sprite", Type = typeof(XSprite))]
        public XObjectCollection Subcomponents
		{
			get { return subcomponents; }
			set {
				if (subcomponents == value)
					return;
				
				if (subcomponents != null)
					subcomponents.Parent = null;
				
				subcomponents = value;
				
				if (subcomponents != null)
					subcomponents.Parent = this;
			}
		}
		
		public virtual void InitHierarchy()
		{
			if (this.Subcomponents == null)
				return;
			
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
			{
				if (this.Subcomponents != null)
					for(int i = 0; i < this.Subcomponents.Count; i++)
					{
						var subcomponent = this.Subcomponents[i];
						subcomponent.InitHierarchy();
					}
			}
			
			// force to rebiuld subcomponent hash
			this.componentDict = null;
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

		public XObject InitiazeFromClass()
		{
			XClass cls = (XClass)this.GetRoot().FindDescendant(this.Class);
			
			if (cls == null)
				throw new InvalidOperationException("Could not find class: " + this.Class);
			
			var instance = cls.CreateInstance(this);
			instance.InitHierarchy();
			
			return instance;
		}
		
		public virtual void Update(UpdateParameters p)
		{
			this.updateCycle = p.Game.CycleNumber;
			
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
                    if (subcomponent.Enabled)
    					subcomponent.Update(p);
				}

			if (signalContainers != null)
			{
				foreach(var item in signalContainers)
					item.Value.Clear();
			}
		}
		
		public virtual void Draw(DrawParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
                    if (subcomponent.Enabled)
                        subcomponent.Draw(p);
				}
		}
		
        public void Enable()
        {
            this.Enabled = true;
        }

        public void Disable()
        {
            this.Enabled = false;
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
		
		public XObject FindDescendant(string id)
		{
			if (this.componentDict == null)
			{
				this.componentDict = new Dictionary<string, XObject>();
				this.AddSubtreeToComponentDict(this);
			}
			
			XObject result = null;
			
			this.componentDict.TryGetValue(id, out result);
			
			return result;
		}

		public void AddSubtreeToComponentDict(XObject subtreeRoot)
		{
			if (this.componentDict == null)
				return;
			
			subtreeRoot.TraveseTree(o => {
						if (!string.IsNullOrEmpty(o.Id))
						{
							if (!this.componentDict.ContainsKey(o.Id))
								this.componentDict.Add(o.Id, o);
			#if DEBUG
							else
								Console.WriteLine("*** Component with id '{0}' already exists! ***", o.Id);
			#endif
						}
					});
		}
		
		public void TraveseTree(Action<XObject> action)
		{
			action(this);

			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
					this.Subcomponents[i].TraveseTree(action);
		}
		
		public void TraveseAncestors(Action<XObject> action)
		{
			XObject ancestor = this.Parent;
			
			while(ancestor != null)
			{
				action(ancestor);
				ancestor = ancestor.Parent;
			}
		}
		
		public XObject FindAncestor(Predicate<XObject> p)
		{
			XObject result = this.Parent;
			
			while((result != null) && !p(result))
			{
				result = result.Parent;
			}
			
			return result;
		}
		
		public XObject GetRoot()
		{
			XObject result = this;
	
			while (result.Parent != null)
				result = result.Parent;

			return result;
		}
		
		private static char[] delimiters = new char[] {'.', ':'};
		
		public void GetTargetFromDescriptor(string descriptor, out XObject target, out string targetMember)
		{
			if (descriptor.StartsWith(".") || descriptor.StartsWith(":"))
			    descriptor = descriptor.Substring(1);
			
			if (descriptor.Contains(".") || descriptor.Contains(":"))
			{
				int index = descriptor.IndexOfAny(delimiters);
				
				var targetId = descriptor.Substring(0, index);
				
				target = this.GetRoot().FindDescendant(targetId);
				
				if (target == null)
					throw new InvalidOperationException(string.Format("Could not find object with id [{0}]", targetId));
				
				targetMember = descriptor.Substring(index + 1);
			}
			else
			{
				target = this.Parent;
				targetMember = descriptor;
			}
		}
		
		public SignalContainer GetSignalContainer(string containerName)
		{
			if (signalContainers == null)
				signalContainers = new Dictionary<string, SignalContainer>();
			
			SignalContainer container = null;
			
			if (!signalContainers.TryGetValue(containerName, out container))
			{
				container = new SignalContainer();
				signalContainers.Add(containerName, container);
			}
			
			return container;
		}

        public List<IRunnable> CollectRunnables(List<IRunnable> result = null)
        {
            if (result == null)
                result = new List<IRunnable>();

            if (this.Subcomponents != null)
                for (int i = 0; i < this.Subcomponents.Count; i++)
                {
                    var subcomponent = this.Subcomponents[i];
                    IRunnable runnable = subcomponent as IRunnable;
                    if (runnable != null)
                        result.Add(runnable);

                    // we don't collect other sets' runnables
                    if (!(subcomponent is XSetBase))
                        subcomponent.CollectRunnables(result);
                }

            return result;
        }
        //public IEnumerable<T> TraverseChildren<T>() where T : class
        //{
        //    for (int i = 0; i < this.Subcomponents.Count; i++)
        //    {
        //        if (this.Subcomponents[i] is T)
        //            yield return (this.Subcomponents[i] as T);
        //    }
        //}

		internal virtual void ReplaceParameter(string par, string val)
		{
			if (this.Id != null)
				this.Id = this.Id.Replace(par, val);
		}
		
		internal void ReplaceParameters(List<string> pars, List<string> vals)
		{
			for (int i = 0; i < pars.Count; i++)
				this.ReplaceParameter(pars[i], vals[i]);

			if (this.subcomponents != null)
				for(int i = 0; i < this.subcomponents.Count; i++)
				{
					subcomponents[i].ReplaceParameters(pars, vals);
				}
		}
		
		public override string ToString()
		{
			return string.Format("[{0}: Id={1}]", this.GetType().Name, Id);
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

