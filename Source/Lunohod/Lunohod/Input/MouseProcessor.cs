using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using Lunohod.Objects;

namespace Lunohod
{
	public class MouseProcessor : InputProcessorBase
	{
        private bool pressed;

		public MouseProcessor(GameEngine game)
			: base(game)
		{
		}

		public static Vector2 LastPosition;
		
		public override void Process(GameTime gameTime)
		{
			var mouseState = Mouse.GetState();
			
			LastPosition.X = mouseState.X;
			LastPosition.Y = mouseState.Y;

            int x = (int)mouseState.X;
            int y = (int)mouseState.Y;

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (this.pressed)
                    game.ProcessTouch(gameTime, XTapType.Move, x, y);
                else
                {
                    this.pressed = true;
                    game.ProcessTouch(gameTime, XTapType.Press, x, y);
                }
            }
            else if (this.pressed)
            {
                this.pressed = false;
                game.ProcessTouch(gameTime, XTapType.Release, x, y);
            }

            if (mouseState.RightButton == ButtonState.Pressed)
                MoveHero((int)mouseState.X, (int)mouseState.Y);
		}

		private void MoveHero(int x, int y)
        {
            var levelEngine = game.ScreenEngine as LevelEngine;

            if (levelEngine == null || levelEngine.Hero == null)
                return;

            levelEngine.Hero.Bounds.X = x - levelEngine.Hero.Bounds.Width / 2;
            levelEngine.Hero.Bounds.Y = y - levelEngine.Hero.Bounds.Height / 2;
        }

		public override void ResetController()
		{
			Mouse.GetState();
		}
	}
}

