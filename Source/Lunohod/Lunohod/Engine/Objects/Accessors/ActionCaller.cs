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
		public abstract object Call();
	}
	
	public class ActionCaller : ActionCallerBase
	{
		private XObject target;
		private MethodInfo methodInfo;
		private List<StrValueReader> parameters;
		private object[] parameterValues;

		public ActionCaller(XObject target, string action)
		{
            this.target = target;
			
			ParseParameters(ref action);
			
			Type type = this.target.GetType();
			this.methodInfo = type.GetMethod(action);
			
			if (this.methodInfo == null)
				throw new InvalidOperationException(
					string.Format("Could not find action [{0}] on object [{1}] of type [{2}]",
						action, this.target.Id, this.GetType().FullName)
				);
		}

		public void ParseParameters(ref string action)
		{
			int lpIndex = action.IndexOf("(");
			int rpIndex = action.IndexOf(")");
			
			if (lpIndex == -1 && rpIndex == -1)
				return;
			
			if ((lpIndex == -1 || rpIndex == -1) || (lpIndex > rpIndex))
				throw new InvalidOperationException("Wrong number of parenthesis");
			
			var paramsString = action.Substring(lpIndex + 1, rpIndex - lpIndex - 1);
			action = action.Substring(0, lpIndex);
			
			if (rpIndex == lpIndex + 1)
				return;
			
			parameters = paramsString.Split(',').Select(s => new StrValueReader(this.target, s)).ToList();
			parameterValues = new object[parameters.Count];
		}
		
		public override object Call()
		{
			if (parameters == null || parameters.Count == 0)
				return methodInfo.Invoke(target, null);
			else
			{
				for(int i = 0; i < parameters.Count; i++)
					parameterValues[i] = parameters[i].Value;
				return methodInfo.Invoke(target, parameterValues);
			}
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

