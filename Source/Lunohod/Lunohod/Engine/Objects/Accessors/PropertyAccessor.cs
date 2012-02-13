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
		
		private XElement element;
		private XSprite sprite;
		private IHasVolume audio;
		
		private string property;
		private PropertyInfo propertyInfo;
		private Type targetType;
		private Type propertyType;

		private Func<float> floatGetter;
		private Action<float> floatSetter;
		
		public PropertyAccessor(XObject currentObject, string memberDescriptor)
		{
			this.target = null;
            this.property = null;

			currentObject.GetTargetFromDescriptor(memberDescriptor, out this.target, out this.property);
			
			this.targetType = target.GetType();
			this.propertyInfo = targetType.GetProperty(property);

			this.element = target as XElement;
			this.sprite = target as XSprite;
			this.audio = target as IHasVolume;
			
			this.propertyType = typeof(float);
			
			switch(property)
			{
				case "X": floatGetter = GetX; floatSetter = SetX; break;
				case "Y": floatGetter = GetY; floatSetter = SetY; break;
				case "Height": floatGetter = GetHeight; floatSetter = SetHeight; break;
				case "Width": floatGetter = GetWidth; floatSetter = SetWidth; break;
				case "Rotation": floatGetter = GetRotation; floatSetter = SetRotation; break;
				case "Scale": floatGetter = GetScale; floatSetter = SetScale; break;
				case "Opacity": floatGetter = GetOpacity; floatSetter = SetOpacity; break;
				case "CurrentFrame": floatGetter = GetCurrentFrame; floatSetter = SetCurrentFrame; break;
				case "Volume": floatGetter = GetVolume; floatSetter = SetVolume; break;
				default :
				{
					if (this.propertyInfo == null)
						throw new InvalidOperationException(
							string.Format("Could not find property [{0}] on object [{1}] of type [{2}])",
								property, this.target.Id, targetType.FullName)
						);
				
					propertyType = propertyInfo.PropertyType;
				
				}; break;
			}
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
			get { return propertyType; }
		}
		
        public virtual object PropertyValue
        {
            get {
				if (floatGetter != null)
					return floatGetter();
				
				return propertyInfo.GetValue(target, null);
			}
            set {
				
				if (floatSetter != null)
				{
					floatSetter((float)value);
					return;
				}
				
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
		
		public virtual float FloatPropertyValue
		{
			get { return this.floatGetter(); }
			set { this.floatSetter(value); }
		}
		
		#region numeric Getters/Setters
		private float GetX()
		{
			return element.Bounds.X;
		}
		private void SetX(float v)
		{
			element.Bounds.X = (int)Math.Round(v);
		}
		private float GetY()
		{
			return element.Bounds.Y;
		}
		private void SetY(float v)
		{
			element.Bounds.Y = (int)Math.Round(v);
		}
		private float GetWidth()
		{
			return element.Bounds.Width;
		}
		private void SetWidth(float v)
		{
			element.Bounds.Width = (int)Math.Round(v);
		}
		private float GetHeight()
		{
			return element.Bounds.Height;
		}
		private void SetHeight(float v)
		{
			element.Bounds.Height = (int)Math.Round(v);
		}
		private float GetRotation()
		{
			return element.Rotation;
		}
		private void SetRotation(float v)
		{
			element.Rotation = v;
		}
		private float GetCurrentFrame()
		{
			return sprite.CurrentFrame;
		}
		private void SetCurrentFrame(float v)
		{
			sprite.CurrentFrame = (int)Math.Round(v);
		}
		private float GetOpacity()
		{
			return element.Opacity;
		}
		private void SetOpacity(float v)
		{
			element.Opacity = v;
		}
		private float GetScale()
		{
			return element.Scale;
		}
		private void SetScale(float v)
		{
			element.Scale = v;
		}
		private float GetVolume()
		{
			return audio.Volume;
		}
		private void SetVolume(float v)
		{
			audio.Volume = v;
		}
		#endregion
	}
}

