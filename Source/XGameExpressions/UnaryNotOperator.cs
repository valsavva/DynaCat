using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    class UnaryNotOperator : BoolExpression
    {
        private BoolExpression expression;

        public UnaryNotOperator(BoolExpression expression)
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
