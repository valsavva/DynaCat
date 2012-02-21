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
    [XmlType("NumAnimation")]
    public class XNumAnimation : XAnimationBase
    {
        private List<TimeCurve> curves;
        private List<PropertyAccessor> targets;
        private List<XKeyFrame> keyFrames;

        [XmlAttribute]
        public string Target;
        [XmlAttribute]
        public string From;
        [XmlAttribute]
        public string To;
		[XmlAttribute]
		public bool IsDelta;
		
		private List<float?> startValues;
		
        public override void Initialize(InitializeParameters p)
        {
            if (!string.IsNullOrEmpty(this.From))
            {
                // Use the From/To properties
                this.Subcomponents.Add(new XKeyFrame() { Time = TimeSpan.Zero, Value = this.From });
                this.Subcomponents.Add(new XKeyFrame() { Time = this.Duration, Value = this.To });
            }

            base.Initialize(p);

            // get targets
            targets = this.Target.Split(',').Select(s => new PropertyAccessor(this, s)).ToList();
			startValues = new List<float?>(new float?[targets.Count]);

            // collect keyFrames and check for consistency
            keyFrames = this.GetComponents<XKeyFrame>().ToList();
            if (keyFrames.Any(k => k.CurveKeys.Count != targets.Count))
                throw new InvalidOperationException("Number of values in key frames must match number of targets.");
            if (keyFrames.Count < 2)
                throw new InvalidOperationException("Number of key frames must be 2 or more.");

            // create curves
            curves = new List<TimeCurve>(targets.Count);
            for (int i = 0; i < targets.Count; i++)
            {
                var curve = new TimeCurve();
                curve.PostLoop = this.Autoreverse ? CurveLoopType.Oscillate : CurveLoopType.Cycle;
                curve.PreLoop = this.Autoreverse ? CurveLoopType.Oscillate : CurveLoopType.Cycle;

                for (int j = 0; j < keyFrames.Count; j++)
                {
                    curve.Keys.Add(keyFrames[j].CurveKeys[i]);
                }

                curves.Add(curve);
            }
        }
		
		public override void Start()
		{
			base.Start();

            for (int i = 0; i < startValues.Count; i++)
                startValues[i] = null;
		}
		
		protected override void UpdateAnimation()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                for (int j = 0; j < curves[i].Keys.Count; j++)
                {
                    curves[i].ComputeT(j, keyFrames[j].Smoothing);
                }

                var newPropertyValue = (float)curves[i].Evaluate((float)this.elapsedTime.TotalMilliseconds);

                if (this.IsDelta)
                {
                    if (!startValues[i].HasValue)
                        startValues[i] = targets[i].FloatPropertyValue;

                    newPropertyValue += startValues[i].Value;
                }

                targets[i].FloatPropertyValue = newPropertyValue;
            }
        }
		internal override void ReplaceParameter(string par, string val)
		{
	        if (this.Target != null)
			    this.Target = this.Target.Replace(par, val);
			if (this.From != null)
				this.From = this.From.Replace(par, val);
			if (this.To != null)
				this.To = this.To.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
    }
}

