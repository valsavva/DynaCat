using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using System.Reflection;

namespace Lunohod.Objects
{
	public abstract class ActionCallerBase
	{
		public abstract void Call();
	}
	
	public class ActionCaller : ActionCallerBase
	{
		private XObject target;
		private MethodInfo methodInfo;

		public ActionCaller(XObject target, string action)
		{
            this.target = target;
			
			Type type = this.target.GetType();
			this.methodInfo = type.GetMethod(action);
			
			if (this.methodInfo == null)
				throw new InvalidOperationException(
					string.Format("Could not find action [{0}] on object [{1}] of type [{2}]",
						action, this.target.Id, this.GetType().FullName)
				);
		}
		
		public override void Call()
		{
			methodInfo.Invoke(target, null);
		}
		
		public static ActionCallerBase CreateActionCaller(XObject trigger, string memberDescriptor)
		{
			if (string.IsNullOrEmpty(memberDescriptor))
				return null;
			
			XObject target;
			string actoin;
			trigger.GetTargetFromDescriptor(memberDescriptor, out target, out actoin);
			
			if (memberDescriptor.Contains(":"))
				return new EventCaller(target, actoin);
			else
				return new ActionCaller(target, actoin);
		}
	}
}

