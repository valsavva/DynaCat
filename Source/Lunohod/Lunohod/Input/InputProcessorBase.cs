using System;
using Microsoft.Xna.Framework;

namespace Lunohod
{
	public abstract class InputProcessorBase
	{
		protected GameEngine game;
			
		public InputProcessorBase(GameEngine game)
		{
			this.game = game;
		}
		
		public abstract void Process(GameTime gameTime);
	}
}

