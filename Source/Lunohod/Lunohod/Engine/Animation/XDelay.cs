using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    /// <summary>
    /// A "runnable" component, that idles for the given period of time.
    /// <XGAME />
    /// </summary>
    [XmlType("Delay")]
	public class XDelay : XObject, IRunnable
	{
		private TimeSpan elapsedTime;
		private bool inProgress;
		private bool isPaused;
		
		private IExpression<float> durationReader;
		
        /// <summary>
        /// Specifies the amount of time the componente will idle.
        /// </summary>
		[XmlAttribute]
		public string Duration;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			durationReader = Compiler.CompileExpression<float>(this, this.Duration);
		}
		
        public override void Update(UpdateParameters p)
		{
			if (!this.inProgress || this.isPaused)
				return;

			this.elapsedTime += p.GameTime.ElapsedGameTime;
			
			if (this.elapsedTime.TotalSeconds > this.durationReader.GetValue())
				this.Stop();
			
			base.Update(p);
		}
		
        /// <inheritdoc />
		public void Start()
		{
			this.inProgress = true;
			this.isPaused = false;

			this.elapsedTime = TimeSpan.Zero;
		}

        /// <inheritdoc />
        public void Stop()
		{
			this.inProgress = false;
			this.isPaused = false;
		}

        /// <inheritdoc />
        public void Pause()
		{
			this.isPaused = true;
		}

        /// <inheritdoc />
        public void Resume()
		{
			if (!this.inProgress)
				this.Start();
			else
				this.isPaused = false;
		}

        /// <inheritdoc />
        public bool InProgress
		{
			get { return this.inProgress;}
			set { this.inProgress = value; }
		}

        /// <inheritdoc />
        public bool IsPaused
        {
            get { return this.isPaused; }
        }
	}
}

