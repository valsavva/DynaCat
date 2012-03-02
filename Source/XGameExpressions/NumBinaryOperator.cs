using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    public class NumBinaryOperator : NumExpression
    {
        private TokenType tokenType;
        private NumExpression expression1;
        private NumExpression expression2;

        private Func<NumExpression, NumExpression, float> func;

        public NumBinaryOperator(TokenType tokenType, NumExpression expression1, NumExpression expression2)
        {
            this.tokenType = tokenType;
            this.expression1 = expression1;
            this.expression2 = expression2;

            switch (this.tokenType)
            {
                case TokenType.Plus: func = (e1, e2) => e1.Value + e2.Value; break;
                case TokenType.Minus: func = (e1, e2) => e1.Value - e2.Value; break;
                case TokenType.Multiply: func = (e1, e2) => e1.Value * e2.Value; break;
                case TokenType.Divide: func = (e1, e2) => e1.Value / e2.Value; break;
                case TokenType.Modulo: func = (e1, e2) => e1.Value % e2.Value; break;
                default: throw new InvalidOperationException("WTF?");
            }
        }

        public override float Value { get { return func(expression1, expression2); } }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", expression1, Scanner.CharTokenMapping.First(i => i.Value == tokenType).Key, expression2);
        }
    }
}
