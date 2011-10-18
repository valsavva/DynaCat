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

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
        }
        
        public XAnimationBase()
		{
		}

		[XmlIgnore]
		public TimeSpan Duration;
		[XmlAttribute]
		public bool Autoreverse;
		[XmlAttribute]
		public string TargetId;
		[XmlAttribute]
		public string TargetProperty;
        [XmlIgnore]
        public TimeSpan RepeatTime;
        [XmlAttribute]
        public float RepeatCount;
		
		[XmlAttribute]
		public string Target
		{
			get {
				if (TargetId == null || TargetProperty == null)
					return null;
				return TargetId + "." + TargetProperty;
			}
			set {
				var parts = value.Split('.');
				this.TargetId = parts[0];
				this.TargetProperty = parts[1];
			}
		}

		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.Duration.ToString(); }
			set { this.Duration = TimeSpan.Parse(value, CultureInfo.InvariantCulture); }
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

            if (this.RepeatTime != TimeSpan.Zero)
            {
                if (this.elapsedTime > this.RepeatTime)
                {
                    EndAnimation();
                    return;
                }
            }
            else if (this.RepeatCount != 0)
            {
                if ((this.elapsedTime.TotalMilliseconds > 0) && ((this.elapsedTime.TotalMilliseconds / this.Duration.TotalMilliseconds) > this.RepeatCount))
                {
                    EndAnimation();
                    return;
                }
            }

            this.elapsedTime += p.GameTime.ElapsedGameTime;

            UpdateAnimation(p);
        }

        private void EndAnimation()
        {
            
        }

        public abstract void UpdateAnimation(UpdateParameters p);
    }
}

