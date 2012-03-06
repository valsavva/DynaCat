using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class UnaryNotOperator : BoolExpression
    {
        private IBoolExpression expression;

        public UnaryNotOperator(Expression expression)
        {
            this.expression = Validator.CheckType<IBoolExpression>(expression);
        }

        public override bool GetValue() { return !expression.GetValue(); }

        public override string ToString()
        {
            return "!" + expression.ToString();
        }
    }
}
