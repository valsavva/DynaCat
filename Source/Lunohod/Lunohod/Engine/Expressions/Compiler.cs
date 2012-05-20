using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Objects;
using System.Globalization;
using Lunohod.Xge;

namespace Lunohod.Xge
{
    public class Compiler
    {
        private Scanner scanner;
        private Token currentToken;
        private XObject currentObject;

        static Compiler()
        {
            Instance = new Compiler();
        }

        private Compiler()
        {
            this.scanner = new Scanner();
            this.VariableStorage = new Dictionary<string, Variable>();
        }

        public static Compiler Instance { get; private set; }

        public Dictionary<string, Variable> VariableStorage { get; private set; }

        private void Initialize(XObject currentObject, string text)
        {
            this.currentObject = currentObject;
            this.currentToken = null;
            this.scanner.Initialize(text);
        }

        public static IExpression<T> CompileExpression<T>(XObject currentObject, string text)
        {
			PerfMon.Start("CompileExpression");

            Instance.Initialize(currentObject, text);
            var result = Instance.CompileExpression<T>();

			PerfMon.Stop("CompileExpression");

			return result;
        }

        public static List<IAction> CompileStatements(XObject currentObject, string text)
        {
			PerfMon.Start("CompileStatements");
            
			Instance.Initialize(currentObject, text);
            var result = Instance.CompileStatements();
		
			PerfMon.Stop("CompileStatements");

			return result;
        }

        public IExpression<T> CompileExpression<T>()
        {
            currentToken = scanner.NextToken();

            if (M(TokenType.Eof))
                return null;

            Expression e = TExpression();

            Ensure(TokenType.Eof);

            var result = e as IExpression<T>;

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
                case TokenType.True:
                case TokenType.False:
                    {
                        return TBoolConstant();
                    }
                case TokenType.Id:
                    {
                        string id = T();

                        Consume();

                        if (M(TokenType.Colon))
                        {
                            return TBoolFlag(id);
                        }

                        return TMethodOrProperty(id);
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

        private Expression TBoolConstant()
        {
            var result = new BoolConstant(M(TokenType.True));
            Consume();
            return result;
        }

        private Expression TMethodOrProperty(string id)
        {
			PerfMon.Start("MethodOrProperty");

            string objectId = null;

            if (M(TokenType.Dot))
            {
                Consume();

                objectId = id;
                id = T();

                Consume(TokenType.Id);
            }


			Expression result;
            if (M(TokenType.LeftPar))
                result = TMethod(objectId, id);
            else
                result = TProperty(objectId, id);
		
			PerfMon.Start("MethodOrProperty");

			return result;
        }

        private List<IAction> CompileStatements()
        {
            currentToken = scanner.NextToken();

            if (M(TokenType.Eof))
                return null;

            var result = TStatementList();

            Ensure(TokenType.Eof);

            return result;
        }

        private List<IAction> TStatementList()
        {
            List<IAction> result = new List<IAction>();

            while(true)
            {
                if (P() == TokenType.At)
                {
                    result.Add(TVariableAssignStatement());
                }
                else if (P() == TokenType.Squiggle)
                {
                    Consume();

                    var isInstant = false;
                    var objectId = T();
                    Consume(TokenType.Id);

                    result.Add(TFlagAction(objectId, isInstant));
                }
                else
                {
                    Ensure(TokenType.Id);

                    var id = T();

                    Consume();

                    if (P() == TokenType.Colon)
                    {
                        result.Add((IAction)TFlagAction(id, true));
                    }
                    else
                    {
                        var methodOrProperty = TMethodOrProperty(id);

                        if (methodOrProperty is IProperty)
                            result.Add(TAssignStatement((IAssignable)methodOrProperty));
                        else
                            result.Add((IAction)methodOrProperty);
                    }
                }

                while (M(TokenType.SemiColon))
                    Consume();

                if (M(TokenType.Eof))
                    break;
            }

            return result;
        }

        private IAction TAssignStatement(IAssignable lExpression)
        {
            Consume(TokenType.Assign);

            var rExpression = TExpression();
			
			if (rExpression.Type == typeof(double))
	            return new AssignStatement<double>((IAssignable<double>)lExpression, (IExpression<double>)rExpression);
			else if (rExpression.Type == typeof(string))
	            return new AssignStatement<string>((IAssignable<string>)lExpression, (IExpression<string>)rExpression);
			else // if (rExpression.Type == typeof(bool))
	            return new AssignStatement<bool>((IAssignable<bool>)lExpression, (IExpression<bool>)rExpression);
        }

        private IAction TVariableAssignStatement()
        {
            var variable = TVariable();

            return TAssignStatement((IAssignable)variable);
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

            if (expression.Type != typeof(bool))
                Error("Unary operator 'not' cannot be applied to opearand of type '{0}'", expression.Type.Name);

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
            Consume(TokenType.At);
            var id = currentToken.Text;
            Consume(TokenType.Id);

            return Variable.GetOrCreateVariable(this.VariableStorage, id);
        }

        private Expression TNumConstant()
        {
            string strNum = T();
            Consume();
            return new NumConstant(double.Parse(strNum, CultureInfo.InvariantCulture));
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
                Error("Unexpected token. Type: '{0}' Value: '{1}'. Expected: '{2}'", P(), T(), string.Join("' or '", tokenTypes.Select(t => t.ToString())));
        }

        private bool M(params TokenType[] tokenTypes)
        {
            return Array.IndexOf(tokenTypes, currentToken.TokenType) >= 0;
        }

        private void Error(string p, params object[] ps)
        {
            throw new InvalidOperationException(string.Format(p, ps));
        }

    }
}
