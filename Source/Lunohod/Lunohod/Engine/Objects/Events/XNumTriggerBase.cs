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
		protected Func<float, float, bool> compareFunc;
		
		private static readonly Dictionary<XValueComparison, Func<float, float, bool>> compareFuncs = new Dictionary<XValueComparison, Func<float, float, bool>>
		{
			{ XValueComparison.E, (v, cv) => v == cv },
			{ XValueComparison.G, (v, cv) => v > cv },
			{ XValueComparison.GE, (v, cv) => v >= cv },
			{ XValueComparison.L, (v, cv) => v < cv },
			{ XValueComparison.LE, (v, cv) => v <= cv }
		};
			
		[XmlAttribute("Value")]
		public float Value;
		[XmlAttribute]
		public XValueComparison Compare = XValueComparison.E;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			compareFunc = compareFuncs[this.Compare];
		}
		
		public abstract float GetNewValue();

		public override bool IsTriggered()
		{
			var v = GetNewValue();
			return compareFunc(v, this.Value);
		}
	}
}
