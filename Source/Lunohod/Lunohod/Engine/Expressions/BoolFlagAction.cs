using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class BoolFlagAction : IAction
    {
        private XObject target;
        private bool isInstant;
        private string evnt;

        public BoolFlagAction(Objects.XObject currentObject, string objectId, string flagId, bool isInstant)
        {
            this.isInstant = isInstant;
            target = currentObject.FindGlobal(objectId);

            if (target == null)
                throw new InvalidOperationException(string.Format("Could not find object with Id: [{0}]", objectId));

            evnt = objectId + ":" + flagId;
        }

        public void Call()
        {
            if (target.Enabled)
                target.EnqueueEvent(evnt, isInstant);
        }

        public override string ToString()
        {
            return isInstant ? evnt : "~" + evnt;
        }
    }
}
