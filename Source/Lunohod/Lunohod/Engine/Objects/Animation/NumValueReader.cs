using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Globalization;

namespace Lunohod.Objects
{
    // Accepts a string containing a number or a numeric property.
    // Accordingly returns the numeric value or the property value.
    public class NumValueReader
    {
        private float floatValue;
        private PropertyAccessor accessor;
		private Func<float> func;

        public NumValueReader(XObject currentObject, string descriptor)
        {
            if (!string.IsNullOrEmpty(descriptor) && !float.TryParse(descriptor, NumberStyles.Number, CultureInfo.InvariantCulture, out floatValue))
			{
                this.accessor = new PropertyAccessor(currentObject, descriptor);
				func = this.ReturnAccessorValue;
			}
			else
			{
				func = this.ReturnFloatValue;
			}
        }
		
		private float ReturnAccessorValue()
		{
			return accessor.FloatPropertyValue;
		}
		
		private float ReturnFloatValue()
		{
			return floatValue;
		}
		
        public float Value
        {
            get { return func(); }
        }
    }
}
