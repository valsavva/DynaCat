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
    /// <summary>
    /// Animates a value <see cref="XNumAnimation.Target"/> starting
    /// from the value <see cref="XNumAnimation.From"/> to <see cref="XNumAnimation.To"/>.
    /// </summary>
    [XmlType("NumAnimation")]
    public class XNumAnimation : XAnimationBase
    {
        private List<TimeCurve> curves;
        private List<PropertyAccessor> targets;
        private List<XKeyFrame> keyFrames;
        private List<float?> startValues;

        /// <summary>
        /// A property, or a comma-delimited list of properties that will be animated.
        /// </summary>
        [XmlAttribute]
        public string Target;
        /// <summary>
        /// The starting value, or a comma-delimited list of corresponding starting values.
        /// </summary>
        [XmlAttribute]
        public string From;
        /// <summary>
        /// The ending value, or a comma-delimited list of corresponding ending values.
        /// </summary>
        [XmlAttribute]
        public string To;
        /// <summary>
        /// Specifies whether the <see cref="From"/> and the <see cref="To"/> values
        /// are not absolute values, but rather offsets of the target value at the beginning of the animation.
        /// I.e. the animation goes from <c>Target+From</c> to <c>Target+To</c>.
        /// </summary>
		[XmlAttribute]
		public bool IsDelta;
        /// <summary>
        /// Specifies smoothing for the "short" form of animation
        /// using "From/To" attributes instead of group of keyframes. Default is <see cref="CurveTangent.Linear"/>.
        /// </summary>
        [XmlAttribute]
        public CurveTangent Smoothing = CurveTangent.Linear;
		
        public override void Initialize(InitializeParameters p)
        {
            if (!string.IsNullOrEmpty(this.From))
            {
                // Use the From/To properties
                this.Subcomponents.Add(new XKeyFrame() { Time = TimeSpan.Zero, Value = this.From, Smoothing = this.Smoothing });
                this.Subcomponents.Add(new XKeyFrame() { Time = this.Duration, Value = this.To, Smoothing = this.Smoothing });
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
		/// <inheritdoc />
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

