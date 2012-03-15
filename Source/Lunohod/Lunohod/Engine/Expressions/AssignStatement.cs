using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Xge;

namespace Lunohod.Xge
{
    public class AssignStatement : IAction
    {
        private IAssignable lExpression;
        private Expression rExpression;

        public AssignStatement(IAssignable lExpression, Expression rExpression)
        {
            // TODO: Complete member initialization
            this.lExpression = lExpression;
            this.rExpression = rExpression;
        }
        

        public void Call()
        {
            if (rExpression.Type == typeof(float))
                DoAssign<float>();
            else if (rExpression.Type == typeof(bool))
                DoAssign<bool>();
            else if (rExpression.Type == typeof(string))
                DoAssign<string>();
            else
                throw new InvalidOperationException(string.Format("Expression '{0}' has unknown type '{1}'", rExpression, rExpression.Type.Name));
        }

        private void DoAssign<T>()
        {
            ((IAssignable<T>)lExpression).SetValue(((IExpression<T>)rExpression).GetValue());
        }

        public override string ToString()
        {
            return lExpression.ToString() + " = " + rExpression.ToString();
        }
    }
}
