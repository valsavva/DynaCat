using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public interface IExpression
    {
        object GetObjValue();
        Type Type { get; }
    }

	public static class ExpressionEx
	{
		public static double GetNumValue(this IExpression e)
		{
			return ((IExpression<double>)e).GetValue();
		}
		public static int GetIntValue(this IExpression e)
		{
			return (int)((IExpression<double>)e).GetValue();
		}
		public static string GetStrValue(this IExpression e)
		{
			return ((IExpression<string>)e).GetValue();
		}
		public static bool GetBoolValue(this IExpression e)
		{
			return ((IExpression<bool>)e).GetValue();
		}
	}
	
    public interface IExpression<T> : IExpression
    {
        T GetValue();
    }

    public interface IAssignable
    {
    }

    public interface IAssignable<T> : IAssignable
    {
        void SetValue(T v);
    }

    public interface IAction
    {
        void Call();
    }

    public abstract class Expression : IExpression
    {
        public abstract Type Type { get; }
        public abstract object GetObjValue();
    }

    public abstract class Expression<T> : Expression, IExpression<T>
    {
        public abstract T GetValue();

        public override Type Type { get { return typeof(T); } }
        public override object GetObjValue() { return this.GetValue(); }
    }
}
