using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public abstract class Function<T> : Expression<T>
    {
        private string objectId;
        private string propertyId;
        private List<Expression> parameters;
        private XObject targetObject;

        public Function(XObject currentObject, string objectId, string propertyId, List<Expression> parameters)
        {
            this.objectId = objectId;
            this.propertyId = propertyId;
            this.parameters = parameters;
            this.targetObject = currentObject;
        }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + propertyId + "(" + (parameters == null ? "" : string.Join(",", parameters)) + ")";
        }
    }

    public class NumFunction : Function<float>, INumExpression
    {
        public NumFunction(XObject currentObject, string objectId, string propertyId, List<Expression> parameters)
            : base(currentObject, objectId, propertyId, parameters)
        {

        }
        public override float Value { get { return 0f; } }
    }

    public class BoolFunction : Function<bool>, IBoolExpression
    {
        public BoolFunction(XObject currentObject, string objectId, string propertyId, List<Expression> parameters)
            : base(currentObject, objectId, propertyId, parameters)
        {

        }
        public override bool Value { get { return false; } }
    }
}
