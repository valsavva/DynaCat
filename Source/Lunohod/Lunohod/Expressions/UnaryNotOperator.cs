using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class UnaryNotOperator : BoolExpression
    {
        private IBoolExpression expression;

        public UnaryNotOperator(IBoolExpression expression)
        {
            this.expression = expression;
        }

        public override bool Value { get { return !expression.Value; } }

        public override string ToString()
        {
            return "!" + expression.ToString();
        }
    }
}
