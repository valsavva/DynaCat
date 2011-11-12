using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Lunohod
{
	public class TouchPanelProcessor : InputProcessorBase
	{
		public TouchPanelProcessor(GameEngine game)
			: base(game)
		{
		}

		public override void Process(GameTime gameTime)
		{
			var touches = TouchPanel.GetState();
			
			for(int i = 0; i < touches.Count; i++)
			{
				var touch = touches[i];

				if (touch.State != TouchLocationState.Pressed)
					continue;
				
				game.ProcessTouch(gameTime, (int)touch.Position.X, (int)touch.Position.Y);
			}
		}
	}
}

