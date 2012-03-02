using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    public abstract class Function<T> : Expression<T>
    {
        private string objectId;
        private string propertyId;
        private List<Expression> parameters;

        public Function(string objectId, string propertyId, List<Expression> parameters)
        {
            this.objectId = objectId;
            this.propertyId = propertyId;
            this.parameters = parameters;
        }

        public override T Value { get { throw new NotImplementedException(); } }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + propertyId + "(" + (parameters == null ? "" : string.Join(",", parameters)) + ")";
        }
    }

    public class NumFunction : Function<float>
    {
        public NumFunction(string objectId, string propertyId, List<Expression> parameters)
            : base(objectId, propertyId, parameters)
        {

        }
        public override float Value { get { return 0f; } }
    }

    public class BoolFunction : Function<bool>
    {
        public BoolFunction(string objectId, string propertyId, List<Expression> parameters)
            : base(objectId, propertyId, parameters)
        {

        }
        public override bool Value { get { return false; } }
    }
}
