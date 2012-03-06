using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public class UnaryMinusOperator : NumExpression
    {
        private INumExpression expression;

        public UnaryMinusOperator(Expression expression)
        {
            this.expression = Validator.CheckType<INumExpression>(expression);
        }

        public override float GetValue() { return -this.expression.GetValue(); }

        public override string ToString()
        {
            return "-" + expression.ToString();
        }
    }
}
