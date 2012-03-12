using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    [XmlType("NumTrigger")]
	public class XNumTrigger : XNumTriggerBase
	{
        private IExpression<float> value1Reader;
		
		[XmlAttribute]
		public string Property;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            value1Reader = Compiler.CompileExpression<float>(this, this.Property);
		}

		public override float GetValue1()
		{
			return value1Reader.GetValue();
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			this.Property = this.Property.Replace(par, val);
			if (this.Value != null)
				this.Value = this.Value.Replace(par, val);

			base.ReplaceParameter(par, val);
		}
	}
}

