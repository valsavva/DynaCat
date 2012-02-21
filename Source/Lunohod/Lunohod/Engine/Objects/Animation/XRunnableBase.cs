using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
    /// <summary>
    /// A base class for "runnable" components that utilize Duration, RepeatCount and RepeatTime.
    /// </summary>
	public abstract class XRunnableBase : XObject, IRunnable
	{
        protected TimeSpan elapsedTime;
		protected bool inProgress = false;
		protected bool isPaused = false;
		protected int repeatsDone = 0;

        /// <summary>
        /// Gets the amount of time elapsed since the component was started.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
        }
        /// <summary>
        /// Gets the number of times component executed since it was started.
        /// </summary>
		public int RepeatsDone
		{
			get { return repeatsDone; }
		}
        /// <summary>
        /// Specifies wheter the components is in the running state.
        /// This attribute value is false when the component has never been run or the <see cref="Stop()"/> method was called.
        /// </summary>
        [XmlAttribute]
        public bool InProgress
		{
			get { return this.inProgress; }
			set { this.inProgress = value; }
		}
        /// <summary>
        /// Gets value identifying whether the current component is in the paused state.
        /// </summary>
        public bool IsPaused
		{
			get { return this.isPaused; }
		}
        /// <summary>
        /// Specifies the amount of time the current animation should be executing. When not specified or zero,
        /// the animation is considered to be infinite. Cannot be specified along with <see cref="RepeatCount"/>.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RepeatTime { get; set; }
        /// <summary>
        /// Specifies the number of times the current animation should execute. When not specified or zero,
        /// the animation is considered to be infinite. Cannot be specified along with <see cref="RepeatTime"/>.
        /// </summary>
        [XmlAttribute]
        public float RepeatCount { get; set; }
        /// <explude />
        [XmlAttribute("RepeatTime")]
        public string zRunTime
        {
            get { return this.RepeatTime.ToString(); }
            set { this.RepeatTime = value.ToDuration(); }
        }
        /// <inheritdoc />
        public override void Update(UpdateParameters p)
        {
			if (!this.inProgress || this.isPaused)
				return;
		
			UpdateTime(p);
			
			if (!this.inProgress || this.isPaused)
				return;
		
			UpdateProgress(p);
        }

        internal void UpdateChildren(UpdateParameters p)
		{
			base.Update(p);
		}

        internal virtual int CalculateRepeatsDone()
		{
			return this.repeatsDone;
		}

		internal abstract void UpdateProgress(UpdateParameters p);
		
		internal virtual void UpdateTime(UpdateParameters p)
		{
			this.elapsedTime += p.GameTime.ElapsedGameTime;
        	
    		if (this.RepeatTime != TimeSpan.Zero)
			{
				if (this.elapsedTime > this.RepeatTime)
					  Stop();
			}
			else if (this.RepeatCount != 0)
			{
				repeatsDone = CalculateRepeatsDone();

				if (repeatsDone >= this.RepeatCount)
					Stop();
			} 
		}

        /// <inheritdoc />
        public virtual void Start()
		{
			this.elapsedTime = TimeSpan.Zero;
			this.inProgress = true;
			this.isPaused = false;
			this.repeatsDone = 0;
		}
        /// <inheritdoc />
        public virtual void Pause()
		{
			if (this.inProgress)
				this.isPaused = true;
		}
        /// <inheritdoc />
        public virtual void Resume()
		{
			if (!this.inProgress)
				Start();
			else			
				this.isPaused = false;
		}
        /// <inheritdoc />
        public virtual void Stop()
        {
			this.inProgress = false;
			this.isPaused = false;
        }
	}
}
