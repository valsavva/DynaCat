using System;
using System.Collections;
using System.Collections.Generic;
using Lunohod.Objects;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public class GameEvent
	{
		public static readonly Dictionary<string, GameEventType> EventTypeDict = new Dictionary<string, GameEventType>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "system:up", GameEventType.Up },
			{ "system:down", GameEventType.Down },
			{ "system:left", GameEventType.Left },
			{ "system:right", GameEventType.Right },
			{ "system:stop", GameEventType.Stop },
			{ "system:explosion", GameEventType.Explosion }
		};
		
		private GameEvent(GameTime gameTime)
		{
			this.Time = gameTime.TotalGameTime;
			this.IsHandled = false;
		}
		
		public GameEvent(GameEventType eventType, GameTime gameTime)
			: this(gameTime)
		{
			this.EventType = eventType;
		}
		public GameEvent(string id, GameTime gameTime)
			: this(gameTime)
		{
			if (!EventTypeDict.TryGetValue(id, out this.EventType))
			{
				this.EventType = GameEventType.Custom;
				this.Id = id;
			}
		}
		
		public string Id;
		
		public TimeSpan Time;
		
		public GameEventType EventType;

		public bool IsInstant;
		
		public bool IsHandled;
	}
}

