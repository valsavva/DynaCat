using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public class EventRateCounter
	{
		private double currentTime;
		private Queue<double> eventTimes = new Queue<double>();
		
		public EventRateCounter(TimeSpan timeSpan)
		{
			if (timeSpan == TimeSpan.Zero)
				throw new ArgumentException("Parameter cannot be zero", "timeSpan");
			
			this.TimeSpan = timeSpan;
		}
		
		public TimeSpan TimeSpan { get; private set; }
		
		public double EventsPerSecond
		{
			get
			{
				return 1.0 * eventTimes.Count / this.TimeSpan.TotalSeconds;
			}
		}
		
		public void RecordEvent()
		{
			eventTimes.Enqueue(currentTime);
			
			CheckQueue();
		}
		
		public void Update(GameTime gameTime)
		{
			currentTime += gameTime.ElapsedGameTime.TotalMilliseconds;

			CheckQueue();
		}

		private void CheckQueue()
		{
			while (eventTimes.Count > 0 && (currentTime - eventTimes.Peek() > this.TimeSpan.TotalMilliseconds))
				eventTimes.Dequeue();
		}
	}
}

