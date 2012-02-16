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
    [XmlType("Do")]
    public class XDo : XRunnableBase
    {
        private List<ActionCallerBase> actionCallers;
		
        [XmlAttribute]
        public string Action;
		
		public XDo()
		{
			this.RepeatCount = 1;
		}
		
        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            actionCallers = this.Action.Split(';').Select(s => ActionCaller.CreateActionCaller(this, s)).ToList();
        }
		
		public override void UpdateProgress(UpdateParameters p)
		{
            actionCallers.ForEach(a => a.Call());
		}		
		
		public override void ReplaceParameter(string par, string val)
		{
			if (this.Action != null)
				this.Action = this.Action.Replace(par, val);
			
			base.ReplaceParameter(par, val);
		}
    }
}
