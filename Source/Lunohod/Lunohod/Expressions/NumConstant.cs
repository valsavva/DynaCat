using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lunohod.Xge
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
