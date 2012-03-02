using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    class CompareOperator : BoolExpression
    {
        private TokenType tokenType;
        private NumExpression expression1;
        private NumExpression expression2;

        private Func<NumExpression, NumExpression, bool> func;

        public CompareOperator(TokenType tokenType, NumExpression expression1, NumExpression expression2)
        {
            // TODO: Complete member initialization
            this.tokenType = tokenType;
            this.expression1 = expression1;
            this.expression2 = expression2;

            switch(tokenType)
            {
                case (TokenType.Op_E) : func = (e1, e2) => e1.Value == e2.Value; break;
                case (TokenType.Op_NE) : func = (e1, e2) => e1.Value != e2.Value; break;
                case (TokenType.Op_G) : func = (e1, e2) => e1.Value > e2.Value; break;
                case (TokenType.Op_GE) : func = (e1, e2) => e1.Value >= e2.Value; break;
                case (TokenType.Op_L) : func = (e1, e2) => e1.Value < e2.Value; break;
                case (TokenType.Op_LE): func = (e1, e2) => e1.Value <= e2.Value; break;
                default: throw new InvalidOperationException("WTF?");
            }
        }

        public override bool Value { get { return func(expression1, expression2); } }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", expression1, Scanner.CharTokenMapping.First(i => i.Value == tokenType).Key, expression2);
        }
    }
}
