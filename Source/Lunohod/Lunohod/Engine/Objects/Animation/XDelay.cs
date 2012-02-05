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
    [XmlType("Delay")]
	public class XDelay : XObject, IRunnable
	{
		private TimeSpan elapsedTime;
		private bool inProgress;
		private bool isPaused;
		
		[XmlIgnore]
		public TimeSpan Duration;

		[XmlAttribute("Duration")]
		public string zDuration
		{
			get { return this.Duration.ToString(); }
            set { this.Duration = value.ToDuration(); }
		}
		
		public override void Update(UpdateParameters p)
		{
			if (!this.inProgress || this.isPaused)
				return;

			this.elapsedTime += p.GameTime.ElapsedGameTime;
			
			if (this.elapsedTime > this.Duration)
				this.Stop();
			
			base.Update(p);
		}
		
		public void Start()
		{
			this.inProgress = true;
			this.isPaused = false;

			this.elapsedTime = TimeSpan.Zero;
		}

		public void Stop()
		{
			this.inProgress = false;
			this.isPaused = false;
		}

		public void Pause()
		{
			this.isPaused = true;
		}

		public void Resume()
		{
			if (!this.inProgress)
				this.Start();
			else
				this.isPaused = false;
		}

		public bool InProgress
		{
			get { return this.inProgress;}
			set { this.inProgress = value; }
		}
	}
}

