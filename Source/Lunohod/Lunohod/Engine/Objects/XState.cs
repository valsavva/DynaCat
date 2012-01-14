using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;

namespace Lunohod.Objects
{
    [XmlType("State")]
	public class XState : XElement
	{
		private XObject target;
		private string evnt;

        private bool enabled;
		
		[XmlAttribute]
		public string When;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);
			
			if (!string.IsNullOrEmpty(this.When))
				this.GetTargetFromDescriptor(this.When, out target, out evnt);
		}
		
		public override void Update(UpdateParameters p)
		{
			if (this.enabled = EvaluateCondition())
				base.Update(p);
		}
		
		public override void Draw(DrawParameters p)
		{
			if (this.enabled)
				base.Draw(p);
		}
		
		private bool EvaluateCondition()
		{
			int ranIndex = this.Parent.Subcomponents.FindIndex(o => o is XState && ((XState)o).enabled);
			int thisIndex = this.Parent.Subcomponents.IndexOf(this);
			
			// a preceding state has already performed
			if (ranIndex != -1 && ranIndex < thisIndex)
				return false;
			
			// no condition
			if (target == null)
				return true;

			// see if the event was signaled
			return target.GetSignalContainer().IsSignaled(evnt);
		}
	}
}

