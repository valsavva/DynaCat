using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using System.Diagnostics;

namespace Lunohod
{
	public class TouchPanelProcessor : InputProcessorBase
	{
		private bool released;
		
		public TouchPanelProcessor(GameEngine game)
			: base(game)
		{
			this.released = true;
		}
		
		public static Vector2 LastPosition;
		
		public override void Process(GameTime gameTime)
		{
			var touches = TouchPanel.GetState();
			
			for(int i = 0; i < touches.Count; i++)
			{
				var touch = touches[i];
				
#if DEBUG
				Debug.WriteLine("Touch: {0},{1} State: {2}", touch.Position.X, touch.Position.Y, touch.State);
#endif
				
				LastPosition = touch.Position;
				
				if (touch.State == TouchLocationState.Released)
				{
					this.released = true;
					continue;
				}
				
				if (touch.State == TouchLocationState.Invalid)
					continue;
				
				if (touch.State == TouchLocationState.Moved && !this.released)
					continue;
				else
				{
					Debug.WriteLine("** Issuing a touch! **");
				}
				
				this.released = false;
				
				game.ProcessTouch(gameTime, (int)(touch.Position.X / game.Scale.X), (int)(touch.Position.Y / game.Scale.Y));
			}
		}
	}
}

