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
		private BoolValueReader valueReader;
		
		[XmlAttribute]
		public string Condition;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			valueReader = new BoolValueReader(this, this.Condition);
		}
		
		public override bool IsTriggered()
		{
			return valueReader.Value;
		}
	}
}
