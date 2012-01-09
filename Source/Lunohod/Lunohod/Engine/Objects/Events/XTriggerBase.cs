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
	public abstract class XTriggerBase : XObject
	{
		private bool isTriggered;
			
		private ActionCaller action;
		private ActionCaller enterAction;
		private ActionCaller exitAction;
		
		[XmlAttribute]
		public string EnterAction;
		[XmlAttribute]
		public string ExitAction;
		[XmlAttribute]
		public string Action;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

			this.action = ActionCaller.CreateActionCaller(this, this.Action);
			this.enterAction = ActionCaller.CreateActionCaller(this, this.EnterAction);
			this.exitAction = ActionCaller.CreateActionCaller(this, this.ExitAction);
		}
		
		public abstract bool IsTriggered();
		
		public override void Update(UpdateParameters p)
		{
			base.Update(p);
			
			bool oldIsTriggered = this.isTriggered;
			this.isTriggered = this.IsTriggered();
			
			if (this.isTriggered && !oldIsTriggered && enterAction != null)
				enterAction.Call();
			if (this.isTriggered && action != null)
				action.Call();
			if (!this.isTriggered && oldIsTriggered && exitAction != null)
				exitAction.Call();
		}
		
	}
}

