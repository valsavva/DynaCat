using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    /// <summary>
    /// Animates a value <see cref="XNumAnimation.Target"/> starting
    /// from the value <see cref="XNumAnimation.From"/> to <see cref="XNumAnimation.To"/>.
    /// </summary>
    [XmlType("NumAnimation")]
    public class XNumAnimation : XRunnableBase
    {
        internal List<TimeCurve> curves;
        internal List<Property<double>> targets;
        private List<XKeyFrame> keyFrames;
        private List<double?> startValues;
		private XKeyFrame lastFrame;

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
        /// <summary>
        /// Specifies the animation duration. This is the amount of time between the start of the first keyframe and
        /// the end of the last one.
        /// </summary>
		[XmlAttribute]
		public string Duration;
        /// <summary>
        /// Specifies whether the animation should automatically re-play in reverse upon completion of the last keyframe.
        /// When set to True, it essentially doubles the duration of one animation repeat.
        /// </summary>
		[XmlAttribute]
		public bool Autoreverse;
        /// <summary>
        /// Specifies the behavior of the animation upon its completion. See <see cref="XAnimationFillBehavior"/> for details.
        /// </summary>
		[XmlAttribute]
		public XAnimationFillBehavior Fill;
		
		public bool EnableTrace;
		private List<Tuple<double, double>> trace;
		
		private double InternalDuration
		{
			get { return lastFrame.CurrentTime; }
		}
		
        public override void Initialize(InitializeParameters p)
        {
			if (EnableTrace)
				this.trace = new List<Tuple<double, double>>(2048);
			
            if (!string.IsNullOrEmpty(this.From))
            {
                // Use the From/To properties
                this.CreateSubcomponents();
                this.Subcomponents.Add(new XKeyFrame() { Time = "0", Value = this.From, Smoothing = this.Smoothing });
                this.Subcomponents.Add(new XKeyFrame() { Time = this.Duration, Value = this.To, Smoothing = this.Smoothing });
            }

            base.Initialize(p);

			PerfMon.Start("Other-Ani");

			// get targets
            GenerateTargets();
			startValues = new List<double?>(new double?[targets.Count]);

            // collect keyFrames and check for consistency
            keyFrames = this.GetComponents<XKeyFrame>().ToList();
            if (keyFrames.Any(k => k.CurveKeys.Count != targets.Count))
                throw new InvalidOperationException("Number of values in key frames must match number of targets.");
            if (keyFrames.Count < 2)
                throw new InvalidOperationException("Number of key frames must be 2 or more.");
			lastFrame = keyFrames[keyFrames.Count - 1];
			
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
			PerfMon.Stop("Other-Ani");
        }

        private void GenerateTargets()
        {
            var targetStrings = this.Target.Split(',');
            targets = new List<Property<double>>(targetStrings.Length);

            for(int i = 0; i < targetStrings.Length; i++)
                targets.Add((Property<double>)Compiler.CompileExpression<double>(this, targetStrings[i]));
        }

		/// <inheritdoc />
		public override void Start()
		{
			base.Start();

            for (int i = 0; i < startValues.Count; i++)
                startValues[i] = null;
		}

		/// <inheritdoc />
        public override void Stop()
        {
			if (this.inProgress)
			{
				if (this.Fill == XAnimationFillBehavior.Reset)
					this.elapsedTime = TimeSpan.Zero;
				else if (this.Fill == XAnimationFillBehavior.End)
					this.elapsedTime = TimeSpan.FromSeconds(this.Autoreverse ? this.InternalDuration * 2f : this.InternalDuration);
				else
				{
					if (this.RepeatCount > 0)
						this.elapsedTime = TimeSpan.FromSeconds(
							Math.Min(this.elapsedTime.TotalSeconds, this.RepeatCount * (this.Autoreverse ? this.InternalDuration * 2f : this.InternalDuration))
						);
					else if (this.RepeatTime > 0f)
						this.elapsedTime = TimeSpan.FromSeconds(
							Math.Min(this.elapsedTime.TotalSeconds, this.RepeatTime)
						);
				}

				UpdateAnimation();
			}
			
			base.Stop();
        }

		/// <exclude />
        internal override double CalculateRepeatsDone()
		{
			if (this.elapsedTime == TimeSpan.Zero || this.InternalDuration == 0f)
				return 0f;
			
			if (this.Autoreverse)
				return (this.elapsedTime.TotalSeconds / (this.InternalDuration * 2.0));
			else
				return (this.elapsedTime.TotalSeconds / this.InternalDuration);
		}
		
		private void UpdateAnimation()
		{
			for (int i = 0; i < targets.Count; i++)
            {
                for (int j = 0; j < curves[i].Keys.Count; j++)
                {
                    curves[i].ComputeT(j, keyFrames[j].Smoothing);
                }

                double newPropertyValue = curves[i].Evaluate((float)this.elapsedTime.TotalSeconds);

                if (this.IsDelta)
                {
                    if (!startValues[i].HasValue)
                        startValues[i] = targets[i].GetValue();

                    newPropertyValue += startValues[i].Value;
                }

                targets[i].SetValue(newPropertyValue);
            }
		}

        /// <exclude />
        internal override void UpdateProgress(UpdateParameters p)
		{
			// update keyframes first
			this.UpdateChildren(p);

			this.UpdateAnimation();
			
			if (this.EnableTrace && this.trace.Count < this.trace.Capacity)
				this.trace.Add(new Tuple<double, double>(p.GameTime.ElapsedGameTime.TotalMilliseconds,targets[0].GetValue()));
        }
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.Duration != null)
				this.Duration = this.Duration.Replace(par, val);
	        if (this.Target != null)
			    this.Target = this.Target.Replace(par, val);
			if (this.From != null)
				this.From = this.From.Replace(par, val);
			if (this.To != null)
				this.To = this.To.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}

		public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Target = reader["Target"];
            this.From = reader["From"];
            this.To = reader["To"];
            this.Duration = reader["Duration"];
            this.IsDelta = reader.ReadAttrAsBoolean("IsDelta");
            reader.ReadAttrAsEnum<CurveTangent>("Smoothing", ref this.Smoothing);
            this.Autoreverse = reader.ReadAttrAsBoolean("Autoreverse");
            reader.ReadAttrAsEnum<XAnimationFillBehavior>("Fill", ref this.Fill);
            reader.ReadAttrAsBoolean("EnableTrace", ref this.EnableTrace);
			
            base.ReadXml(reader);
        }
		
		public void PrintTrace()
		{
			double lastValue = 0;
			foreach(var item in this.trace)
			{
				Console.WriteLine("time: {0, 6} value: {1, 6} rndValue: {2, 6} delta: {3, 6}", item.Item1, item.Item2, Math.Round(item.Item2), item.Item2 - lastValue);
				lastValue = item.Item2;
			}
		}
		
		public override void GetMethod(string methodName, out Action<List<Expression>> method)
		{
            switch (methodName)
            {
				case "PrintTrace": method = (ps) => PrintTrace(); break;
                default:
					base.GetMethod(methodName, out method); break;
            }
		}
    }
}

