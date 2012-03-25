using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;
using Lunohod.Objects;

namespace Lunohod
{
	public class TouchPanelProcessor : InputProcessorBase
	{
		private bool pressed;
		
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
				
                if (touch.State == TouchLocationState.Invalid)
                    continue;
                
                LastPosition = touch.Position;
				
                int x = (int)(touch.Position.X / game.Scale.X);
                int y = (int)(touch.Position.Y / game.Scale.Y);

				if (touch.State == TouchLocationState.Released)
				{
                    this.pressed = false;
                    game.ProcessTouch(gameTime, XTapType.Release, x, y);
					continue;
				}
				
				if (touch.State == TouchLocationState.Moved)
                {
                     if (this.pressed)
                     {
                         game.ProcessTouch(gameTime, XTapType.Move, x, y);
                         continue;
                     }
//                     else
//                         Debug.WriteLine("** Issuing a touch! **");
                }

                this.pressed = true;
                game.ProcessTouch(gameTime, XTapType.Press, x, y);
            }
		}
	}
}

