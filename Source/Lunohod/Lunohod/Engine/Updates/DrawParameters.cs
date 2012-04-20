using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod
{
	public class DrawParameters : ParamentersBase
	{
		public void Initiazlize(GameTime gameTime)
		{
			this.GameTime = gameTime;
			this.SystemImageDepth = 0.0001f;
		}
		
		public GameTime GameTime;
		public SpriteBatchWithFloats SpriteBatch;
		public float SystemImageDepth;
		
		public float NextSystemImageDepth()
		{
			return this.SystemImageDepth -= 0.0000001f;
		}
	}
}

