using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class StrConcatOperator : Expression<string>
    {
        private IExpression<string> expression1;
        private IExpression<string> expression2;

        public StrConcatOperator(IExpression<string> expression1, IExpression<string> expression2)
        {
            this.expression1 = expression1;
            this.expression2 = expression2;
        }

        public override string GetValue()
        {
            return expression1.GetValue() + expression2.GetValue();
        }

        public override string ToString()
        {
            return string.Format("({0} + {1})", expression1, expression2);
        }
    }
}
