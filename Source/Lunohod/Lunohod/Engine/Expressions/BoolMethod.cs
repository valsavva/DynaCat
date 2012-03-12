using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class BoolMethod : Method<bool>, IExpression<bool>
    {
        public BoolMethod(XObject currentObject, string objectId, string actionId, List<Expression> parameters)
            : base(currentObject, objectId, actionId, parameters)
        {
            throw new InvalidOperationException(
                string.Format("Unknown method: {0}.{1}", this.target.GetType().FullName, this.actionId)
            );
        }
        public override bool GetValue() { return false; }
    }
}
