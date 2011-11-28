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
    [XmlType("Sequence")]
	public class XSequence : XAnimationBase
	{
		private List<XAnimationBase> animations;
		private XAnimationBase currentAnimation;
		
		private int repeatsDone = 0;
		
		public XSequence()
		{
		}
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			animations = this.GetComponents<XAnimationBase>().ToList();
			animations.ForEach(a => a.InProgress = false );
		}

		public override void Update(UpdateParameters p)
		{
			if (this.inProgress && !this.isPaused)
			{
				if (currentAnimation == null)
				{	
					currentAnimation = animations[0];
					currentAnimation.Start();
				}
				else
				{
					if (!currentAnimation.InProgress)
					{
						// the current animation is finished - on to the next one
						var curIndex = animations.IndexOf(currentAnimation);
						if (curIndex < 0)
							throw new InvalidOperationException("WFT?");
						
						curIndex++;
						
						if (curIndex >= animations.Count)
						{	
							repeatsDone++;
							
							if (this.RepeatCount > 0 && repeatsDone == this.RepeatCount)
							{
								this.Stop();
								return;
							}
	
							curIndex -= animations.Count;
						}
						
						
						currentAnimation = animations[curIndex];
						currentAnimation.Start();
					}
				}
			}
			
			base.Update(p);
		}
		
		public override void Start()
		{
			base.Start();
			
			repeatsDone = 0;
			animations.ForEach(a => a.Stop());

			currentAnimation = null;
		}
		public override void Pause()
		{
			base.Pause();

			if (currentAnimation == null)
				return;
			
			currentAnimation.Pause();
		}
		public override void Resume()
		{
			base.Resume();

			if (currentAnimation == null)
				return;
			
			currentAnimation.Resume();
		}
		public override void Stop()
		{
			base.Stop();

			if (currentAnimation == null)
				return;
			
			repeatsDone = 0;
			animations.ForEach(a => a.Stop());
		}
		public override void UpdateAnimation()
		{
		}
	}
}

