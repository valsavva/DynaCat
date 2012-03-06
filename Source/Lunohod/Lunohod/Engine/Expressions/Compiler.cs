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

        private static Compiler instance = new Compiler();

        public static Compiler Instance { get { return instance; } }

        private Compiler()
        {
            this.scanner = new Scanner();
        }

        private void Initialize(XObject currentObject, string text)
        {
            this.currentObject = currentObject;
            this.currentToken = null;
            this.scanner.Initialize(text);
        }

        public static INumExpression CompileNumExpression(XObject currentObject, string text)
        {
            Instance.Initialize(currentObject, text);
            return Instance.CompileExpression<INumExpression>();
        }

        public static IBoolExpression CompileBoolExpression(XObject currentObject, string text)
        {
            Instance.Initialize(currentObject, text);
            return Instance.CompileExpression<IBoolExpression>();
        }

        public static IStrExpression CompileStrExpression(XObject currentObject, string text)
        {
            Instance.Initialize(currentObject, text);
            return Instance.CompileExpression<IStrExpression>();
        }

        public static List<IAction> CompileStatementList(XObject currentObject, string text)
        {
            Instance.Initialize(currentObject, text);
            return Instance.CompileStatementList();
        }

        public T CompileExpression<T>()
            where T : class, IExpression
        {
            currentToken = scanner.NextToken();

            if (M(TokenType.Eof))
                return null;

            Expression e = TExpression();

            T result = e as T;

            if (result == null)
                throw new InvalidOperationException(string.Format("Exprected expression of type '{0}', got '{1}' instead.",
                    typeof(T).GetMethod("GetValue").ReturnType.Name,
                    e.Type.Name
                ));

            return result;
        }

        private Expression TExpression()
        {
            Expression expression1 = TMultiplication();

            while (M(TokenType.Plus, TokenType.Minus, TokenType.Or))
            {
                var opType = P();

                Consume();

                Expression expression2 = TMultiplication();

                Expression op = OperatorFactory.CreateOperator(opType, expression1, expression2);

                expression1 = op;
            }

            return expression1;
        }

        private Expression TMultiplication()
        {
            Expression expression1 = TFactor();

            while (M(TokenType.Multiply, TokenType.Divide, TokenType.Modulo, TokenType.And,
                TokenType.Op_E, TokenType.Op_G, TokenType.Op_GE, TokenType.Op_L, TokenType.Op_LE, TokenType.Op_NE))
            {
                var opType = P();

                Consume();

                Expression expression2 = TFactor();

                var op = OperatorFactory.CreateOperator(opType, expression1, expression2);

                expression1 = op;
            }

            return expression1;
        }

        private Expression TFactor()
        {
            switch (P())
            {
                case TokenType.NumConst:
                    {
                        return TNumConstant();
                    }
                case TokenType.StrConst:
                    {
                        return TStrConstant();
                    }
                case TokenType.Id:
                    {
                        string id = T();

                        Consume();

                        if (id == "true")
                            return new BoolConstant(true);
                        else if (id == "false")
                            return new BoolConstant(false);

                        if (M(TokenType.Colon))
                        {
                            return TBoolFlag(id);
                        }

                        string objectId = null;

                        if (M(TokenType.Dot))
                        {
                            Consume();

                            objectId = id;
                            id = T();

                            Consume(TokenType.Id);
                        }


                        if (M(TokenType.LeftPar))
                            return TMethod(objectId, id);
                        else
                            return TProperty(objectId, id);
                    }
                case TokenType.At:
                    {
                        return TVariable();
                    }
                case TokenType.LeftPar:
                    {
                        Consume();
                        var e = TExpression();
                        Consume(TokenType.RightPar);
                        return e;
                    }
                case TokenType.Not:
                    {
                        return TUnaryNotOperator();
                    };
                case TokenType.Plus: goto case TokenType.Minus;
                case TokenType.Minus:
                    {
                        return TUnaryOperator();
                    };
                default: throw new InvalidOperationException("WTF?");
            }
        }

        private List<IAction> CompileStatementList()
        {
            currentToken = scanner.NextToken();

            if (M(TokenType.Eof))
                return null;

            return TStatementList();
        }

        private List<IAction> TStatementList()
        {
            List<IAction> result = new List<IAction>();

            while(true)
            {
                if (P() == TokenType.Squiggle)
                {
                    Consume();

                    var isInstant = false;
                    var objectId = T();
                    Consume(TokenType.Id);

                    result.Add((IAction)TFlagAction(objectId, isInstant));
                }
                else
                {
                    Ensure(TokenType.Id);

                    var objectId = T();

                    Consume();

                    switch (P())
                    {
                        case TokenType.Dot:
                            {
                                Consume();

                                var actionId = T();

                                Consume();

                                result.Add((IAction)TMethod(objectId, actionId));
                            } break;
                        case TokenType.Colon:
                            {
                                result.Add((IAction)TFlagAction(objectId, true));
                            } break;
                    }
                }

                if (M(TokenType.SemiColon))
                    Consume();
                else
                    break;
            }

            return result;
        }

        private IAction TFlagAction(string objectId, bool isInstant)
        {
            Consume(TokenType.Colon);

            string flagId = T();

            if (M(TokenType.Multiply, TokenType.Plus, TokenType.Minus))
            {
                Consume();
                flagId += T();
            }

            Consume(TokenType.Id);

            return new BoolFlagAction(currentObject, objectId, flagId, isInstant);
        }

        private Expression TStrConstant()
        {
            var str = T();
            Consume();
            return new StrConstant(str.Substring(1, str.Length - 2));
        }

        private Expression TUnaryNotOperator()
        {
            Consume();

            var expression = TFactor();

            return new UnaryNotOperator(expression);
        }


        private Expression TUnaryOperator()
        {
            var tokenType = P();
                
            Consume();

            var expression = TFactor();

            if (tokenType == TokenType.Plus)
                return expression;
            else
                return new UnaryMinusOperator(expression);
        }

        private Expression TVariable()
        {
            throw new NotImplementedException();
        }

        private Expression TNumConstant()
        {
            string strNum = T();
            Consume();
            return new NumConstant(float.Parse(strNum));
        }

        private Expression TProperty(string objectId, string propertyId)
        {
            // Property

            return PropertyFactory.CreateProperty(this.currentObject, objectId, propertyId);
        }

        private Expression TMethod(string objectId, string actionId)
        {
            Consume(TokenType.LeftPar);

            List<Expression> parameters = TFuncParameters();

            Consume(TokenType.RightPar);

            return MethodFactory.CreateMethod(this.currentObject, objectId, actionId, parameters);
        }

        private Expression TBoolFlag(string objectId)
        {
            Consume();

            var flagId = T();

            Consume(TokenType.Id);

            return new BoolFlag(this.currentObject, objectId, flagId);
        }

        private List<Expression> TFuncParameters()
        {
            List<Expression> result = null;

            while (!M(TokenType.RightPar))
            {
                Expression parameter = TExpression();
                
                if (result == null)
                    result = new List<Expression>();

                result.Add((Expression)parameter);

                if (!M(TokenType.RightPar))
                    Consume(TokenType.Comma);
            }
            
            return result;
        }

        private TokenType P()
        {
            return currentToken.TokenType;
        }

        private string T()
        {
            return currentToken.Text;
        }

        private void Consume(params TokenType[] tokenTypes)
        {
            Ensure(tokenTypes);

            currentToken = scanner.NextToken();
        }

        private void Ensure(params TokenType[] tokenTypes)
        {
            if (tokenTypes.Length > 0 && !M(tokenTypes))
                Error(string.Format("Unexpected token. Type: '{0}' Value: '{1}'", P(), T()));
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
