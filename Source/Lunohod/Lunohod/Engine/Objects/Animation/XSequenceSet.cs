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
		
		private int repeatsDone = 0;
		
		public XSequenceSet()
		{
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
		}

		public override void Update(UpdateParameters p)
		{
			if (this.inProgress && !this.isPaused)
			{
				if (currentRunnable == null)
				{	
					currentRunnable = runnables[0];
					currentRunnable.Start();
				}
				else
				{
					if (!currentRunnable.InProgress)
					{
						// the current animation is finished - on to the next one
						var curIndex = runnables.IndexOf(currentRunnable);
						if (curIndex < 0)
							throw new InvalidOperationException("WFT?");
						
						curIndex++;
						
						if (curIndex >= runnables.Count)
						{	
							repeatsDone++;
							
							if (repeatsDone >= this.RepeatCount)
							{
								this.Stop();
								return;
							}
	
							curIndex -= runnables.Count;
						}
						
						
						currentRunnable = runnables[curIndex];
						currentRunnable.Start();
					}
				}
			}
			
			base.Update(p);
		}
		
		public override void Start()
		{
			base.Start();
			
			repeatsDone = 0;
			runnables.ForEach(a => a.Stop());

			currentRunnable = null;
		}
		public override void Pause()
		{
			base.Pause();

			if (currentRunnable == null)
				return;
			
			currentRunnable.Pause();
		}
		public override void Resume()
		{
			base.Resume();

			if (currentRunnable == null)
				return;
			
			currentRunnable.Resume();
		}
		public override void Stop()
		{
			base.Stop();

			if (currentRunnable == null)
				return;
			
			runnables.ForEach(a => a.Stop());
		}
	}
}

