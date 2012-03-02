using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

using NumExpression = Nomnom.XGameExpressions.Expression<float>;
using BoolExpression = Nomnom.XGameExpressions.Expression<bool>;

namespace Nomnom.XGameExpressions
{
    class BoolConstant : BoolExpression
    {
        private bool boolean;

        public BoolConstant(bool boolean)
        {
            this.boolean = boolean;
        }

        public override bool Value { get { return boolean; } }

        public override string ToString()
        {
            return boolean.ToString(CultureInfo.InvariantCulture);
        }    }
}
