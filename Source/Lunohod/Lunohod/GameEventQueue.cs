using System;
using System.Collections;
using System.Collections.Generic;

namespace Lunohod
{
	public class GameEventQueue
	{
		private Queue<GameEvent> queue = new Queue<GameEvent>();

		public GameEventQueue()
		{
		}
		
		public void Enqueue(GameEvent e)
		{
			queue.Enqueue(e);
		}
			
		public GameEvent Dequeue()
		{
			return queue.Dequeue();
		}
		
		public int Count
		{
			get { return queue.Count; }	
		}
	}
}

