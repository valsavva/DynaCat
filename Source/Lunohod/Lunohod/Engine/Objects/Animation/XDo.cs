using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;

namespace Lunohod.Objects
{
    /// <summary>
    /// Represents a "runnalbe" component that invokes actions on other components.
    /// </summary>
    [XmlType("Do")]
    public class XDo : XRunnableBase
    {
        private List<ActionCallerBase> actionCallers;
		
        /// <summary>
        /// Action specification.
        /// </summary>
        [XmlAttribute]
        public string Action;
		
        /// <exclude />
		public XDo()
		{
			this.RepeatCount = 1;
		}
		
        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            actionCallers = this.Action.Split(';').Select(s => ActionCaller.CreateActionCaller(this, s)).ToList();
        }

        internal override void UpdateProgress(UpdateParameters p)
		{
            actionCallers.ForEach(a => a.Call());
			this.repeatsDone++;
		}		
		
		internal override void ReplaceParameter(string par, string val)
		{
			if (this.Action != null)
				this.Action = this.Action.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
    }
}
