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

        List<IRunnable> runnables;

        [XmlAttribute]
        public string When;
        [XmlAttribute]
        public string AlwaysOnce;
        [XmlAttribute]
        public bool ResetContent = true;

        public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            runnables = new List<IRunnable>();
            var descendants = this.GetAllDescendants();

            for (int i = 0; i < descendants.Count; i++)
            {
                IRunnable runnable = descendants[i] as IRunnable;

                if (runnable != null)
                    runnables.Add(runnable);
            }

            if (!string.IsNullOrEmpty(this.When))
                this.GetTargetFromDescriptor(this.When, out whenTarget, out whenEvnt);
            if (!string.IsNullOrEmpty(this.AlwaysOnce))
                this.GetTargetFromDescriptor(this.AlwaysOnce, out aonceTarget, out aonceEvnt);
        }
		
		public override void Update(UpdateParameters p)
		{
            bool oldEnabled = this.enabled;
                
            this.enabled = EvaluateCondition();

            if (!this.enabled && oldEnabled)
                DoResetContent();

            if (this.enabled)
                base.Update(p);
		}

        private void DoResetContent()
        {
            runnables.ForEach(r => r.Stop());
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

