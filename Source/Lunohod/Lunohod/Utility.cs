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
			if (s.Length == 8)
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
            return v.X.ToString(CultureInfo.InvariantCulture) + 
                v.Y.ToString(CultureInfo.InvariantCulture);
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
		
		public static int Area(this Microsoft.Xna.Framework.Rectangle rect)
		{
			return rect.Width * rect.Height;
		}
		
		public static string ToStr(this Color c)
		{
			return c.ToString();
		}

        public static string ToBounds(this Microsoft.Xna.Framework.Rectangle rect)
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
    }
}
