using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public abstract class Property<T> : Expression<T>
    {
        private string objectId;
        private string propertyId;
        protected PropertyAccessor accessor;

        public Property(XObject currentObject, string objectId, string propertyId)
        {
            this.objectId = objectId;
            this.propertyId = propertyId;

            accessor = new PropertyAccessor(currentObject, objectId + "." + propertyId);
        }

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + propertyId;
        }
    }

    public class NumProperty : Property<float>, INumExpression
    {
        public NumProperty(XObject currentObject, string objectId, string propertyId)
            : base(currentObject, objectId, propertyId)
        {

        }
        public override float Value { get { return accessor.FloatPropertyValue; } }
    }

    public class BoolProperty : Property<bool>, IBoolExpression
    {
        public BoolProperty(XObject currentObject, string objectId, string propertyId)
            : base(currentObject, objectId, propertyId)
        {

        }
        public override bool Value { get { return Convert.ToBoolean(accessor.PropertyValue); } }
    }
}
