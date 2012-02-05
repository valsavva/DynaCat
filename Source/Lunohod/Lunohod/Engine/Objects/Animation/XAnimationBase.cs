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
	public abstract class XAnimationBase : XRunnableBase
	{
        public XAnimationBase()
		{
		}

		[XmlIgnore]
		public TimeSpan Duration;
		[XmlAttribute]
		public bool Autoreverse;
		[XmlAttribute]
		public XAnimationFillBehavior Fill;

		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.Duration.ToString(); }
            set { this.Duration = value.ToDuration(); }
		}
		
		public override int CalculateRepeatsDone()
		{
			if (this.elapsedTime == TimeSpan.Zero || this.Duration == TimeSpan.Zero)
				return 0;
			
			if (this.Autoreverse)
				return (int)(this.elapsedTime.TotalMilliseconds / (this.Duration.TotalMilliseconds * 2.0f));
			else
				return (int)(this.elapsedTime.TotalMilliseconds / this.Duration.TotalMilliseconds);
		}
		
		public override void UpdateProgress(UpdateParameters p)
		{
			UpdateAnimation();
			
			this.UpdateChildren(p);
		}
		
		protected abstract void UpdateAnimation();
		
        public override void Stop()
        {
			base.Stop();
			
			if (this.inProgress)
			{
				if (this.Fill == XAnimationFillBehavior.Reset)
					this.elapsedTime = TimeSpan.Zero;
				else if (this.Fill == XAnimationFillBehavior.End)
					this.elapsedTime = this.Duration + (this.Autoreverse ? this.Duration : TimeSpan.Zero);

				UpdateAnimation();
			}
        }
    }
}
