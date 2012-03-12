using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Lunohod.Xge
{
    class StrConstant : Expression<string>
    {
        private string str;

        public StrConstant(string str)
        {
            this.str = str;
        }

        public override string GetValue() { return str; }

        public override string ToString()
        {
            return str;
        }
    }
}
