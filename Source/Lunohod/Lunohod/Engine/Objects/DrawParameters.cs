using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod.Objects
{
	public class DrawParameters
	{
		public DrawParameters ()
		{
		}
		
		public GameEngine Game;
		public ScreenEngine ScreenEngine;
		public XResourceBundle Resources;
		public GameTime GameTime;
		public SpriteBatch SpriteBatch;
	}
}

