using System;

namespace Lunohod.Objects
{
	public class NumericValueTimeline
	{
		private const double FrameRate = 50.0;
		private const double FrameFrequency = 1.0 / FrameRate;
		
		private double distance = 0;
		private double speed;
		
		public NumericValueTimeline()
		{
		}
		
		public TimeSpan Duration;
		
		public bool Autoreverse;
		
		public double From;
		
		public double To;
		
		public double MinValueChange = 1f;
		
		public void BuildTimeline()
		{
			distance = this.To - this.From;
			speed = this.distance / this.Duration.TotalSeconds;
		}
		
		public double GetValue(TimeSpan time)
		{
			if (this.Autoreverse)
			{
				var v = this.speed * (time.TotalSeconds % (this.Duration.TotalSeconds * 2));
				if (v > this.distance)
					v = this.distance - (v - this.distance);
				return this.From + v;
			}
			else
				return this.From + this.speed * (time.TotalSeconds % this.Duration.TotalSeconds);
		}
		
		public int GetIntValue(TimeSpan time)
		{
			return (int)Math.Round(this.GetValue(time));
		}
	}
}

