using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lunohod.Xge
{
    class BoolConstant : Expression<bool>
    {
        private bool boolean;

        public BoolConstant(bool boolean)
        {
            this.boolean = boolean;
        }

        public override bool GetValue() { return boolean; }

        public override string ToString()
        {
            return boolean.ToString(CultureInfo.InvariantCulture);
        }
    }
}
