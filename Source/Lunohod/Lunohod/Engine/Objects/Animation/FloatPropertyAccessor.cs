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
//	public class FloatPropertyAccessor : PropertyAccessor
//	{
//		private Func<float> getter;
//		private Action<float> setter;
//		
//		private XElement targetElement;
//		
//		public FloatPropertyAccessor(XObject target, string property, Func<float> getter, Action<float> setter)
//			:base(target, property)
//		{
//			targetElement = (XElement)target;
//			
//            this.getter = getter;
//			this.setter = setter;
//		}
//		
//		public override Type PropertyType
//		{
//			get {
//				return typeof(float);
//			}
//		}
//		
//        public float FloatPropertyValue
//        {
//            get { return this.getter(targetElement);  }
//            set { setter(targetElement, value);  }
//        }
//		
//		public override object PropertyValue
//		{
//			get {
//				return this.FloatPropertyValue;
//			}
//			set {
//				this.FloatPropertyValue = (float)value;
//			}
//		}
//	}
}

