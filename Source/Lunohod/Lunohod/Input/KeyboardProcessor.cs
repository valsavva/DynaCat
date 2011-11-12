using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public class KeyboardProcessor : InputProcessorBase
	{
		private static readonly Keys[] emptyKeys = new Keys[0];

		private Keys[] pressedKeys;
		
		public KeyboardProcessor(GameEngine game)
			: base(game)
		{
		}
		
		public override void Process(GameTime gameTime)
		{
			var state = Keyboard.GetState();
			
			var newPressedKeys = state.GetPressedKeys();
			
			if (newPressedKeys.Length == 0)
			{
				// little trick to optimize GC
				pressedKeys = emptyKeys;
				return;
			}

			for(int i = 0; i < newPressedKeys.Length; i++)
			{
				Keys key = newPressedKeys[i];
				
				if (pressedKeys.Contains(key))
					continue;
				
				ProcessPressed(gameTime, key);
			}
			
			pressedKeys = newPressedKeys;
		}

		private void ProcessPressed(GameTime gameTime, Keys key)
		{
			switch (key)
			{
				case Keys.Up : game.EnqueueEvent(new GameEvent(GameEventType.Up, gameTime)); break;
				case Keys.Down : game.EnqueueEvent(new GameEvent(GameEventType.Down, gameTime)); break;
				case Keys.Left : game.EnqueueEvent(new GameEvent(GameEventType.Left, gameTime)); break;
				case Keys.Right : game.EnqueueEvent(new GameEvent(GameEventType.Right, gameTime)); break;
			}
		}
	}
}

