using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Globalization;

namespace Lunohod.Objects
{
    public class StrValueReader
    {
        private string stringValue;
        private PropertyAccessor accessor;
		private ActionCallerBase actionCaller;
		private Func<string> func;

        public StrValueReader(XObject currentObject, string descriptor)
        {
			if (!descriptor.StartsWith("="))
			{
				stringValue = descriptor;
				func = ReturnStringValue;
			}
			else 
			{
				descriptor = descriptor.Substring(1);
				if (descriptor.Contains("("))
				{
					actionCaller = ActionCaller.CreateActionCaller(currentObject, descriptor);
					func = ReturnActionValue;
				}
				else
				{
	                this.accessor = new PropertyAccessor(currentObject, descriptor);
					func = this.ReturnAccessorValue;
				}
			}
        }
		
		private string ReturnAccessorValue()
		{
			var value = accessor.PropertyValue;
			return value == null ? "" : value.ToString();
		}

		private string ReturnActionValue()
		{
			var value = actionCaller.Call();
			return value == null ? "" : value.ToString();
		}
		
		private string ReturnStringValue()
		{
			return stringValue;
		}
		
        public string Value
        {
            get { return func(); }
        }
    }
}
