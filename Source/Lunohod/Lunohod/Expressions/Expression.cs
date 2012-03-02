using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public interface INumExpression
    {
        float Value { get; }
    }

    public interface IBoolExpression
    {
        bool Value { get; }
    }

    public abstract class Expression
    {
        public abstract Type Type { get; }
        public abstract object ObjValue { get; }
    }

    public abstract class Expression<T> : Expression
    {
        public abstract T Value { get; }

        public override Type Type { get { return typeof(T); } }
        public override object ObjValue { get { return this.Value; } }
    }
}
