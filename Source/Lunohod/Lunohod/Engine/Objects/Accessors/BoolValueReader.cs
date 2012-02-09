using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Objects
{
	public class BoolValueReader
	{
        private bool boolValue = true;
        private PropertyAccessor accessor;
		private Func<bool> func;
		private bool not = false;
		
		private XObject eventContainerObject;
		private string eventName;

        public BoolValueReader(XObject currentObject, string descriptor)
        {
			if (descriptor != null && descriptor.StartsWith("!"))
			{
				descriptor = descriptor.Substring(1);
				not = true;
			}
			
            if (string.IsNullOrEmpty(descriptor) || bool.TryParse(descriptor, out boolValue))
			{
				func = this.ReturnBoolValue;
			}
			else if (descriptor.Contains(":"))
			{
				currentObject.GetTargetFromDescriptor(descriptor, out eventContainerObject, out eventName);
				func = this.ReturnEventSignaled;
			}
			else
			{
                this.accessor = new PropertyAccessor(currentObject, descriptor);
				func = this.ReturnAccessorValue;
			}
        }
		
		private bool ReturnEventSignaled()
		{
			return this.eventContainerObject.GetSignalContainer("events").IsSignaled(this.eventName);
		}
			
		private bool ReturnAccessorValue()
		{
			return (bool)accessor.PropertyValue;
		}
		
		private bool ReturnBoolValue()
		{
			return boolValue;
		}
		
        public bool Value
        {
            get { return not ^ func(); }
        }
	}
}

