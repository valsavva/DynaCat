using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

namespace Lunohod
{
	public class MouseProcessor : InputProcessorBase
	{
		public MouseProcessor(GameEngine game)
			: base(game)
		{
		}

		public override void Process(GameTime gameTime)
		{
			var mouseState = Mouse.GetState();

			if (mouseState.LeftButton == ButtonState.Pressed)
				game.ProcessTouch(gameTime, (int)mouseState.X, (int)mouseState.Y);
		}
	}
}

