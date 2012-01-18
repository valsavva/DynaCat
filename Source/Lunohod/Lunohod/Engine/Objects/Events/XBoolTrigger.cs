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
	[XmlType("BoolTrigger")]
	public class XBoolTrigger : XTriggerBase
	{
		private PropertyAccessor propertyAccessor;
        private bool not = false;
		
		[XmlAttribute]
		public string Property;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            string property = this.Property;

            if (property.StartsWith("!"))
            {
                property = property.Substring(1);
                not = true;
            }

			propertyAccessor = new PropertyAccessor(this, property);
		}
		
		public override bool IsTriggered()
		{
			return not ^ (bool)propertyAccessor.PropertyValue;
		}
	}
}
