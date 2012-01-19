using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using System.Collections;

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

        private List<IRunnable> runnables;
        private bool[] runnableRunning;

        [XmlAttribute]
        public string When;
        [XmlAttribute]
        public string AlwaysOnce;
        [XmlAttribute]
        public bool ResetContent = true;

        public override void Initialize(InitializeParameters p)
		{
			base.Initialize(p);

            runnables = CollectRunnables();
            runnableRunning = runnables.Select(r => r.InProgress).ToArray();

            if (!string.IsNullOrEmpty(this.When))
                this.GetTargetFromDescriptor(this.When, out whenTarget, out whenEvnt);
            if (!string.IsNullOrEmpty(this.AlwaysOnce))
                this.GetTargetFromDescriptor(this.AlwaysOnce, out aonceTarget, out aonceEvnt);
        }
		
		public override void Update(UpdateParameters p)
		{
            bool oldEnabled = this.enabled;
                
            this.enabled = EvaluateCondition();

            if (this.enabled && !oldEnabled)
                OnEnter();

            if (!this.enabled && oldEnabled)
                OnExit();

            if (this.enabled)
                base.Update(p);
		}

        private void OnEnter()
        {
            if (this.ResetContent)
            {
                for (int i = 0; i < runnables.Count; i++)
                {
                    if (runnableRunning[i])
                        runnables[i].Start();
                }
            }
        }

        private void OnExit()
        {
            if (this.ResetContent)
            {
                for (int i = 0; i < runnables.Count; i++)
                {
                    runnables[i].Stop();
                }
            }
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

