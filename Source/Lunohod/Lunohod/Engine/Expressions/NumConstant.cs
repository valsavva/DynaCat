using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lunohod.Xge
{
    public class NumConstant : Expression<double>
    {
        private double number;

        public NumConstant(double number)
        {
            this.number = number;
        }

        public override double GetValue() { return number; }

        public override string ToString()
        {
            return this.number.ToString(CultureInfo.InvariantCulture);
        }
    }
}
