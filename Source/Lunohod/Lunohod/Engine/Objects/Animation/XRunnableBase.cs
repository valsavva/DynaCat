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
	public abstract class XRunnableBase : XObject, IRunnable
	{
        protected TimeSpan elapsedTime;
		protected bool inProgress = false;
		protected bool isPaused = false;
		protected int repeatsDone = 0;

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
        }
		public int RepeatsDone
		{
			get { return repeatsDone; }
		}
		[XmlAttribute]
        public bool InProgress
		{
			get { return this.inProgress; }
			set { this.inProgress = value; }
		}
		public bool IsPaused
		{
			get { return this.isPaused; }
		}
		
        [XmlIgnore]
        public TimeSpan RepeatTime;
        [XmlAttribute]
        public float RepeatCount;

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
		
		public void UpdateChildren(UpdateParameters p)
		{
			base.Update(p);
		}
		
		public virtual int CalculateRepeatsDone()
		{
			return this.repeatsDone;
		}

		public abstract void UpdateProgress(UpdateParameters p);
		
		public virtual void UpdateTime(UpdateParameters p)
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
		
		public virtual void Start()
		{
			this.elapsedTime = TimeSpan.Zero;
			this.inProgress = true;
			this.isPaused = false;
			this.repeatsDone = 0;
		}
		public virtual void Pause()
		{
			if (this.inProgress)
				this.isPaused = true;
		}
		public virtual void Resume()
		{
			if (!this.inProgress)
				Start();
			else			
				this.isPaused = false;
		}
        public virtual void Stop()
        {
			this.inProgress = false;
			this.isPaused = false;
        }
	}
}

