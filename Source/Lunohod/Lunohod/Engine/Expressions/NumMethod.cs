using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class NumMethod : Method<float>, IExpression<float>
    {
        public NumMethod(XObject currentObject, string objectId, string propertyId, List<Expression> parameters)
            : base(currentObject, objectId, propertyId, parameters)
        {
            InitializeAction();
        }
        private void InitializeAction()
        {
            switch (actionId)
            {
                case "Rnd": action = this.ActionRnd; break;
                case "RndX": action = this.ActionRndX; break;
                case "RndY": action = this.ActionRndY; break;
                default:
                    throw new InvalidOperationException(
                        string.Format("Unknown method: {0}.{1}", this.target.GetType().FullName, this.actionId)
                    );
            }
        }
        private float ActionRnd()
        {
            return system.Rnd(((IExpression<float>)parameters[0]).GetValue(), ((IExpression<float>)parameters[1]).GetValue());
        }
        private float ActionRndX()
        {
            return system.RndX(((IExpression<float>)parameters[0]).GetValue(), ((IExpression<float>)parameters[1]).GetValue());
        }
        private float ActionRndY()
        {
            return system.RndY(((IExpression<float>)parameters[0]).GetValue(), ((IExpression<float>)parameters[1]).GetValue());
        }
    }
}
