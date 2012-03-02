using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    public class NumConstant : NumExpression
    {
        private float number;

        public NumConstant(float number)
        {
            this.number = number;
        }

        public override float Value { get { return number; } }

        public override string ToString()
        {
            return this.Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
