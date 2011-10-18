using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
	public class PropertyAccessor
	{
		private XElement target;
		private string property;

		private Func<XElement, float> getter;
		private Action<XElement, float> setter;

		private PropertyAccessor(XElement target, string property, Func<XElement, float> getter, Action<XElement, float> setter)
		{
            this.target = target;
            this.property = property;
            this.getter = getter;
			this.setter = setter;
		}

		public XElement Target
		{
			get {
				return this.target;
			}
		}		

		public string Property
		{
			get {
				return this.property;
			}
		}
		
        public float PropertyValue
        {
            get { return this.getter(target);  }
            set { setter(target, value);  }
        }

		public static PropertyAccessor CreatePropertyAccessor(XElement target, string property)
		{
			switch(property)
			{
				case "X": return new PropertyAccessor(target, property, GetX, SetX);
				case "Y": return new PropertyAccessor(target, property, GetY, SetY);
				case "Height": return new PropertyAccessor(target, property, GetHeight, SetHeight);
				case "Width": return new PropertyAccessor(target, property, GetWidth, SetWidth);
				case "Rotation": return new PropertyAccessor(target, property, GetRotation, SetRotation);
				case "Opacity": return new PropertyAccessor(target, property, GetOpacity, SetOpacity);
				case "CurrentFrame": return new PropertyAccessor(target, property, GetCurrentFrame, SetCurrentFrame);
				default: throw new InvalidOperationException("Unknown attribute");
			}
		}
					
		private static float GetX(XElement e)
		{
			return e.Bounds.X;
		}
		private static void SetX(XElement e, float v)
		{
			e.Bounds.X = (int)Math.Round(v);
		}
		private static float GetY(XElement e)
		{
			return e.Bounds.Y;
		}
		private static void SetY(XElement e, float v)
		{
			e.Bounds.Y = (int)Math.Round(v);
		}
		private static float GetWidth(XElement e)
		{
			return e.Bounds.Width;
		}
		private static void SetWidth(XElement e, float v)
		{
			e.Bounds.Width = (int)Math.Round(v);
		}
		private static float GetHeight(XElement e)
		{
			return e.Bounds.Height;
		}
		private static void SetHeight(XElement e, float v)
		{
			e.Bounds.Height = (int)Math.Round(v);
		}
		private static float GetRotation(XElement e)
		{
			return e.Rotation;
		}
		private static void SetRotation(XElement e, float v)
		{
			e.Rotation = v;
		}
		private static float GetCurrentFrame(XElement e)
		{
			return ((XSprite)e).CurrentFrame;
		}
		private static void SetCurrentFrame(XElement e, float v)
		{
			((XSprite)e).CurrentFrame = (int)Math.Round(v);
		}
		private static float GetOpacity(XElement e)
		{
			return e.Opacity;
		}
		private static void SetOpacity(XElement e, float v)
		{
			e.Opacity = v;
		}
	}
}

