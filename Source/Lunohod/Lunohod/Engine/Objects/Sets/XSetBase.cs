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
    public abstract class XSetBase : XRunnableBase
    {
		protected List<IRunnable> runnables;

		public XSetBase()
        {
			this.RepeatCount = 1;
        }

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			runnables = this.CollectRunnables();
			runnables.ForEach(r => r.InProgress = false);

			if (this.inProgress)
				this.Start();
		}
		
		public override int CalculateRepeatsDone()
		{
			return this.repeatsDone;
		}
	}
}
