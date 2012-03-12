using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class StrMethod : Method<string>, IExpression<string>
    {
        public StrMethod(XObject currentObject, string objectId, string actionId, List<Expression> parameters)
            : base(currentObject, objectId, actionId, parameters)
        {
            InitializeAction();
        }
        
        private void InitializeAction()
        {
            switch (actionId)
            {
                case "GetLevelId": action = this.ActionGetLevelId; break;
                case "Str": action = this.ActionStr; break;
                default:
                    throw new InvalidOperationException(
                        string.Format("Unknown method: {0}.{1}", this.target.GetType().FullName, this.actionId)
                    );
            }
        }

        // System
        public string ActionGetLevelId()
        {
            return system.GetLevelId((int)((IExpression<float>)parameters[0]).GetValue());
        }
        public string ActionStr()
        {
            return system.Str(parameters[0].GetObjValue());
        }
    }
}
