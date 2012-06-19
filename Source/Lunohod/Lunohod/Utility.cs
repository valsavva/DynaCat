using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using Lunohod.Objects;

using Microsoft.Xna.Framework;

namespace Lunohod
{
    public static class Utility
    {
        public static Color ToColor(this string s)
        {
			Color c;
			if (s.Length == 9)
			{
	            c = new Color(
	                Byte.Parse(s.Substring(3,2), System.Globalization.NumberStyles.AllowHexSpecifier),
	                Byte.Parse(s.Substring(5,2), System.Globalization.NumberStyles.AllowHexSpecifier),
	                Byte.Parse(s.Substring(7,2), System.Globalization.NumberStyles.AllowHexSpecifier),
	                Byte.Parse(s.Substring(1,2), System.Globalization.NumberStyles.AllowHexSpecifier)
	            );
			} 
			else
			{
	            c = new Color(
	                Byte.Parse(s.Substring(1,2), System.Globalization.NumberStyles.AllowHexSpecifier),
	                Byte.Parse(s.Substring(3,2), System.Globalization.NumberStyles.AllowHexSpecifier),
	                Byte.Parse(s.Substring(5,2), System.Globalization.NumberStyles.AllowHexSpecifier)
	            );
			}
				
            return c;
        }
		
		public static Vector2 ToVector2(this string s)
		{
            string[] parts = s.Split(',');
			return new Vector2(
                float.Parse(parts[0], CultureInfo.InvariantCulture),
                float.Parse(parts[1], CultureInfo.InvariantCulture)
			);
		}
		
		public static string ToStr(this Vector2 v)
		{
            return v.X.ToString(CultureInfo.InvariantCulture) + "," +
                v.Y.ToString(CultureInfo.InvariantCulture);
		}

        public static TimeSpan ToDuration(this string s)
        {
            if (s.Contains(":"))
                return TimeSpan.Parse(s, CultureInfo.InvariantCulture);
            else
                return TimeSpan.FromSeconds(double.Parse(s, CultureInfo.InvariantCulture));
        }

        public static Microsoft.Xna.Framework.Point ToPoint(this string s)
        {
            string[] parts = s.Split(',');
            return new Microsoft.Xna.Framework.Point(
                int.Parse(parts[0], CultureInfo.InvariantCulture),
                int.Parse(parts[1], CultureInfo.InvariantCulture)
            );
        }

        public static string ToStr(this Microsoft.Xna.Framework.Point p)
        {
            return p.X.ToString(CultureInfo.InvariantCulture) +
                p.Y.ToString(CultureInfo.InvariantCulture);
        }

        public static Microsoft.Xna.Framework.Rectangle ToRect(this string s)
        {
            string[] parts = s.Split(',');
            return new Microsoft.Xna.Framework.Rectangle(
                int.Parse(parts[0], CultureInfo.InvariantCulture),
                int.Parse(parts[1], CultureInfo.InvariantCulture),
                int.Parse(parts[2], CultureInfo.InvariantCulture),
                int.Parse(parts[3], CultureInfo.InvariantCulture)
            );
        }

        public static System.Drawing.RectangleF ToRectF(this string s)
        {
            string[] parts = s.Split(',');
            return new System.Drawing.RectangleF(
                int.Parse(parts[0], CultureInfo.InvariantCulture),
                int.Parse(parts[1], CultureInfo.InvariantCulture),
                int.Parse(parts[2], CultureInfo.InvariantCulture),
                int.Parse(parts[3], CultureInfo.InvariantCulture)
            );
        }

        public static int Area(this Microsoft.Xna.Framework.Rectangle rect)
		{
			return rect.Width * rect.Height;
		}
		
		public static string ToStr(this Color c)
		{
			return c.ToString();
		}

        public static string ToStr(this Microsoft.Xna.Framework.Rectangle rect)
        {
            return rect.Left.ToString(CultureInfo.InvariantCulture) + "," + rect.Top.ToString(CultureInfo.InvariantCulture) + "," + rect.Width.ToString(CultureInfo.InvariantCulture) + "," + rect.Height.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStr(this System.Drawing.RectangleF rect)
        {
            return rect.Left.ToString(CultureInfo.InvariantCulture) + "," + rect.Top.ToString(CultureInfo.InvariantCulture) + "," + rect.Width.ToString(CultureInfo.InvariantCulture) + "," + rect.Height.ToString(CultureInfo.InvariantCulture);
        }

        public static void ForEach<T>(this IList<T> collection, Action<T> a)
		{
			if (collection == null)
				return;
			for(int i = 0; i < collection.Count; i++)
				a(collection[i]);
		}

        public static void ToVector2(this Rectangle r, ref Vector2 v)
        {
            v.X = r.X;
            v.Y = r.Y;
        }

        public static void ToVector2(this System.Drawing.RectangleF r, ref Vector2 v)
        {
            v.X = r.X;
            v.Y = r.Y;
        }

        public static void ToVector2(this Point p, ref Vector2 v)
		{
			v.X = p.X;
			v.Y = p.Y;
		}
		
		public static Point Location(this Rectangle r)
		{
			return new Point(r.X, r.Y);
		}
		
		public static double DistanceTo(this Point p1, Point p2)
		{
			int dx = p1.X - p2.X;
			int dy = p1.Y - p2.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}

        public static double SquaredDistanceTo(this Point p1, Point p2)
        {
            int dx = p1.X - p2.X;
            int dy = p1.Y - p2.Y;
            return dx * dx + dy * dy;
        }

        public static double SquaredDistanceTo(this Vector2 p1, Vector2 p2)
		{
			double dx = p1.X - p2.X;
			double dy = p1.Y - p2.Y;
			return dx * dx + dy * dy;
		}

        public static Vector2 Center(this System.Drawing.RectangleF rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Y + rect.Height / 2.0F);
        }

        public static void Center(this System.Drawing.RectangleF rect, ref Vector2 v)
        {
            v.X = rect.X + rect.Width / 2.0f;
            v.Y = rect.Y + rect.Height / 2.0F;
        }

        public static void Intersect(ref System.Drawing.RectangleF rect1, ref System.Drawing.RectangleF rect2, out System.Drawing.RectangleF rectInt)
        {
			rectInt = System.Drawing.RectangleF.Intersect(rect1, rect2);
        }

        public static double Area(this System.Drawing.RectangleF rect)
        {
            return rect.Width * rect.Height;
        }

        public static void ToRectangle(this System.Drawing.RectangleF rectF, ref Rectangle rect)
        {
            rect.X = (int)Math.Round(rectF.X);
            rect.Y = (int)Math.Round(rectF.Y);
            rect.Width = (int)Math.Round(rectF.Width);
            rect.Height = (int)Math.Round(rectF.Height);
        }
		
		public static void Clear(this System.Drawing.RectangleF rectF)
		{
			rectF.X = 0; rectF.Y = 0; rectF.Width = 0; rectF.Height = 0;
		}

		public static bool IsZero(this System.Drawing.RectangleF rectF)
		{
			return rectF.Width == 0 && rectF.Height == 0 && rectF.X == 0 && rectF.Y == 0;
		}
		
		public static TSource FirstOrNew<TSource>(this IEnumerable<TSource> source, Action<TSource> touchup = null)
			where TSource : new()
		{
			var result = source.FirstOrDefault();
			if (result == null)
			{
				result = new TSource();
				if (touchup != null)
					touchup(result);
			}
			return result;
		}
    }
}
