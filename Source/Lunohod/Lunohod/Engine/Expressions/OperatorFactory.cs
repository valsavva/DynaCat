using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public class OperatorFactory
    {
        public static Dictionary<TokenType, Func<TokenType, Expression, Expression, Expression>> OperatorCreators = new Dictionary<TokenType, Func<TokenType,Expression,Expression,Expression>>()
        {
            { TokenType.Plus, NewNumOperator },
            { TokenType.Minus, NewNumOperator },
            { TokenType.Multiply, NewNumOperator },
            { TokenType.Divide, NewNumOperator },
            { TokenType.Modulo, NewNumOperator },

            { TokenType.Or, NewBoolOperator },
            { TokenType.And, NewBoolOperator },
            { TokenType.Not, NewBoolOperator },

            { TokenType.Op_E, NewCompareOperator },
            { TokenType.Op_NE, NewCompareOperator },
            { TokenType.Op_G, NewCompareOperator },
            { TokenType.Op_GE, NewCompareOperator },
            { TokenType.Op_L, NewCompareOperator },
            { TokenType.Op_LE, NewCompareOperator }
        };


        public static Expression CreateOperator(TokenType opType, Expression expression1, Expression expression2)
        {
            Func<TokenType, Expression, Expression, Expression> creator;

            if (!OperatorCreators.TryGetValue(opType, out creator))
                throw new InvalidOperationException("Unknown operator type: " + opType);

            return creator(opType, expression1, expression2);
        }

        public static Expression NewNumOperator(TokenType tokenType, Expression expression1, Expression expression2)
        {
            if (tokenType == TokenType.Plus && expression1 is IStrExpression)
                return new StrConcatOperator(Validator.CheckType<IStrExpression>(expression1), Validator.CheckType<IStrExpression>(expression2));
            else
                return new NumOperator(tokenType, Validator.CheckType<INumExpression>(expression1), Validator.CheckType<INumExpression>(expression2));
        }

        public static Expression NewBoolOperator(TokenType tokenType, Expression expression1, Expression expression2)
        {
            return new BoolOperator(tokenType, Validator.CheckType<IBoolExpression>(expression1), Validator.CheckType<IBoolExpression>(expression2));
        }

        public static Expression NewCompareOperator(TokenType tokenType, Expression expression1, Expression expression2)
        {
            return new CompareOperator(tokenType, Validator.CheckType<INumExpression>(expression1), Validator.CheckType<INumExpression>(expression2));
        }
    }
}
