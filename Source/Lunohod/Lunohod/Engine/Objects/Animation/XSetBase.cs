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
    public class XSetBase : XAnimationBase
    {
		protected List<IRunnable> runnables;

		public XSetBase()
        {
        }

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			runnables = this.CollectRunnables();
		}
		
		public override void UpdateAnimation()
		{
			
		}
	}
}
