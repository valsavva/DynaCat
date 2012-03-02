using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    class BoolBinaryOperator : BoolExpression
    {
        private TokenType tokenType;
        private BoolExpression expression1;
        private BoolExpression expression2;

        public BoolBinaryOperator(TokenType tokenType, BoolExpression expression1, BoolExpression expression2)
        {
            this.tokenType = tokenType;
            this.expression1 = expression1;
            this.expression2 = expression2;
        }

        public override bool Value
        {
            get { return tokenType == TokenType.And ? expression1.Value && expression2.Value : expression1.Value || expression2.Value; }
        }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", expression1, Scanner.CharTokenMapping.First(i => i.Value == tokenType).Key, expression2);
        }
    }
}
