using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class BoolBinaryOperator : BoolExpression
    {
        private TokenType tokenType;
        private IBoolExpression expression1;
        private IBoolExpression expression2;

        public BoolBinaryOperator(TokenType tokenType, IBoolExpression expression1, IBoolExpression expression2)
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
