using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using System.Reflection;
using System.Diagnostics;

namespace Lunohod.Objects
{
	public abstract class ActionCallerBase
	{
		public abstract object Call();
	}
	
	public class ActionCaller : ActionCallerBase
	{
		private ObjectProxy target;
		private Func<object[], object> method;
		private MethodInfo methodInfo;
		private List<StrValueReader> parameters;
		private object[] parameterValues;

		public ActionCaller(XObject target, string action)
		{
            this.target = new ObjectProxy(target);
			
			ParseParameters(ref action);
			
			method = ResolveMethod(action);
			
			if (method == null)
			{
				this.methodInfo = this.target.ObjectType.GetMethod(action);
				if (this.methodInfo != null)
					this.method = this.ActionInvokeMehodInfo;
			}
			
			if (method == null && methodInfo == null)
				throw new InvalidOperationException(
					string.Format("Could not find action [{0}] on object [{1}] of type [{2}]",
						action, this.target.ObjectId, this.target.ObjectType.FullName)
				);
		}

		public Func<object[], object> ResolveMethod(string action)
		{
			switch(action)
			{
				case "Start" : return this.ActionStart;
				case "Stop" : return this.ActionStop;
				case "Pause" : return this.ActionPause;
				case "Resume" : return this.ActionResume;
				
				case "GetLevelId" : return this.ActionGetLevelId;
				case "StartLevel" : return this.ActionStartLevel;
				case "StartScreen" : return this.ActionStartScreen;
				case "CloseCurrentScreen" : return this.ActionCloseCurrentScreen;
				case "EndCurrentLevel" : return this.ActionEndCurrentLevel;
	
				case "StartTransaction" : return this.ActionStartTransaction;
				case "EndTransaction" : return this.ActionEndTransaction;
				case "SetDirection" : return this.ActionSetDirection;

				case "Explode" : return this.ActionExplode;
				default:
				{
					Debug.WriteLine("Using reflection to access method: " + action);
					return null;
				}
			}
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
			
			parameters = paramsString.Split(',').Select(s => new StrValueReader(this.target.Object, s)).ToList();
			parameterValues = new object[parameters.Count];
		}
		
		public override object Call()
		{
			if (parameters == null || parameters.Count == 0)
				return method(null);
			else
			{
				for(int i = 0; i < parameters.Count; i++)
					parameterValues[i] = parameters[i].Value;
				return method(parameterValues);
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
		
		
		#region Well known actions
		
		// Generic
		public object ActionInvokeMehodInfo(object[] ps)
		{
			return methodInfo.Invoke(this.target.Object, ps);
		}
		
		// Runnables
		public object ActionStart(object[] ps)
		{
			((IRunnable)this.target.Object).Start();
			return null;
		}
		public object ActionStop(object[] ps)
		{
			((IRunnable)this.target.Object).Stop();
			return null;
		}
		public object ActionResume(object[] ps)
		{
			((IRunnable)this.target.Object).Resume();
			return null;
		}
		public object ActionPause(object[] ps)
		{
			((IRunnable)this.target.Object).Pause();
			return null;
		}
		
		// System
		public object ActionGetLevelId(object[] ps)
		{
			return ((XSystem)this.target.Object).GetLevelId((string)ps[0]);
		}
		public object ActionStartLevel(object[] ps)
		{
			((XSystem)this.target.Object).StartLevel((string)ps[0]);
			return null;
		}
		public object ActionStartScreen(object[] ps)
		{
			((XSystem)this.target.Object).StartScreen((string)ps[0]);
			return null;
		}
		public object ActionCloseCurrentScreen(object[] ps)
		{
			((XSystem)this.target.Object).CloseCurrentScreen();
			return null;
		}
		public object ActionEndCurrentLevel(object[] ps)
		{
			((XSystem)this.target.Object).EndCurrentLevel();
			return null;
		}
		
		// Hero
		public object ActionStartTransaction(object[] ps)
		{
			((XHero)this.target.Object).StartTransaction();
			return null;
		}
		public object ActionEndTransaction(object[] ps)
		{
			((XHero)this.target.Object).EndTransaction();
			return null;
		}
		public object ActionSetDirection(object[] ps)
		{
			((XHero)this.target.Object).SetDirection((string)ps[0],(string)ps[1]);
			return null;
		}
		public object ActionExplode(object[] ps)
		{
			((XExplosion)this.target.Object).Explode();
			return null;
		}
		#endregion
	}
}

