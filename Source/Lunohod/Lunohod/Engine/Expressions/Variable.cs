using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Xge;

namespace Lunohod.Windows.Engine.Expressions
{
    public interface IVariable : IExpression
    {
        string Id { get; }
    }

    public class Variable : Expression, IVariable, INumExpression, IBoolExpression, IStrExpression,
        IAssignable<float>, IAssignable<bool>, IAssignable<string>
    {
        private Type type;
        private float floatValue;
        private bool boolValue;
        private string strValue;
        private object objValue;


        public string Id { get; private set; }

        public override Type Type
        {
            get
            {
                CheckInitialized();
                
                return type;
            }
        }

        public override object GetObjValue()
        {
            return objValue;
        }


        float IExpression<float>.GetValue()
        {
            CheckType<float>();
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

        public void SetValue(float v)
        {
            type = typeof(float);
            objValue = floatValue = v;
        }

        public void SetValue(bool v)
        {
            type = typeof(bool);
            objValue = boolValue = v;
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
            return this.Id;
        }
    }
}
