using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class UnaryNotOperator : Expression<bool>
    {
        private IExpression<bool> expression;

        public UnaryNotOperator(Expression expression)
        {
            this.expression = Validator.CheckType<IExpression<bool>>(expression);
        }

        public override bool GetValue() { return !expression.GetValue(); }

        public override string ToString()
        {
            return "!" + expression.ToString();
        }
    }
}
