using System;
using Microsoft.Xna.Framework.Graphics;

namespace Lunohod
{
    [Flags]
	public enum XFlipEffects
	{
		None = SpriteEffects.None,
		Vertically = SpriteEffects.FlipVertically,
		Horizontally = SpriteEffects.FlipHorizontally
	}
}

