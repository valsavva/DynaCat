﻿using System;
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
                total = 0;
				for(int i = 0; i < this.Count; i++)
				{
					total += this[i];
					this[i] = total;
				}
            }

            public int NextIndex()
            {
                double n = random.NextDouble() * total;

                for (int i = 0; i < this.Count - 1; i++)
                {
                    if (this[i] >= n)
                        return i;
                }

                return this.Count - 1;
            }
        }

        private Random random = new Random();

        private IRunnable currentRunnable;
        private ProbabilityList probabilities;

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

        internal override void UpdateProgress(UpdateParameters p)
        {
            if (currentRunnable == null)
            {
                currentRunnable = GetNextAnimation();
                currentRunnable.Start();
            }

			currentRunnable.Update(p);

            if (!currentRunnable.InProgress)
			{
                repeatsDone++;
				currentRunnable = null;
			}
		}

        private IRunnable GetNextAnimation()
        {
            int index = probabilities == null ? random.Next(this.runnables.Count) : probabilities.NextIndex();

            return runnables[index];
        }

        public override void Start()
        {
            base.Start();

			if (currentRunnable != null)
				currentRunnable.Stop();

            currentRunnable = null;
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

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Probabilities = reader["Probabilities"];

            base.ReadXml(reader);
        }
    }
}
