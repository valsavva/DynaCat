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
    [XmlType("SequenceSet")]
	public class XSequenceSet : XSetBase
	{
		private IRunnable currentRunnable;
		private int currentIndex = 0;
		
        internal override void UpdateProgress(UpdateParameters p)
		{
			if (currentRunnable == null)
			{	
				currentRunnable = runnables[currentIndex];
				currentRunnable.Start();
			}
			
			currentRunnable.Update(p);
			
			if (!currentRunnable.InProgress)
			{
				currentRunnable = null;
				currentIndex++;
				
				if (currentIndex >= runnables.Count)
				{	
					repeatsDone++;
					currentIndex -= runnables.Count;
				}
			}
		}
		
		public override void Start()
		{
			base.Start();
			
			if (currentRunnable != null)
				currentRunnable.Stop();

			currentRunnable = null;
			currentIndex = 0;
		}
		public override void Pause()
		{
			base.Pause();

			if (currentRunnable != null)
				currentRunnable.Pause();
		}
		public override void Resume()
		{
			base.Resume();

			if (currentRunnable != null)
				currentRunnable.Resume();
		}
		public override void Stop()
		{
			base.Stop();

			if (currentRunnable != null)
	            currentRunnable.Stop();
		}
	}
}

