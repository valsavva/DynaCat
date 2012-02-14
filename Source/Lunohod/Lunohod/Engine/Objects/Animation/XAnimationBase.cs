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

		private string durationStr;
		
		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.durationStr ?? this.Duration.ToString(); }
            set { 
				try
				{
					this.Duration = value.ToDuration();
					this.durationStr = null;
				} catch {
					this.durationStr = value;
				}
			}
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
			if (this.inProgress)
			{
				if (this.Fill == XAnimationFillBehavior.Reset)
					this.elapsedTime = TimeSpan.Zero;
				else if (this.Fill == XAnimationFillBehavior.End)
					this.elapsedTime = this.Duration + (this.Autoreverse ? this.Duration : TimeSpan.Zero);

				UpdateAnimation();
			}

			base.Stop();
        }
		
		public override void ReplaceParameter(string par, string val)
		{
			if (this.durationStr != null)
				this.zDuration = this.durationStr.Replace(par, val);
			    
			base.ReplaceParameter(par, val);
		}
    }
}
