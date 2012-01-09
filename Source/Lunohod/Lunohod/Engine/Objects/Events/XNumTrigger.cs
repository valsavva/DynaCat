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
    [XmlType("NumTrigger")]
	public class XNumTrigger : XNumTriggerBase
	{
		private PropertyAccessor propertyAccessor;
		
		[XmlAttribute]
		public string Property;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			propertyAccessor = new PropertyAccessor(this, this.Property);
		}

		public override float GetNewValue()
		{
			return propertyAccessor.FloatPropertyValue;
		}
	}
}

