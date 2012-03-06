using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class MethodFactory
    {
        public static Expression CreateMethod(XObject currentObject, string objectId, string actionId, List<Expression> parameters)
        {
            XObject target;

            if (objectId == null)
                target = currentObject.Parent;
            else
                target = currentObject.GetRoot().FindDescendant(objectId);

            if (target == null)
                throw new InvalidOperationException(string.Format("Could not find object with Id: [{0}]", objectId));

            var returnType = target.GetType().GetMethod(actionId).ReturnType;

            if (returnType == typeof(string))
                return new StrMethod(currentObject, objectId, actionId, parameters);
            else if (returnType == typeof(bool))
                return new BoolMethod(currentObject, objectId, actionId, parameters);
            else if (returnType == typeof(void))
                return new VoidMethod(currentObject, objectId, actionId, parameters);
            else
                return new NumMethod(currentObject, objectId, actionId, parameters);
        }
    }
}
