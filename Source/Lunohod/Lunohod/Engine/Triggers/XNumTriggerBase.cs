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
	public enum XValueComparison
	{
		E,
		G,
		GE,
		L,
		LE
	}
	
	public abstract class XNumTriggerBase : XTriggerBase
	{
		protected IExpression<double> value2Reader;
		protected Func<double, double, bool> compareFunc;
		
		private static readonly Dictionary<XValueComparison, Func<double, double, bool>> compareFuncs = new Dictionary<XValueComparison, Func<double, double, bool>>
		{
			{ XValueComparison.E, (v, cv) => v == cv },
			{ XValueComparison.G, (v, cv) => v > cv },
			{ XValueComparison.GE, (v, cv) => v >= cv },
			{ XValueComparison.L, (v, cv) => v < cv },
			{ XValueComparison.LE, (v, cv) => v <= cv }
		};
			
		[XmlAttribute("Value")]
		public string Value;
		[XmlAttribute]
		public XValueComparison Compare = XValueComparison.E;
		
        public XNumTriggerBase()
        {
            this.Value = "0";
        }

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            if (this.Value == "0")
                value2Reader = NumConstant.Zero;
            else
			    value2Reader = Compiler.CompileExpression<double>(this, this.Value);

			compareFunc = compareFuncs[this.Compare];
		}
		
		public abstract double GetValue1();

		public virtual double GetValue2()
		{
			return value2Reader.GetValue();
		}

		public override bool IsTriggered()
		{
			return compareFunc(GetValue1(), GetValue2());
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            reader.ReadAttrAsString("Value", ref this.Value);
            reader.ReadAttrAsEnum<XValueComparison>("Compare", ref this.Compare);

            base.ReadXml(reader);
        }
	}
}
