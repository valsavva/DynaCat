using System;
using Microsoft.Xna.Framework;
using Lunohod.Objects;

namespace Lunohod
{
	public static class Direction
	{
		public static readonly Vector2 VectorUp = new Vector2(0, -1);
		public static readonly Vector2 VectorLeft = new Vector2(-1, 0);
		public static readonly Vector2 VectorRight = new Vector2(1, 0);
		public static readonly Vector2 VectorDown = new Vector2(0, 1);
		public static readonly Vector2 VectorStop = new Vector2(0, 0);
		
		public static Vector2 Reverse(this Vector2 v)
		{
			return v * -1.0f;
		}
	}
}

