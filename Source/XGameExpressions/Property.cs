using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nomnom.XGameExpressions
{
    public class Property<T> : Expression<T>
    {
        private string objectId;
        private string propertyId;

        public Property(string objectId, string propertyId)
        {
            this.objectId = objectId;
            this.propertyId = propertyId;
        }

        public override T Value { get { throw new NotImplementedException(); } }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + propertyId;
        }
    }

    public class NumProperty : Property<float>
    {
        public NumProperty(string objectId, string propertyId)
            : base(objectId, propertyId)
        {

        }
        public override float Value { get { return 0f; } }
    }

    public class BoolProperty : Property<bool>
    {
        public BoolProperty(string objectId, string propertyId)
            : base(objectId, propertyId)
        {

        }
        public override bool Value { get { return false; } }
    }
}
