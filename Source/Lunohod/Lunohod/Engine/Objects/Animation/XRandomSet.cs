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
        private class ProbabilityList : List<double>
        {
            private Random random;
            private double total;

            public ProbabilityList(Random random, IEnumerable<double> source)
                : base(source)
            {
                this.random = random;
                total = this.Sum();
            }

            public int NextIndex()
            {
                double n = random.NextDouble() * total;
                double sum = 0;

                for (int i = 0; i < this.Count - 1; i++)
                {
                    sum += this[i];
                    if (sum >= n)
                        return i;
                }

                return this.Count - 1;
            }
        }

        private Random random = new Random();

        private IRunnable currentRunnable;
        private ProbabilityList probabilities;

        private int repeatsDone = 0;

        [XmlAttribute]
        public string Probabilities;

        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            if (!string.IsNullOrWhiteSpace(this.Probabilities))
            {
                probabilities = new ProbabilityList(this.random, this.Probabilities.Split(',').Select(s => double.Parse(s)));

                if (probabilities.Count != runnables.Count)
                    throw new InvalidOperationException(string.Format("Number of probabilities must match the number of runnables - {0} != {1}", probabilities.Count, runnables));
            }
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
            int index = probabilities == null ? random.Next(this.runnables.Count) : probabilities.NextIndex();

            return runnables[index];
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

            repeatsDone = 0;
            runnables.ForEach(a => a.Stop());
        }
        public override void UpdateAnimation()
        {
        }
    }
}
