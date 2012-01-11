using System;
using System.Collections.Generic;

namespace Lunohod.Objects
{
	public class SignalContainer
	{
		public HashSet<string> hashset;
		
		public void Clear()
		{
			if (hashset != null)
				hashset.Clear();
		}
		
		public void Signal(string eventId)
		{
			if (hashset == null)
				hashset = new HashSet<string>();
			
			hashset.Add(eventId);
		}
		
		public bool IsSignaled(string eventId)
		{
			return (hashset != null) && hashset.Contains(eventId);
		}
	}
}

