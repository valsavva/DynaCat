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
				// verifying event object exists
				currentObject.GetTargetFromDescriptor(descriptor, out currentObject, out eventName);
				
				eventName = descriptor;
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
			return GameEngine.Instance.ScreenEngine.CurrentEvents.ContainsKey(this.eventName);
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

