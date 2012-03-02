using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    public class UnaryMinusOperator : NumExpression
    {
        private NumExpression expression;

        public UnaryMinusOperator(NumExpression expression)
        {
            this.expression = expression;
        }

        public override float Value { get { return -this.expression.Value; } }

        public override string ToString()
        {
            return "-" + expression.ToString();
        }
    }
}
