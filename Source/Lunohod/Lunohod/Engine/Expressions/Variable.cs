using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Xge;

namespace Lunohod.Xge
{
    public interface IVariable : IExpression
    {
        string Id { get; }
    }

    public class Variable : Expression, IVariable, IExpression<double>, IExpression<bool>, IExpression<string>,
        IAssignable<double>, IAssignable<bool>, IAssignable<string>
    {
        private Type type;
        private double floatValue;
        private bool boolValue;
        private string strValue;
        private object objValue;

        private Variable(string id)
        {
            this.Id = id;
        }

        public static Variable GetOrCreateVariable(Dictionary<string, Variable> variableStorage, string id)
        {
            Variable result = null;

            if (!variableStorage.TryGetValue(id, out result))
            {
                result = new Variable(id);
                variableStorage.Add(id, result);
            }


            return result;
        }

        public string Id { get; private set; }

        public override Type Type
        {
            get
            {
                CheckInitialized();
                
                return type;
            }
        }

		internal void SetType(Type type)
		{
			this.type = type;
		}
		
        public override object GetObjValue()
        {
            return objValue;
        }


        double IExpression<double>.GetValue()
        {
            CheckType<double>();
            return floatValue;
        }

        bool IExpression<bool>.GetValue()
        {
            CheckType<bool>();
            return boolValue;
        }

        string IExpression<string>.GetValue()
        {
            CheckType<string>();
            return strValue;
        }

        public void SetValue(double v)
        {
            type = typeof(double);
            objValue = floatValue = v;
            strValue = null;
        }

        public void SetValue(bool v)
        {
            type = typeof(bool);
            objValue = boolValue = v;
            strValue = null;
        }

        public void SetValue(string v)
        {
            type = typeof(string);
            objValue = strValue = v;
        }

        private void CheckInitialized()
        {
            if (type == null)
                throw new InvalidOperationException(string.Format("Variable '{0}' has not been initialized yet.", this.Id));
        }

        private void CheckType<T>()
        {
            CheckInitialized();

            if (type != typeof(T))
                throw new InvalidOperationException(string.Format("Value of wrong type requested. Variable '{0}' is of type '{1}', value type requested: '{2}'", this.Id, type.Name, typeof(T).Name));
        }

        public override string ToString()
        {
            return "@" + this.Id;
        }
    }
}
