﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lunohod.Xge
{
    public class NumConstant : Expression<float>
    {
        private float number;

        public NumConstant(float number)
        {
            this.number = number;
        }

        public override float GetValue() { return number; }

        public override string ToString()
        {
            return this.number.ToString(CultureInfo.InvariantCulture);
        }
    }
}