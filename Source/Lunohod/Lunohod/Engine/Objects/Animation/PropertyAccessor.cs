using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using System.Reflection;

namespace Lunohod.Objects
{
	public class PropertyAccessor
	{
		protected XObject target;
		private string property;
		private PropertyInfo propertyInfo;
		private Type targetType;

		protected PropertyAccessor(XObject target, string property)
		{
            this.target = target;
            this.property = property;

			targetType = target.GetType();
			this.propertyInfo = targetType.GetProperty(property);
			
			if (this.GetType() == typeof(PropertyAccessor) && this.propertyInfo == null)
				throw new InvalidOperationException(
					string.Format("Could not find property [{0}] on object [{1}] of type [{2}])",
						property, this.target.Id, targetType.FullName)
				);
		}
		
		public XObject Target
		{
			get { return this.target; }
		}	

		public Type TargetType
		{
			get { return this.targetType; }
		}

		public string Property
		{
			get { return this.property; }
		}
		
		public virtual Type PropertyType
		{
			get { return this.propertyInfo.PropertyType; }
		}
		
        public virtual object PropertyValue
        {
            get { return propertyInfo.GetValue(target, null);  }
            set { 
				try
				{
					propertyInfo.SetValue(target, value, null);
				}
				catch
				{
					var newValue = Convert.ChangeType(value, propertyInfo.PropertyType);
					propertyInfo.SetValue(target, newValue, null);
				}
			}
        }

		public static PropertyAccessor CreatePropertyAccessor(XObject currentObject, string memberDescriptor)
		{
			XObject target;
			string property;
			currentObject.GetTargetFromDescriptor(memberDescriptor, out target, out property);
			
			switch(property)
			{
				case "X": return new FloatPropertyAccessor(target, property, GetX, SetX);
				case "Y": return new FloatPropertyAccessor(target, property, GetY, SetY);
				case "Height": return new FloatPropertyAccessor(target, property, GetHeight, SetHeight);
				case "Width": return new FloatPropertyAccessor(target, property, GetWidth, SetWidth);
				case "Rotation": return new FloatPropertyAccessor(target, property, GetRotation, SetRotation);
				case "Opacity": return new FloatPropertyAccessor(target, property, GetOpacity, SetOpacity);
				case "CurrentFrame": return new FloatPropertyAccessor(target, property, GetCurrentFrame, SetCurrentFrame);
				default: return new PropertyAccessor(target, property);
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

