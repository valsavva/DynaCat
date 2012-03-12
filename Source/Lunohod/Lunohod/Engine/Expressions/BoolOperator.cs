using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class BoolOperator : Expression<bool>
    {
        private TokenType tokenType;
        private IExpression<bool> expression1;
        private IExpression<bool> expression2;

        public BoolOperator(TokenType tokenType, IExpression<bool> expression1, IExpression<bool> expression2)
        {
            this.tokenType = tokenType;
            this.expression1 = expression1;
            this.expression2 = expression2;
        }

        public override bool GetValue()
        {
            return tokenType == TokenType.And ? expression1.GetValue() && expression2.GetValue() : expression1.GetValue() || expression2.GetValue();
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", expression1, Scanner.CharTokenMapping.First(i => i.Value == tokenType).Key, expression2);
        }
    }
}
