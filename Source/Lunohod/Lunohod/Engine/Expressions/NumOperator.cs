using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public class NumOperator : Expression<double>
    {
        private TokenType tokenType;
        private IExpression<double> expression1;
        private IExpression<double> expression2;

        private Func<IExpression<double>, IExpression<double>, double> func;

        public NumOperator(TokenType tokenType, IExpression<double> expression1, IExpression<double> expression2)
        {
            this.tokenType = tokenType;
            this.expression1 = expression1;
            this.expression2 = expression2;

            switch (this.tokenType)
            {
                case TokenType.Plus: func = (e1, e2) => (e1.GetValue() + e2.GetValue()); break;
                case TokenType.Minus: func = (e1, e2) => (e1.GetValue() - e2.GetValue()); break;
                case TokenType.Multiply: func = (e1, e2) => (e1.GetValue() * e2.GetValue()); break;
                case TokenType.Divide: func = (e1, e2) => (e1.GetValue() / e2.GetValue()); break;
                case TokenType.Modulo: func = (e1, e2) => (e1.GetValue() % e2.GetValue()); break;
                default: throw new InvalidOperationException("WTF?");
            }
        }

        public override double GetValue() { return func(expression1, expression2); }

        public override string ToString()
        {
            return string.Format("({0} {1} {2})", expression1, Scanner.CharTokenMapping.First(i => i.Value == tokenType).Key, expression2);
        }
    }
}
