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
    /// A helper base class for components that expose the <see cref="IRunnable"/> interface.
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

        [XmlIgnore]
        public TimeSpan RepeatTime { get; set; }
        [XmlAttribute]
        public float RepeatCount { get; set; }

        [XmlAttribute("RepeatTime")]
        public string zRunTime
        {
            get { return this.RepeatTime.ToString(); }
            set { this.RepeatTime = value.ToDuration(); }
        }

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

        /// <summary>
        /// Puts the component into the running state. This results in the <see cref="InProgress"/> property to be set to true.
        /// </summary>
        public virtual void Start()
		{
			this.elapsedTime = TimeSpan.Zero;
			this.inProgress = true;
			this.isPaused = false;
			this.repeatsDone = 0;
		}
        /// <summary>
        /// Pauses the component. This results in the <see cref="IsPaused"/> property to be set to true.
        /// </summary>
        public virtual void Pause()
		{
			if (this.inProgress)
				this.isPaused = true;
		}
        /// <summary>
        /// Resumes the component. Calling this method may result in three different behaviors:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// If the component is currently in the running state, it will continue to run.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the component is currently paused, it will resume running.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// If the component is currently stopped, it will be put into the running state.
        /// </description>
        /// </item>
        /// </list>
        /// </summary>
        public virtual void Resume()
		{
			if (!this.inProgress)
				Start();
			else			
				this.isPaused = false;
		}
        /// <summary>
        /// Stops the component. This results in the <see cref="InProgress"/> property to be set to false.
        /// </summary>
        public virtual void Stop()
        {
			this.inProgress = false;
			this.isPaused = false;
        }
	}
}

