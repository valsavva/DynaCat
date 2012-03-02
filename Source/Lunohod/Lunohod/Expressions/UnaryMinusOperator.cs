using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public class UnaryMinusOperator : NumExpression
    {
        private INumExpression expression;

        public UnaryMinusOperator(INumExpression expression)
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
