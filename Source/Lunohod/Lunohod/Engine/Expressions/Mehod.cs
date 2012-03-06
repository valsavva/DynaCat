using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public abstract class Method<T> : Expression<T>, IAction
    {
        protected string objectId;
        protected string actionId;
        protected List<Expression> parameters;
        protected XObject target;

        protected XSystem system;

        protected Func<T> action;

        public Method(XObject currentObject, string objectId, string actionId, List<Expression> parameters)
        {
            this.objectId = objectId;
            this.actionId = actionId;
            this.parameters = parameters;

            if (objectId == null)
                target = currentObject.Parent;
            else
                target = currentObject.GetRoot().FindDescendant(objectId);

            if (target == null)
                throw new InvalidOperationException(string.Format("Could not find object with Id: [{0}]", objectId));

            system = this.target as XSystem;
        }

        public void Call()
        {
            this.GetValue();
        }

        public override T GetValue() { return action(); }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + actionId + "(" + (parameters == null ? "" : string.Join(",", parameters)) + ")";
        }
    }
}
