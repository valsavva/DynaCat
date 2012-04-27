using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Xge;

namespace Lunohod.Xge
{
    public class AssignStatement<T> : IAction
    {
        private IAssignable<T> lExpression;
        private IExpression<T> rExpression;

        public AssignStatement(IAssignable<T> lExpression, IExpression<T> rExpression)
        {
            this.lExpression = lExpression;
            this.rExpression = rExpression;
			
			if (this.lExpression is Variable)
				((Variable)this.lExpression).SetType(rExpression.Type);
        }
        

        public void Call()
        {
			lExpression.SetValue(rExpression.GetValue());
        }

        public override string ToString()
        {
            return lExpression.ToString() + " = " + rExpression.ToString();
        }
    }
}
