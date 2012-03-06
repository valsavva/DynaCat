using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Reflection;
using System.Diagnostics;

namespace Lunohod.Xge
{
    public class PropertyFactory
    {
        public static Expression CreateProperty(XObject currentObject, string objectId, string propertyId)
        {
            XObject target;

            if (objectId == null)
                target = currentObject.Parent;
            else
                target = currentObject.GetRoot().FindDescendant(objectId);

            if (target == null)
                throw new InvalidOperationException(string.Format("Could not find object with Id: [{0}]", objectId));

            Type returnType = null;
            
            var propertyInfo = target.GetType().GetProperty(propertyId);

            if (propertyInfo != null)
                returnType = propertyInfo.PropertyType;
            else
            {
                var fieldInfo = target.GetType().GetField(propertyId);
                if (fieldInfo != null)
                    returnType = fieldInfo.FieldType;
                else
                    throw new InvalidOperationException(
                        string.Format("Could not find property [{0}] on object [{1}] of type [{2}])",
                            propertyId, target.Id, target.GetType().FullName)
                    );
            }

            if (returnType == typeof(string))
                throw new InvalidOperationException("No String properties");
            else if (returnType == typeof(bool))
                return new BoolProperty(currentObject, objectId, propertyId);
            else
                return new NumProperty(currentObject, objectId, propertyId);
        }
    }
}