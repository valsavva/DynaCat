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
	public abstract class XTriggerBase : XObject
	{
		private bool isTriggered;
			
		private List<IAction> actions;
        private List<IAction> enterActions;
        private List<IAction> exitActions;
		
		[XmlAttribute]
		public string EnterAction;
		[XmlAttribute]
		public string ExitAction;
		[XmlAttribute]
		public string Action;
		[XmlAttribute]
		public string Group;
		[XmlAttribute]
		public bool StayTriggered;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			if (this.Action != null)
				this.actions = Compiler.CompileStatements(this, this.Action);
			if (this.EnterAction != null)
                this.enterActions = Compiler.CompileStatements(this, this.EnterAction);
			if (this.ExitAction != null)
                this.exitActions = Compiler.CompileStatements(this, this.ExitAction);
		}
		
		public abstract bool IsTriggered();
		
		public override void Update(UpdateParameters p)
		{
			bool oldIsTriggered = this.isTriggered;
			
			if (oldIsTriggered && this.StayTriggered)
			{
				// once we're triggered, we stay triggered
			}
			// if we belong to a group of triggers and one of them has already signaled - get out
			else if (this.Group != null
			         && this.Parent.GetTriggerGroups().ContainsKey(this.Group)
			          && this.Parent.GetTriggerGroups()[this.Group] == p.Game.CycleNumber)
			{
				this.isTriggered = false;
			}
			else
			{
				// get new state
				this.isTriggered = this.IsTriggered();
			}
			
			// if triggered and belong to a group - need to signal it for others in the group
			if (this.isTriggered && this.Group != null)
				this.Parent.GetTriggerGroups()[this.Group] = p.Game.CycleNumber;
			
			// fire appropriate actions
			if (this.isTriggered && !oldIsTriggered && enterActions != null)
				enterActions.ForEach(a => a.Call());
			if (this.isTriggered && actions != null)
				actions.ForEach(a => a.Call());
			if (!this.isTriggered && oldIsTriggered && exitActions != null)
				exitActions.ForEach(a => a.Call());

			if (this.isTriggered)
				base.Update(p);
		}
		
		public override void Draw(DrawParameters p)
		{
			if (this.isTriggered)
				base.Draw(p);
		}
		
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.EnterAction != null)
				this.EnterAction = this.EnterAction.Replace(par, val);
			if (this.Action != null)
				this.Action = this.Action.Replace(par, val);
			if (this.ExitAction != null)
				this.ExitAction = this.ExitAction.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}

        public override void ReadXml(System.Xml.XmlReader reader)
        {
            this.EnterAction = reader["EnterAction"];
            this.ExitAction = reader["ExitAction"];
            this.Action = reader["Action"];
            this.Group = reader["Group"];
            this.StayTriggered = reader.ReadAttrAsBoolean("StayTriggered");

            base.ReadXml(reader);
        }
	}
}
