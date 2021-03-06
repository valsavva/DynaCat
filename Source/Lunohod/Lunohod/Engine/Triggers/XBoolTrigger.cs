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
	[XmlType("BoolTrigger")]
	public class XBoolTrigger : XTriggerBase
	{
		private IExpression<bool> valueReader;
		
		[XmlAttribute]
		public string Condition;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			valueReader = Compiler.CompileExpression<bool>(this, this.Condition ?? "true");
		}
		
		public override bool IsTriggered()
		{
			return valueReader.GetValue();
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.Condition != null)
				this.Condition = this.Condition.Replace(par, val);

			base.ReplaceParameter(par, val);
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.Condition = reader["Condition"];

            base.ReadXml(reader);
        }
	}
}
