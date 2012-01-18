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
        private List<IRunnable> runnables;
        private int repeatsDone = 0;

        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            runnables = this.CollectRunnables();
        }
        
        public override void Update(UpdateParameters p)
        {
            if (this.inProgress && !this.isPaused)
            {
                if (runnables.All(a => !a.InProgress))
                {
                    // all animations executed
                    repeatsDone++;

                    if (repeatsDone >= this.RepeatCount)
                        this.Stop();
                    else
                        this.Start();
                }
            }

            base.Update(p);

            if (this.inProgress && !this.isPaused)
            {
                if (runnables.All(a => !a.InProgress))
                {
                    // all animations executed
                    repeatsDone++;

                    if (repeatsDone >= this.RepeatCount)
                        this.Stop();
                    else
                        this.Start();
                }
            }
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
