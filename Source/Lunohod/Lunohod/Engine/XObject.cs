using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Xml;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    /// <summary>
    /// The base class for all components in the XGAME framework.
    /// </summary>
	abstract public class XObject : IDisposable, IXmlSerializable
	{
		protected bool isDisposed;
		private Dictionary<string, int> triggerGroups;
		private XObjectCollection subcomponents;
		private Dictionary<string, XObject> componentDict;
		protected int updateCycle;
		

        /// <summary>
        /// Specifies the object id. The object ids must be unique across loaded module, such as a game level.
        /// </summary>
        [XmlAttribute]
        public string Id;
        /// <summary>
        /// Specifies the name of the class which defines the template of this object.
        /// </summary>
        [XmlAttribute]
        public string Class;
        /// <summary>
        /// Specifies the list of class parameters. The list is a comma-delimited list of key/value pairs, such as
        /// "@x=10,@y=20"
        /// </summary>
        [XmlAttribute]
        public string ClassParams;
        /// <summary>
        /// Specifies whether the current component is enabled. When disabled, the component does not get updated or drawn.
        /// </summary>
        [XmlAttribute]
        public bool Enabled = true;
        /// <summary>
        /// Gets parent component.
        /// </summary>
        [XmlIgnore]
        public XObject Parent;
		/// <summary>
		/// Gets a value indicating whether this <see cref="Lunohod.Objects.XObject"/> participated in the current or the last update cycle.
		/// If <c>false</c>, this indicates that the current component or one of its ancestors was disabled.
		/// </summary>
		/// <value>
		/// <c>true</c> if was updated; otherwise, <c>false</c>.
		/// </value>
		[XmlIgnore]
		public bool WasUpdated
		{
			get { return Math.Abs(this.updateCycle - GameEngine.Instance.CycleNumber) <= 1; }
		}
        /// <summary>
        /// Gets collection of subcomponents.
        /// </summary>
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

        #region InitHierarchy, Initialize, Update, Draw
        /// <summary>
		/// This method represents the first stage of the four primary stages of the component lifecycle
        /// (hierarchy building, component initialization, update and draw, dispose).
        /// <remarks>
        /// The <see cref="InitHierarchy"/> is called when the entire component graph just has been constructed,
        /// usually as the result of deserialization from an XML file.
        /// This method is used solely for the object hierarchy manipulation, such as substituting Include nodes
        /// or building class instances. This method is not intended for any kind of component initialization.
        /// </remarks>
		/// </summary>
		public virtual void InitHierarchy()
		{
            // classes
            if (!string.IsNullOrEmpty(this.Class))
            {
                InitiazeFromClass();
                return;
            }
            
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
			
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.InitHierarchy();
				}

            // force to rebiuld subcomponent hash
			this.componentDict = null;
		}
        /// <summary>
        /// This method represents the second stage of the four primary stages of the component lifecycle
        /// (hierarchy building, component initialization, update and draw, dispose).
        /// <remarks>
        /// The <see cref="Initialize"/> method is called when after the object hierarchy is built and allows
        /// for component-specific initialization of parameters. Examles include finding related objects in the hierarchy,
        /// setting default values, pre-calculation of certain values.
        /// </remarks>
        /// </summary>
        public virtual void Initialize(InitializeParameters p)
		{
			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
				{
					var subcomponent = this.Subcomponents[i];
					subcomponent.Initialize(p);
				}
		}
        /// <summary>
        /// This method represents the update stage of the four primary stages of the component lifecycle
        /// (hierarchy building, component initialization, update and draw, dispose).
        /// <remarks>
        /// The <see cref="Update"/> method is the main counterpart of the <see cref="Draw"/> method.
        /// While the <see cref="Draw"/> method
        /// is responsible for drawing components on the screen, the <see cref="Update"/> method is responsible
        /// for calculating where the things need to be drawn. Essentially, the <see cref="Update"/> takes
        /// the "heavy" part out of equasions for when things need to be rendered.
        /// </remarks>
        /// </summary>
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
		}
        /// <summary>
        /// This method represents the draw stage of the four primary stages of the component lifecycle
        /// (hierarchy building, component initialization, update and draw, dispose).
        /// <remarks>
        /// The <see cref="Draw"/> method is responsible for rendering the component on the screen. This is where
        /// the XGAME framework integrates with thatever the underlying graphics technolody is used.
        /// </remarks>
        /// </summary>
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
        /// <summary>
        /// Creates a new instance from the class specified in the <see cref="Class"/> property.
        /// </summary>
        /// <returns>New instance of the class.</returns>
        internal XObject InitiazeFromClass()
        {
            XClass cls = (XClass)this.GetRoot().FindDescendant(this.Class);

            if (cls == null)
                throw new InvalidOperationException("Could not find class: " + this.Class);

            var instance = cls.CreateInstance(this);
            instance.InitHierarchy();

            return instance;
        }
        /// <summary>
        /// Creates a semi-shallow copy of the current component.
        /// <remarks>
        /// The <see cref="System.Object.MemberwiseClone()"/> method is used to create a copy of the current component,
        /// then a new subcomponents collection is created and populated from the original one by calling
        /// the <see cref="Copy()"/> method on each one of the original subcomponents.
        /// Because a copy of the subcomponent collection gets created, hence the "semi-" prefix describing this method.
        /// </remarks>
        /// </summary>
        /// <returns>A semi-shallow copy of the current component.</returns>
        internal virtual XObject Copy()
        {
            var result = (XObject)this.MemberwiseClone();

            if (result.subcomponents != null)
            {
                result.Subcomponents = new XObjectCollection(this.Subcomponents.Count);
                for (int i = 0; i < this.Subcomponents.Count; i++)
                {
                    result.Subcomponents.Add(this.Subcomponents[i].Copy());
                }
            }

            return result;
        }
        internal void CreateSubcomponents()
        {
            if (this.subcomponents == null)
                this.subcomponents = new XObjectCollection() { Parent = this };
        }
        #endregion

        #region Enable/Disable
        /// <summary>
        /// Enables the component by setting the Enabled property to true.
        /// </summary>
        public void Enable()
        {
            this.Enabled = true;
        }
        /// <summary>
        /// Disables the component by setting the Enabled property to false.
        /// </summary>
        public void Disable()
        {
            this.Enabled = false;
        }
        #endregion

        #region Hierarchy manipulation
        internal T GetComponent<T>() where T : XObject
		{
			if (this.Subcomponents == null)
				return null;
			
			return (T)this.Subcomponents.FirstOrDefault(c => c is T);
		}
		internal IEnumerable<T> GetComponents<T>() where T : XObject
		{
			if (this.Subcomponents == null)
				return null;
			
			return this.Subcomponents.Where(c => c is T).Cast<T>();
		}
		internal XObject FindDescendant(string id)
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
		internal void AddSubtreeToComponentDict(XObject subtreeRoot)
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
		internal void TraveseTree(Action<XObject> action)
		{
			action(this);

			if (this.Subcomponents != null)
				for(int i = 0; i < this.Subcomponents.Count; i++)
					this.Subcomponents[i].TraveseTree(action);
		}
		internal void TraveseAncestors(Action<XObject> action)
		{
			XObject ancestor = this.Parent;
			
			while(ancestor != null)
			{
				action(ancestor);
				ancestor = ancestor.Parent;
			}
		}
		internal XObject FindAncestor(Predicate<XObject> p)
		{
			XObject result = this.Parent;
			
			while((result != null) && !p(result))
			{
				result = result.Parent;
			}
			
			return result;
		}
		internal XObject GetRoot()
		{
			XObject result = this;
	
			while (result.Parent != null)
				result = result.Parent;

			return result;
		}

        private static char[] delimiters = new char[] {'.', ':'};
		internal void GetTargetFromDescriptor(string descriptor, out XObject target, out string targetMember)
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
        #endregion


		public Dictionary<string, int> GetTriggerGroups()
		{
			if (this.triggerGroups == null)
				this.triggerGroups = new Dictionary<string, int>();
			
			return this.triggerGroups;
		}
		
		internal virtual void ReplaceParameter(string par, string val)
		{
			if (this.Id != null)
				this.Id = this.Id.Replace(par, val);
			
			if (this.ClassParams != null)
				this.ClassParams = this.ClassParams.Replace(par, val);
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
		
		internal void EnqueueEvent(string name, bool isInstant = true)
		{
			GameEngine.Instance.EnqueueEvent(
				new GameEvent(name, GameEngine.Instance.CurrentUpdateTime) { IsInstant = isInstant }
			);
		}
		internal void EnqueueEvent(GameEventType type, bool isInstant = true)
		{
			GameEngine.Instance.EnqueueEvent(
				new GameEvent(type, GameEngine.Instance.CurrentUpdateTime) { IsInstant = isInstant }
			);
		}
		
        /// <summary>
        /// Disposes of the current component and its subcomponents.
        /// When ovveriden, base.Dispose() must be called to ensure subcomponents' diposal.
        /// </summary>
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
    
        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format("[{0}: Id={1}]", this.GetType().Name, Id);
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            this.Id = reader["Id"];
            this.Class = reader["Class"];
            this.ClassParams = reader["ClassParams"];
            reader.ReadAttrAsBoolean("Enabled", ref this.Enabled);

            if (this is IExploding)
                ((IExploding)this).IsExploding = reader.ReadAttrAsBoolean("IsExploding");

            ReadSubcomponents(reader);
        }

        public void ReadSubcomponents(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return;

            this.Subcomponents = new XObjectCollection();

            while (true)
            {
                reader.Read();
                
                if (reader.MoveToContent() == XmlNodeType.EndElement)
                    return;

                XObject sub = ClassFactory.CreateObject(reader.LocalName);
                sub.ReadXml(reader);

                AddSubcomponent(reader.LocalName, sub);
            }
        }

        public virtual void AddSubcomponent(string name, XObject sub)
        {
            this.Subcomponents.Add(sub);
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            throw new NotImplementedException();
        }
		
		public virtual void GetMethod(string methodName, out Func<List<Expression>, double> method)
		{
			throw new InvalidOperationException("Unknown method '" + methodName + "'");
		}
		public virtual void GetMethod(string methodName, out Func<List<Expression>, bool> method)
		{
			throw new InvalidOperationException("Unknown method '" + methodName + "'");
		}
		public virtual void GetMethod(string methodName, out Func<List<Expression>, string> method)
		{
			throw new InvalidOperationException("Unknown method '" + methodName + "'");
		}
		public virtual void GetMethod(string methodName, out Action<List<Expression>> method)
		{
			method = null;
			
            switch (methodName)
            {
                case "Enable": method = (ps) => Enable(); break;
                case "Disable": method = (ps) => Disable(); break;
				default: {
					if (this is IRunnable)
					{
						IRunnable runnable = (IRunnable)this;
			            switch (methodName)
			            {
			                case "Start": method = (ps) => runnable.Start(); break;
			                case "Stop": method = (ps) => runnable.Stop(); break;
			                case "Pause": method = (ps) => runnable.Pause(); break;
			                case "Resume": method = (ps) => runnable.Resume(); break;
						}
					}
				}; break;
			}

			if (method == null)
				throw new InvalidOperationException("Unknown method '" + methodName + "'");
		}
		
		public virtual void GetProperty(string propertyName, out Func<double> getter, out Action<double> setter)
		{
			getter = null; setter = null;

			if (getter == null && setter == null && this is IHasVolume)
			{
				IHasVolume audio = (IHasVolume)this;
				switch (propertyName)
				{
	                case "Volume": getter = () => audio.Volume; setter = (v) => audio.Volume = v; break;
				}
			}

			throw new InvalidOperationException("Unknown property '" + propertyName + "'");
		}
		public virtual void GetProperty(string propertyName, out Func<bool> getter, out Action<bool> setter)
		{
			getter = null; setter = null;
			
			switch (propertyName)
			{
				case ("Enabled") : getter = () => this.Enabled; setter = (v) => this.Enabled = v; break;
			}
			
			if (getter == null && setter == null && this is IRunnable)
			{
				IRunnable runnable = (IRunnable)this;
				switch (propertyName)
				{
	                case "InProgress": getter = () => runnable.InProgress; setter = (v) => runnable.InProgress = v; break;
	                case "IsPaused": getter = () => runnable.IsPaused; setter = null; break;
				}
			}
			
			if (getter == null && setter == null && this is IExploding)
			{
				IExploding exploding = (IExploding)this;
				switch (propertyName)
				{
	                case "IsExploding": getter = () => exploding.IsExploding; setter = (v) => exploding.IsExploding = v; break;
				}
			}
			
			if (getter == null && setter == null)
				throw new InvalidOperationException("Unknown property '" + propertyName + "'");
		}
		public virtual void GetProperty(string propertyName, out Func<string> getter, out Action<string> setter)
		{
			throw new InvalidOperationException("Unknown property '" + propertyName + "'");
		}
	}
}

