using System;
using System.Collections;
using System.Collections.Generic;
using Lunohod.Objects;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public class GameEvent
	{
		private static readonly Dictionary<string, GameEventType> eventNameTypeDict = new Dictionary<string, GameEventType>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "system:up", GameEventType.Up },
			{ "system:down", GameEventType.Down },
			{ "system:left", GameEventType.Left },
			{ "system:right", GameEventType.Right },
			{ "system:stop", GameEventType.Stop },
			{ "system:explosion", GameEventType.Explosion },
			{ "system:closeScreen", GameEventType.CloseCurrentScreen },
			{ "system:restartLevel", GameEventType.RestartLevel },
			{ "system:abandonLevel", GameEventType.AbandonLevel },
			{ "system:startNextLevel", GameEventType.StartNextLevel },
			{ "system:endLevel", GameEventType.EndLevel },
			
			{ "system:levelLoaded", GameEventType.LevelLoaded }
		};
		
		private static readonly Dictionary<GameEventType, string> eventTypeNameDict = new Dictionary<GameEventType, string>();
		
		static GameEvent()
		{
			foreach(var item in eventNameTypeDict)
				eventTypeNameDict.Add(item.Value, item.Key);
		}
		
		private GameEvent(GameTime gameTime)
		{
			this.Time = gameTime.TotalGameTime;
			this.IsHandled = false;
		}
		
		public GameEvent(GameEventType eventType, GameTime gameTime)
			: this(gameTime)
		{
			this.EventType = eventType;
			this.Id = eventTypeNameDict[eventType];
		}
		public GameEvent(string id, GameTime gameTime)
			: this(gameTime)
		{
			this.Id = id;

			if (!eventNameTypeDict.TryGetValue(this.Id, out this.EventType))
				this.EventType = GameEventType.Custom;
		}
		
		public string Id;
		
		public TimeSpan Time;
		
		public GameEventType EventType;

		public bool IsInstant;
		
		public bool IsHandled;
		
		public override string ToString()
		{
			return string.Format("[GameEvent Type:'{0}' Id:'{1}' IsInstant:{2}]", this.EventType, this.Id, this.IsInstant);
		}
	}
}

