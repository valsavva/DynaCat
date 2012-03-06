using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Globalization;
using Microsoft.Xna.Framework;
using Lunohod;
using Lunohod.Xge;

namespace Lunohod.Objects
{
    /// <summary>
    /// A "runnalbe" component that invokes actions on other components.
    /// </summary>
    [XmlType("Do")]
    public class XDo : XRunnableBase
    {
        private List<IAction> actions;
		
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

            actions = Compiler.CompileStatementList(this, this.Action);
        }

        internal override void UpdateProgress(UpdateParameters p)
		{
            actions.ForEach(a => a.Call());
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
