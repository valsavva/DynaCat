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

            runnables = new List<IRunnable>();
            CollectRunnables(this, runnables);
			runnables.ForEach(r => r.InProgress = false);

			if (this.inProgress)
				this.Start();
		}

        private static List<IRunnable> CollectRunnables(XObject obj, List<IRunnable> result)
        {
            if (obj.Subcomponents != null)
                for (int i = 0; i < obj.Subcomponents.Count; i++)
                {
                    var subcomponent = obj.Subcomponents[i];
                    IRunnable runnable = subcomponent as IRunnable;
                    if (runnable != null)
                        result.Add(runnable);

                    // we don't collect other sets' runnables
                    if (!(subcomponent is XSetBase))
                        CollectRunnables(subcomponent, result);
                }

            return result;
        }
    }
}
