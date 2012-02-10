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
		private NumValueReader value1Reader;
		
		[XmlAttribute]
		public string Property;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			value1Reader = new NumValueReader(this, this.Property);
		}

		public override float GetValue1()
		{
			return value1Reader.Value;
		}
		
		public override void ReplaceParameter(string par, string val)
		{
			this.Property = this.Property.Replace(par, val);
			if (this.Value != null)
				this.Value = this.Value.Replace(par, val);

			base.ReplaceParameter(par, val);
		}
	}
}

