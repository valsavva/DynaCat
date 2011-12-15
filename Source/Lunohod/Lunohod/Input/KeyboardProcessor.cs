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

		private Keys[] pressedKeys = emptyKeys;
		
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

		public bool IsKeyPressed(Keys key)
		{
			return Array.IndexOf(pressedKeys, key) >= 0;
		}
		
		private void ProcessPressed(GameTime gameTime, Keys key)
		{
            GameEvent e = null;

			switch (key)
			{
				case Keys.Up : e = new GameEvent(GameEventType.Up, gameTime); break;
                case Keys.Down: e = new GameEvent(GameEventType.Down, gameTime); break;
                case Keys.Left: e = new GameEvent(GameEventType.Left, gameTime); break;
                case Keys.Right: e = new GameEvent(GameEventType.Right, gameTime); break;
                case Keys.Space: e = new GameEvent(GameEventType.Stop, gameTime); break;
			}

            if (e != null)
            {
                e.IsInstant = e.EventType == GameEventType.Stop
                    || pressedKeys.Contains(Keys.LeftControl)
                    || pressedKeys.Contains(Keys.RightControl);

                game.EnqueueEvent(e);
            }
		}
	}
}

