using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public class UnaryMinusOperator : Expression<double>
    {
        private IExpression<double> expression;

        public UnaryMinusOperator(Expression expression)
        {
            this.expression = Validator.CheckType<IExpression<double>>(expression);
        }

        public override double GetValue() { return -this.expression.GetValue(); }

        public override string ToString()
        {
            return "-" + expression.ToString();
        }
    }
}
