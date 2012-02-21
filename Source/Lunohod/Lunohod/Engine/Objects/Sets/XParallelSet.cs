using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Lunohod.Objects
{
    [XmlType("ParallelSet")]
    public class XParallelSet : XSetBase
    {
        internal override void UpdateProgress(UpdateParameters p)
        {
            base.UpdateChildren(p);

			if (runnables.All(a => !a.InProgress))
                // all animations executed
                repeatsDone++;
        }

        public override void Start()
        {
            base.Start();
            runnables.ForEach(a => a.Start());
        }

        public override void Stop()
        {
            base.Stop();
			runnables.ForEach(a => a.Stop());
        }

        public override void Pause()
        {
            base.Pause();
			runnables.ForEach(a => a.Pause());
        }

        public override void Resume()
        {
            base.Resume();
			runnables.ForEach(a => a.Resume());
        }
    }
}
