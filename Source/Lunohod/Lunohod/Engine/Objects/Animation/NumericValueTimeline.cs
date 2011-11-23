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
		
		private bool grow;
		
		public void BuildTimeline()
		{
			distance = Math.Abs(this.To - this.From);
			speed = this.distance / this.Duration.TotalSeconds;
			grow = this.To > this.From;
		}
		
		public double GetValue(TimeSpan time)
		{
			double dx;
			double dt;
			
			if (this.Autoreverse)
			{
				double totalDuration = (this.Duration.TotalSeconds * 2);
				
				dt = time.TotalSeconds;
				
				if (dt > totalDuration)
					dt = dt % totalDuration;
				
				dx = this.speed * dt;
				
				if (dx > this.distance)
					dx = this.distance - (dx - this.distance);
			}
			else
			{
				double totalDuration = this.Duration.TotalSeconds;

				dt = time.TotalSeconds;
				
				if (dt > totalDuration)
					dt = dt % totalDuration;

				dx = this.speed * dt;
			}
			
			if (grow)
				return this.From + dx;
			else
				return this.From - dx;
		}
		
		public int GetIntValue(TimeSpan time)
		{
			return (int)Math.Round(this.GetValue(time));
		}
	}
}

