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
				case "X": return new PropertyAccessor<int>(target, property, GetWidth, SetWidth);
				default: throw new InvalidOperationException("WTF?");
			}
		}
					
		private static int GetWidth(XElement e)
		{
			return e.Bounds.X;
		}
		
		private static void SetWidth(XElement e, int v)
		{
			e.Bounds.X = v;
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

