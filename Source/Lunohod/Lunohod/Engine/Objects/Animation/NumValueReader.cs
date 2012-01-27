using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Objects
{
    // Accepts a string containing a number or a numeric property.
    // Accordingly returns the numeric value or the property value.
    public class NumValueReader
    {
        private float floatValue;
        private PropertyAccessor accessor;

        public NumValueReader(XObject currentObject, string descriptor)
        {
            if (!float.TryParse(descriptor, out floatValue))
                this.accessor = new PropertyAccessor(currentObject, descriptor);
        }

        public float Value
        {
            get { return accessor == null ? floatValue : accessor.FloatPropertyValue; }
        }
    }
}
