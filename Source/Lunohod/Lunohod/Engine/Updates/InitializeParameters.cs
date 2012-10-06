using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Lunohod
{
	public class InitializeParameters : ParamentersBase
	{
        public ContentManager Content { get { return this.ScreenEngine != null ? this.ScreenEngine.Content : this.Game.Content; } }
	}
}

