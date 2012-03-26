using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class CompareOperator : Expression<bool>
    {
        private TokenType tokenType;
        private IExpression<double> expression1;
        private IExpression<double> expression2;

        private Func<IExpression<double>, IExpression<double>, bool> func;

        public CompareOperator(TokenType tokenType, IExpression<double> expression1, IExpression<double> expression2)
        {
            // TODO: Complete member initialization
            this.tokenType = tokenType;
            this.expression1 = expression1;
            this.expression2 = expression2;

            switch(tokenType)
            {
                case (TokenType.Op_E) : func = (e1, e2) => e1.GetValue() == e2.GetValue(); break;
                case (TokenType.Op_NE) : func = (e1, e2) => e1.GetValue() != e2.GetValue(); break;
                case (TokenType.Op_G) : func = (e1, e2) => e1.GetValue() > e2.GetValue(); break;
                case (TokenType.Op_GE) : func = (e1, e2) => e1.GetValue() >= e2.GetValue(); break;
                case (TokenType.Op_L) : func = (e1, e2) => e1.GetValue() < e2.GetValue(); break;
                case (TokenType.Op_LE): func = (e1, e2) => e1.GetValue() <= e2.GetValue(); break;
                default: throw new InvalidOperationException("WTF?");
            }
        }

        public override bool GetValue() { return func(expression1, expression2); }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", expression1, Scanner.CharTokenMapping.First(i => i.Value == tokenType).Key, expression2);
        }
    }
}
