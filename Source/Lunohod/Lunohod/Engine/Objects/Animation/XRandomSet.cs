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
    [XmlType("RandomSet")]
    public class XRandomSet : XSetBase
    {
        private Random random = new Random();

        private List<IRunnable> runnables;
        private IRunnable currentRunnable;

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
                if (currentRunnable == null)
                {
                    currentRunnable = GetNextAnimation();
                    currentRunnable.Start();
                }
                else if (!currentRunnable.InProgress)
                {
                    repeatsDone++;

                    if (repeatsDone >= this.RepeatCount)
                    {
                        this.Stop();
                        return;
                    }

                    currentRunnable = GetNextAnimation();
                    currentRunnable.Start();
                }
            }

            base.Update(p);
        }

        private IRunnable GetNextAnimation()
        {
            return runnables[random.Next(this.runnables.Count)];
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

            repeatsDone = 0;
            runnables.ForEach(a => a.Stop());
        }
        public override void UpdateAnimation()
        {
        }
    }
}

