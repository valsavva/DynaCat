using System;

namespace Lunohod.Objects
{
	public class NumericValueTimeline
	{
		private const double FrameRate = 50.0;
		private const double FrameFrequency = 1.0 / FrameRate;
		
		private double distance = 0;
		private double absDistance = 0;
		private double timeStep = 0;
		private double valueStep = 0;
		
		private double[] values;
		
		public bool isValid;

		public NumericValueTimeline()
		{
		}
		
		public TimeSpan Duration;
		
		public bool Autoreverse;
		
		public double From;
		
		public double To;
		
		public double MinValueChange = 1f;
		
		public bool IsValid
		{
			get {
				return this.isValid;
			}
		}		
		
		public void BuildTimeline()
		{
			distance = this.To - this.From;
			int direction = (int)(distance / Math.Abs(distance));
			absDistance = Math.Abs(distance);
			
			if (distance == 0)
				return;
			
			timeStep = this.Duration.TotalSeconds / (absDistance / MinValueChange);
			
			if (timeStep < FrameFrequency)
				timeStep = FrameFrequency;
			
			values = new double[(int)(this.Duration.TotalSeconds / timeStep) + 1];
			
			if (values.Length < 2)
				return;
			
			timeStep = this.Duration.TotalSeconds / (values.Length - 1);
			valueStep = distance / (values.Length - 1);
			
			double v = this.From;
			for(int i = 0; i < values.Length; i++)
			{
				values[i] = v;
				v += valueStep;
			}
			
			this.isValid = true;
		}
		
		public double GetValue(TimeSpan time)
		{
			if (!this.isValid)
				return this.From;
			
			int valueIndex;
			
			if (this.Autoreverse)
			{
				valueIndex = (int)(time.TotalSeconds / timeStep) % (values.Length * 2);
				if (valueIndex >= values.Length)
					valueIndex = values.Length - 1 - (valueIndex - values.Length);
			}
			else
				valueIndex = (int)(time.TotalSeconds / timeStep) % values.Length;
			
			return values[valueIndex];
		}
		
		public int GetIntValue(TimeSpan time)
		{
			return (int)Math.Round(this.GetValue(time));
		}
	}
}

