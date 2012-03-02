using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Reflection;
using System.Diagnostics;

namespace Lunohod.Xge
{
    public abstract class Property<T> : Expression<T>, IAssignable<T>
    {
        protected string objectId;
        protected string propertyId;
        protected XObject target;

        protected PropertyInfo propertyInfo;

        protected Func<T> getter;
        protected Action<T> setter;

        public Property(XObject currentObject, string objectId, string propertyId)
        {
            this.objectId = objectId;
            this.propertyId = propertyId;

            if (objectId == null)
                target = currentObject;
            else
                target = currentObject.GetRoot().FindDescendant(objectId);

            if (target == null)
                throw new InvalidOperationException(string.Format("Could not find object with Id: [{0}]", objectId));
        }

        protected void InitializeGenericAccessor()
        {
            var targetType = target.GetType();
            this.propertyInfo = targetType.GetProperty(propertyId);

            if (this.propertyInfo == null)
                throw new InvalidOperationException(
                    string.Format("Could not find property [{0}] on object [{1}] of type [{2}])",
                        propertyId, this.target.Id, targetType.FullName)
                );

            Debug.WriteLine("!!!!! Using PropertyInfo for [{0}.{1}] !!!!!", targetType.FullName, propertyId);

            getter = this.GetConvertedValue;
            setter = this.SetConvertedValue;
        }

        public override T Value
        {
            get { return getter(); }
        }

        public void SetValue(T v)
        {
            setter(v);
        }

        private void SetConvertedValue(T v)
        {
            propertyInfo.SetValue(target, v, null);
        }

        private T GetConvertedValue()
        {
            return (T)propertyInfo.GetValue(target, null);
        }
        

        public override string ToString()
        {
            return (string.IsNullOrEmpty(objectId) ? "" : objectId + ".") + propertyId;
        }
    }
}
