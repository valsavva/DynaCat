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
	public abstract class PropertyAccessorBase
	{
		private XElement target;
		private string property;

		public PropertyAccessorBase(XElement target, string property)
		{
			this.target = target;
			this.property = property;
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
		
		public static PropertyAccessorBase CreatePropertyAccessor(XElement target, string property)
		{
			switch(property)
			{
				case "X": return new PropertyAccessor<int>(target, property, GetX, SetX);
				case "Y": return new PropertyAccessor<int>(target, property, GetY, SetY);
				case "Height": return new PropertyAccessor<int>(target, property, GetHeight, SetHeight);
				case "Width": return new PropertyAccessor<int>(target, property, GetWidth, SetWidth);
				case "Rotation": return new PropertyAccessor<float>(target, property, GetRotation, SetRotation);
				case "Opacity": return new PropertyAccessor<float>(target, property, GetOpacity, SetOpacity);
				case "CurrentFrame": return new PropertyAccessor<int>(target, property, GetCurrentFrame, SetCurrentFrame);
				default: throw new InvalidOperationException("Unknown attribute");
			}
		}
					
		private static int GetX(XElement e)
		{
			return e.Bounds.X;
		}
		private static void SetX(XElement e, int v)
		{
			e.Bounds.X = v;
		}
		private static int GetY(XElement e)
		{
			return e.Bounds.Y;
		}
		private static void SetY(XElement e, int v)
		{
			e.Bounds.Y = v;
		}
		private static int GetWidth(XElement e)
		{
			return e.Bounds.Width;
		}
		private static void SetWidth(XElement e, int v)
		{
			e.Bounds.Width = v;
		}
		private static int GetHeight(XElement e)
		{
			return e.Bounds.Height;
		}
		private static void SetHeight(XElement e, int v)
		{
			e.Bounds.Height = v;
		}
		private static float GetRotation(XElement e)
		{
			return e.Rotation;
		}
		private static void SetRotation(XElement e, float v)
		{
			e.Rotation = v;
		}
		private static int GetCurrentFrame(XElement e)
		{
			return ((XSprite)e).CurrentFrame;
		}
		private static void SetCurrentFrame(XElement e, int v)
		{
			((XSprite)e).CurrentFrame = v;
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
	
	public class PropertyAccessor<T> : PropertyAccessorBase
	{
		private Func<XElement,T> getter;
		private Action<XElement,T> setter;

		public PropertyAccessor(XElement target, string property, Func<XElement,T> getter, Action<XElement,T> setter)
			:base(target, property)
		{
			this.getter = getter;
			this.setter = setter;
		}
		
		public T PropertyValue
		{
			get { return getter(this.Target); }
			set { setter(this.Target, value); }
		}
	}
}

