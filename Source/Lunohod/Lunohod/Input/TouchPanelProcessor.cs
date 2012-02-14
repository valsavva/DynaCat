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
		
		public static Vector2 LastPosition;
		
		public override void Process(GameTime gameTime)
		{
			var touches = TouchPanel.GetState();
			
			for(int i = 0; i < touches.Count; i++)
			{
				var touch = touches[i];
				
#if DEBUG
				Console.WriteLine("Touch: {0},{1} State: {2}", touch.Position.X, touch.Position.Y, touch.State);
#endif
				
				LastPosition = touch.Position;
				
				if (touch.State != TouchLocationState.Pressed)
					continue;
				
				game.ProcessTouch(gameTime, (int)(touch.Position.X / game.Scale.X), (int)(touch.Position.Y / game.Scale.Y));
			}
		}
	}
}

