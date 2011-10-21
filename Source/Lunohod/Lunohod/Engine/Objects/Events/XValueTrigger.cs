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
	
    [XmlType("ValueTrigger")]
	public class XValueTrigger : XObject
	{
		private bool isTriggered = false;
		private PropertyAccessor propertyAccessor;
		
		private object objValue;
		private float floatValue;

		private ActionCaller action;
		private ActionCaller enterAction;
		private ActionCaller exitAction;
		
		private static readonly Dictionary<XValueComparison, Func<XValueTrigger, object, bool>> compareFuncs = new Dictionary<XValueComparison, Func<XValueTrigger, object, bool>>
		{
			{ XValueComparison.E, (t, o) => t.objValue.Equals(o) },
			{ XValueComparison.G, (t, o) => (float)o > t.floatValue },
			{ XValueComparison.GE, (t, o) => (float)o >= t.floatValue},
			{ XValueComparison.L, (t, o) => (float)o < t.floatValue },
			{ XValueComparison.LE, (t, o) => (float)o <= t.floatValue }
		};
			
		
		[XmlAttribute]
		public string Property;
		[XmlAttribute("Value")]
		public string ValueString;
		[XmlAttribute]
		public XValueComparison Compare = XValueComparison.E;
		[XmlAttribute]
		public string EnterAction;
		[XmlAttribute]
		public string ExitAction;
		[XmlAttribute]
		public string Action;
		
		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			propertyAccessor = PropertyAccessor.CreatePropertyAccessor(this, this.Property);
			
			if (propertyAccessor.PropertyType == typeof(bool))
				objValue = bool.Parse(this.ValueString);
			else if (propertyAccessor.PropertyType == typeof(float))
			{
				floatValue = float.Parse(this.ValueString);
				objValue = floatValue;
			}
			
			this.action = ActionCaller.CreateActionCaller(this, this.Action);
			this.enterAction = ActionCaller.CreateActionCaller(this, this.EnterAction);
			this.exitAction = ActionCaller.CreateActionCaller(this, this.ExitAction);
		}
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			var v = this.propertyAccessor.PropertyValue;
			
			bool oldIsTriggered = this.isTriggered;
			
			this.isTriggered = compareFuncs[this.Compare](this,v);
			
			if (this.isTriggered && !oldIsTriggered && enterAction != null)
				enterAction.Call();
			if (this.isTriggered && action != null)
				action.Call();
			if (!this.isTriggered && oldIsTriggered && exitAction != null)
				exitAction.Call();
		}
	}
}

