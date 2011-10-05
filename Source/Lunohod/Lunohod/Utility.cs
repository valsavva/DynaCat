using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Lunohod.Objects;

using Microsoft.Xna.Framework;
using System.Drawing;

namespace Lunohod
{
    public static class Utility
    {
        public static Color ToColor(this string s)
        {
            Color c = new Color(
                Byte.Parse(s.Substring(3,2), System.Globalization.NumberStyles.AllowHexSpecifier),
                Byte.Parse(s.Substring(5,2), System.Globalization.NumberStyles.AllowHexSpecifier),
                Byte.Parse(s.Substring(7,2), System.Globalization.NumberStyles.AllowHexSpecifier),
                Byte.Parse(s.Substring(1,2), System.Globalization.NumberStyles.AllowHexSpecifier)
            );

            return c;
        }

        public static RectangleF ToRect(this string s)
        {
            string[] parts = s.Split(',');
            return new RectangleF(
                int.Parse(parts[0], CultureInfo.InvariantCulture),
                int.Parse(parts[1], CultureInfo.InvariantCulture),
                int.Parse(parts[2], CultureInfo.InvariantCulture),
                int.Parse(parts[3], CultureInfo.InvariantCulture)
            );
        }
		
		public static Microsoft.Xna.Framework.Rectangle ToXnaRectangle(this RectangleF r)
		{
			return new Microsoft.Xna.Framework.Rectangle(
				(int)Math.Round(r.X),
				(int)Math.Round(r.Y),
				(int)Math.Round(r.Width),
				(int)Math.Round(r.Height)
			);
		}
		
		public static string ToString(this Color c)
		{
			return c.ToString();
		}

        public static string ToBounds(this RectangleF rect)
        {
            return rect.Left.ToString(CultureInfo.InvariantCulture) + "," + rect.Top.ToString(CultureInfo.InvariantCulture) + "," + rect.Width.ToString(CultureInfo.InvariantCulture) + "," + rect.Height.ToString(CultureInfo.InvariantCulture);
        }

        public static XHeroMoveType ReverseMove(this XAlignType m)
        {
            return ReverseMove((XHeroMoveType)m);
        }

        public static XHeroMoveType ReverseMove(this XHeroMoveType m)
        {
            XHeroMoveType r = XHeroMoveType.Left;
            switch (m)
            {
                case XHeroMoveType.Left: r = XHeroMoveType.Right; break;
                case XHeroMoveType.Right: r = XHeroMoveType.Left; break;
                case XHeroMoveType.Up: r = XHeroMoveType.Down; break;
                case XHeroMoveType.Down: r = XHeroMoveType.Up; break;
            }
            return r;
        }

        public static XAlignType ReverseEdge(this XHeroMoveType m)
        {
            XAlignType r = XAlignType.Left;
            switch (m)
            {
                case XHeroMoveType.Left: r = XAlignType.Right; break;
                case XHeroMoveType.Right: r = XAlignType.Left; break;
                case XHeroMoveType.Up: r = XAlignType.Bottom; break;
                case XHeroMoveType.Down: r = XAlignType.Top; break;
            }
			return r;
        }

        public static double Area(this RectangleF rect)
        {
            return rect.Width * rect.Height;
        }
		
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> a)
		{
			if (collection == null)
				return;
			
			foreach(T item in collection)
				a(item);
		}
    }
}
