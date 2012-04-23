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
			{
                target = currentObject.Parent;
				objectId = target.Id;
			}
			else
                target = currentObject.FindGlobal(objectId);

            if (target == null)
                throw new InvalidOperationException(string.Format("Could not find object with Id: [{0}]", objectId));
			
			var method = target.GetType().GetMethod(actionId);
			
			if (method == null)
                throw new InvalidOperationException(string.Format("Could not find method [{0}] on object [{1}]", actionId, objectId));
			
            var returnType = method.ReturnType;

            if (returnType == typeof(string))
			{
				Func<List<Expression>, string> func;
				target.GetMethod(actionId, out func);
                return new Method<string>(currentObject, objectId, actionId, func, parameters);
			}
            else if (returnType == typeof(bool))
			{
				Func<List<Expression>, bool> func;
				target.GetMethod(actionId, out func);
                return new Method<bool>(currentObject, objectId, actionId, func, parameters);
			}
            else if (returnType == typeof(void))
			{
				Action<List<Expression>> func;
				target.GetMethod(actionId, out func);
                return new Method<object>(currentObject, objectId, actionId, (ps) => { func(ps); return null; }, parameters);
			}
            else
			{
				Func<List<Expression>, double> func;
				target.GetMethod(actionId, out func);
                return new Method<double>(currentObject, objectId, actionId, func, parameters);
			}
        }
    }
}
