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
		private ObjectProxy target;
		private string evnt;

		public EventCaller(XObject target, string evnt)
		{
			this.target = new ObjectProxy(target);
			this.evnt = evnt;
		}
		
		public override object Call()
		{
			target.Object.GetSignalContainer("events").Signal(evnt);
			
			return null;
		}
	}
}

