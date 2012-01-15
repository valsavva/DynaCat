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
        private XObject whenTarget;
        private string whenEvnt;
        private XObject aonceTarget;
        private string aonceEvnt;

        private bool enabled;
        private bool alwaysEnabled;

        [XmlAttribute]
        public string When;
        [XmlAttribute]
        public string AlwaysOnce;

		public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            if (!string.IsNullOrEmpty(this.When))
                this.GetTargetFromDescriptor(this.When, out whenTarget, out whenEvnt);
            if (!string.IsNullOrEmpty(this.AlwaysOnce))
                this.GetTargetFromDescriptor(this.AlwaysOnce, out aonceTarget, out aonceEvnt);
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

            if (alwaysEnabled)
                return true;

            if (aonceTarget != null)
            {
                if (alwaysEnabled = aonceTarget.GetSignalContainer().IsSignaled(aonceEvnt))
                    return true;
            }

			// no condition
            if (aonceTarget == null && whenTarget == null)
				return true;

			// see if the event was signaled
            return (whenTarget != null) && whenTarget.GetSignalContainer().IsSignaled(whenEvnt);
		}
	}
}

