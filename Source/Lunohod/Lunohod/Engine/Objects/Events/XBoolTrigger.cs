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
		
		[XmlAttribute]
		public bool Not;
		[XmlAttribute]
		public string Property;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			propertyAccessor = new PropertyAccessor(this, this.Property);
		}
		
		public override bool IsTriggered()
		{
			return !this.Not && (bool)propertyAccessor.PropertyValue;
		}
	}
}
