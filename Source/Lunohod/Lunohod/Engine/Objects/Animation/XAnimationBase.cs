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
    /// Represents a base class for all animations.
    /// </summary>
	public abstract class XAnimationBase : XRunnableBase
	{
        public XAnimationBase()
		{
		}

        /// <summary>
        /// Specifies the animation duration. This is the amount of time between the start of the first keyframe and
        /// the end of the last one.
        /// </summary>
		[XmlIgnore]
		public TimeSpan Duration;
        /// <summary>
        /// Specifies whether the animation should automatically re-play in reverse upon completion of the last keyframe.
        /// When set to True, it essentially doubles the duration of one animation repeat.
        /// </summary>
		[XmlAttribute]
		public bool Autoreverse;
        /// <summary>
        /// Specifies the behavior of the animation upon its completion. See <see cref="XAnimationFillBehavior"/> for details.
        /// </summary>
		[XmlAttribute]
		public XAnimationFillBehavior Fill;

		private string durationStr;
		
        /// <exclude />
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

        /// <exclude />
        internal override float CalculateRepeatsDone()
		{
			if (this.elapsedTime == TimeSpan.Zero || this.Duration == TimeSpan.Zero)
				return 0f;
			
			if (this.Autoreverse)
				return (float)(this.elapsedTime.TotalMilliseconds / (this.Duration.TotalMilliseconds * 2.0));
			else
				return (float)(this.elapsedTime.TotalMilliseconds / this.Duration.TotalMilliseconds);
		}

        /// <exclude />
        internal override void UpdateProgress(UpdateParameters p)
		{
			UpdateAnimation();
			
			this.UpdateChildren(p);
		}

        /// <exclude />
        protected abstract void UpdateAnimation();
		
        /// <inheritdoc />
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

        /// <exclude />
        internal override void ReplaceParameter(string par, string val)
		{
			if (this.durationStr != null)
				this.zDuration = this.durationStr.Replace(par, val);
			    
			base.ReplaceParameter(par, val);
		}
    }
}
