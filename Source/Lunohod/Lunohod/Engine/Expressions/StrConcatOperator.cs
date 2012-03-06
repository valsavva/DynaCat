using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class StrConcatOperator : StrExpression
    {
        private IStrExpression expression1;
        private IStrExpression expression2;

        public StrConcatOperator(IStrExpression expression1, IStrExpression expression2)
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
            return string.Format("({0} + {2})", expression1, expression2);
        }
    }
}
