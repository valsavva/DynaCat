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
	public class EventCaller : ActionCallerBase
	{
		private string evnt;
		private XObject target;
		private bool isInstant;

		public EventCaller(XObject caller, string memberDescriptor)
		{
			if (memberDescriptor.StartsWith("~"))
			{
				isInstant = false;
				memberDescriptor = memberDescriptor.Substring(1);
			}
			else
				isInstant = true;
			
			// ensure that the event target exists
			string action;
			caller.GetTargetFromDescriptor(memberDescriptor, out target, out action);
			
			this.evnt = memberDescriptor;
		}
		
		public override object Call()
		{
			if (target.Enabled)
				target.EnqueueEvent(evnt, isInstant);
			
			return null;
		}
	}
}

