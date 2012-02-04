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
    public class XDo : XObject, IRunnable
    {
        private List<ActionCallerBase> actionCallers;

        [XmlAttribute]
        public string Action;

        public override void Initialize(InitializeParameters p)
        {
            base.Initialize(p);

            actionCallers = this.Action.Split(';').Select(s => ActionCaller.CreateActionCaller(this, s)).ToList();
        }

        public bool InProgress
        {
            get { return false; }
            set { /* noop */ }
        }

        public void Start()
        {
            actionCallers.ForEach(a => a.Call());
        }

        public void Stop()
        {
        }

        public void Pause()
        {
        }

        public void Resume()
        {
        }
    }
}
