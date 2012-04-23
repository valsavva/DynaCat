using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class Method<T> : Expression<T>, IAction
    {
        protected string objectId;
        protected string actionId;
        protected List<Expression> parameters;
        protected XObject target;

        protected Func<List<Expression>, T> action;

        public Method(XObject target, string objectId, string actionId, Func<List<Expression>, T> func, List<Expression> parameters)
        {
			this.target = target;
            this.objectId = objectId;
            this.actionId = actionId;
            this.parameters = parameters;
			this.action = func;
        }

        public void Call()
        {
            this.GetValue();
        }

        public override T GetValue() { return action(parameters); }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + actionId + "(" + (parameters == null ? "" : string.Join(",", parameters)) + ")";
        }
    }
}
