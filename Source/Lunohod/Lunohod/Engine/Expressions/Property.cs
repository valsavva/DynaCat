using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Reflection;
using System.Diagnostics;

namespace Lunohod.Xge
{
    public interface IProperty
    {
        string Id { get; }
    }

    public class Property<T> : Expression<T>, IProperty, IAssignable<T>
    {
        protected string objectId;
        protected string propertyId;
        protected XObject target;

        protected PropertyInfo propertyInfo;

        protected Func<T> getter;
        protected Action<T> setter;

        public Property(XObject currentObject, string objectId, string propertyId, Func<T> getter, Action<T> setter)
        {
            this.objectId = objectId;
            this.propertyId = propertyId;
            this.Id = propertyId;
			this.getter = getter;
			this.setter = setter;
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

        public string Id { get; private set; }

        public override T GetValue()
        {
            return getter();
        }

        public void SetValue(T v)
        {
			if (setter != null)
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
            return string.IsNullOrEmpty(objectId) ? propertyId : objectId + "." + propertyId;
        }
    }
}
