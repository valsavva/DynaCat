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
		
		private static readonly Vector2[] alignToVectorMap = 
			{ VectorUp, VectorLeft, VectorRight, VectorDown };
		
		public static XAlignType Reverse(this XAlignType align)
		{
			return (XAlignType)((int)align ^ 3);
		}
		
		public static XAlignType ToAlign(this Vector2 v)
		{
			return (XAlignType)Array.IndexOf(alignToVectorMap, v);
		}
		
		public static Vector2 ToVector(this XAlignType align)
		{
			return alignToVectorMap[(int)align];
		}
	}
}

