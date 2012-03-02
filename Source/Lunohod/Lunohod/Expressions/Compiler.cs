using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;

namespace Lunohod.Xge
{
    public class Compiler
    {
        private Scanner scanner;
        private Token currentToken;
        private XObject currentObject;

        public Compiler(XObject currentObject)
        {
            this.currentObject = currentObject;
        }

        public INumExpression CompileNumExpression(string input)
        {
            this.scanner = new Scanner(input);
            this.currentToken = scanner.NextToken();

            if (M(TokenType.Eof))
                return null;

            return TNumExpression();
        }

        public IBoolExpression CompileBoolExpression(string input)
        {
            scanner = new Scanner(input);
            currentToken = scanner.NextToken();

            if (M(TokenType.Eof))
                return null;

            return TBoolExpression();
        }

        private IBoolExpression TBoolExpression()
        {
            var expression1 = TBoolMultiplication();

            while (M(TokenType.Or))
            {
                var opType = Consume().TokenType;
                var expression2 = TBoolMultiplication();

                var op = new BoolBinaryOperator(opType, expression1, expression2);

                expression1 = op;
            }

            return expression1;
        }

        private IBoolExpression TBoolMultiplication()
        {
            var expression1 = TPredicate();

            while (M(TokenType.And))
            {
                var opType = Consume().TokenType;
                var expression2 = TPredicate();

                var op = new BoolBinaryOperator(opType, expression1, expression2);

                expression1 = op;
            }

            return expression1;
        }

        private IBoolExpression TPredicate()
        {
            switch (T())
            {
                case TokenType.Id:
                    {
                        if (this.currentToken.Text == "true")
                        {
                            Consume();
                            return new BoolConstant(true);
                        }
                        else if (this.currentToken.Text == "false")
                        {
                            Consume();
                            return new BoolConstant(false);
                        }

                        return (IBoolExpression)TBoolFlagOrPropertyOrFunction(Consume().Text);
                    }
                case TokenType.At:
                    {
                        return (IBoolExpression)TVariable<bool>();
                    }
                case TokenType.LeftPar:
                    {
                        Consume();
                        var e = TBoolExpression();
                        Consume(TokenType.RightPar);
                        return e;
                    }
                case TokenType.Not:
                    {
                        return TUnaryNotOperator();
                    };

                default:
                    {
                        var expression1 = TNumExpression();
                        var op = Consume(
                            TokenType.Op_E,
                            TokenType.Op_NE,
                            TokenType.Op_G,
                            TokenType.Op_GE,
                            TokenType.Op_L,
                            TokenType.Op_LE
                        );
                        var expression2 = TNumExpression();
                        return new CompareOperator(op.TokenType, expression1, expression2);
                    }
            }
        }

        private Expression TBoolFlagOrPropertyOrFunction(string id)
        {
            if (M(TokenType.Semi))
            {
                Consume();
                return new BoolFlag(currentObject, id, Consume(TokenType.Id).Text);
            }

            return TPropertyOrFunction<bool>(id);
        }

        private IBoolExpression TUnaryNotOperator()
        {
            var operatorToken = Consume();

            var expression = TPredicate();

            return new UnaryNotOperator(expression);
        }

        private INumExpression TNumExpression()
        {
            var expression1 = TNumMultiplication();

            while (M(TokenType.Plus, TokenType.Minus))
            {
                var opType = Consume().TokenType;
                var expression2 = TNumMultiplication();

                var op = new NumBinaryOperator(opType, expression1, expression2);

                expression1 = op;
            }

            return expression1;
        }

        private INumExpression TNumMultiplication()
        {
            var expression1 = TFactor();

            while (M(TokenType.Multiply, TokenType.Divide, TokenType.Modulo))
            {
                var opType = Consume().TokenType;
                var expression2 = TFactor();

                var op = new NumBinaryOperator(opType, expression1, expression2);

                expression1 = op;
            }

            return expression1;
        }


        private INumExpression TFactor()
        {
            switch (T())
            {
                case TokenType.NumConst:
                    {
                        return TNumConstant();
                    }
                case TokenType.Id:
                    {
                        return (INumExpression)TPropertyOrFunction<float>(Consume().Text);
                    }
                case TokenType.At:
                    {
                        return (INumExpression)TVariable<float>();
                    }
                case TokenType.LeftPar:
                    {
                        Consume();
                        var e = TNumExpression();
                        Consume(TokenType.RightPar);
                        return e;
                    }
                case TokenType.Plus: goto case TokenType.Minus;
                case TokenType.Minus:
                    {
                        return TUnaryOperator();
                    };
                default: throw new InvalidOperationException("WTF?");
            }
        }

        private INumExpression TUnaryOperator()
        {
            var operatorToken = Consume();

            var expression = TFactor();

            if (operatorToken.TokenType == TokenType.Plus)
                return expression;
            else
                return new UnaryMinusOperator(expression);
        }

        private Expression TVariable<T>()
        {
            throw new NotImplementedException();
        }

        private INumExpression TNumConstant()
        {
            return new NumConstant(float.Parse(Consume().Text));
        }

        private Expression TPropertyOrFunction<T>(string id)
        {
            string propertyId = id;
            string objectId  = null;

            if (M(TokenType.Dot))
            {
                Consume();

                objectId = propertyId;
                propertyId = Consume(TokenType.Id).Text;
            }

            if (M(TokenType.LeftPar))
            {
                Consume();

                List<Expression> parameters = TFuncParameters();

                Consume(TokenType.RightPar);
                if (typeof(T) == typeof(float))
                    return new NumFunction(this.currentObject, objectId, propertyId, parameters);
                else
                    return new BoolFunction(this.currentObject, objectId, propertyId, parameters);
            }
            else
            {
                if (typeof(T) == typeof(float))
                    return new NumProperty(this.currentObject, objectId, propertyId);
                else
                    return new BoolProperty(this.currentObject, objectId, propertyId);
            }
        }

        private List<Expression> TFuncParameters()
        {
            List<Expression> result = null;

            while (!M(TokenType.RightPar))
            {
                var parameter = TNumExpression();
                
                if (result == null)
                    result = new List<Expression>();

                result.Add((Expression)parameter);

                if (!M(TokenType.RightPar))
                    Consume(TokenType.Comma);
            }
            
            return result;
        }

        private TokenType T()
        {
            return currentToken.TokenType;
        }

        private Token Consume(params TokenType[] tokenTypes)
        {
            if (tokenTypes.Length > 0 && !M(tokenTypes))
                Error("Unexpected token: " + currentToken.Text);

            var oldToken = currentToken;
            currentToken = scanner.NextToken();
            return oldToken;
        }

        private bool M(params TokenType[] tokenTypes)
        {
            return Array.IndexOf(tokenTypes, currentToken.TokenType) >= 0;
        }

        private void Error(string p)
        {
            throw new InvalidOperationException(p);
        }

    }
}
