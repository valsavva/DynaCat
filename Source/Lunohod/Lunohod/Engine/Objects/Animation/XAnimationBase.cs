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
	public abstract class XAnimationBase : XObject
	{
        protected TimeSpan elapsedTime;
		protected bool isActive = true;
		protected bool isPaused = false;

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
        }
		[XmlAttribute]
        public bool IsActive
		{
			get { return this.isActive; }
			set { this.isActive = value; }
		}
		public bool IsPaused
		{
			get { return this.isPaused; }
		}
		
        public XAnimationBase()
		{
		}

		[XmlIgnore]
		public TimeSpan Duration;
		[XmlAttribute]
		public bool Autoreverse;
        [XmlIgnore]
        public TimeSpan RepeatTime;
        [XmlAttribute]
        public float RepeatCount;
		[XmlAttribute]
		public XAnimationFillBehavior FillBehavior;
		
		[XmlAttribute]
		public string Target;

		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.Duration.ToString(); }
			set {
                if (value.Contains(":"))
                    this.Duration = TimeSpan.Parse(value, CultureInfo.InvariantCulture);
                else
                    this.Duration = TimeSpan.FromSeconds(double.Parse(value, CultureInfo.InvariantCulture));
            }
		}

        [XmlAttribute("RepeatTime")]
        public string zRepeatTime
        {
            get { return this.RepeatTime.ToString(); }
            set { this.RepeatTime = TimeSpan.Parse(value, CultureInfo.InvariantCulture); }
        }

        public override void Update(UpdateParameters p)
        {
            base.Update(p);
			
			if (!this.isActive || this.isPaused)
				return;

			UpdateTime(p);

            UpdateAnimation();
        }

		void UpdateTime(UpdateParameters p)
		{
			this.elapsedTime += p.GameTime.ElapsedGameTime;
        	
    		if (this.RepeatTime != TimeSpan.Zero)
			{
				if (this.elapsedTime > this.RepeatTime)
					  Stop();
			}
			else if (this.RepeatCount != 0)
			{
				if ((this.elapsedTime.TotalMilliseconds > 0) && ((this.elapsedTime.TotalMilliseconds / this.Duration.TotalMilliseconds) >= this.RepeatCount))
					Stop();
			} 
		}
		
		//protected abstract void Reset();
        public abstract void UpdateAnimation();
		
		public void Start()
		{
			this.elapsedTime = TimeSpan.Zero;
			this.isActive = true;
			this.isPaused = false;
		}
		public void Pause()
		{
			this.isPaused = true;
		}
		public void Resume()
		{
			if (!this.isActive)
				Start();
			else
				this.isPaused = false;
		}
        public void Stop()
        {
			if (this.FillBehavior == XAnimationFillBehavior.Reset)
				this.elapsedTime = TimeSpan.Zero;
			else
				this.elapsedTime = this.Duration;
			
			this.isActive = false;
			this.isPaused = false;
        }
    }
}

