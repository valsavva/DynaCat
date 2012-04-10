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
                case "Str": action = this.ActionStr; break;
                case "GetLevelName": action = this.ActionGetLevelName; break;
                case "GetSeriesLevelName": action = this.ActionGetSeriesLevelName; break;
                case "GetSeriesName": action = this.ActionGetSeriesName; break;
                default:
                    throw new InvalidOperationException(
                        string.Format("Unknown method: {0}.{1}", this.target.GetType().FullName, this.actionId)
                    );
            }
        }
		
        // System
        public string ActionGetLevelName()
        {
            return system.GetLevelName((int)((IExpression<double>)parameters[0]).GetValue());
        }
        public string ActionGetSeriesLevelName()
        {
            return system.GetSeriesLevelName((int)((IExpression<double>)parameters[0]).GetValue(),(int)((IExpression<double>)parameters[1]).GetValue());
        }
        public string ActionStr()
        {
            return system.Str(parameters[0].GetObjValue());
        }
		public string ActionGetSeriesName()
		{
			return system.GetSeriesName((int)((IExpression<double>)parameters[0]).GetValue());
		}
    }
}
