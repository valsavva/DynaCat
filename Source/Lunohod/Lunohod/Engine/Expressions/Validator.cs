using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public class Validator
    {
        public static T CheckType<T>(Expression e)
            where T : class
        {
            if (!(e is T))
                throw new InvalidOperationException(string.Format(
                    "Invalid type. Expected: {0} Received: {1}", typeof(T).Name, e.Type.Name
                ));

            return e as T;
        }
    }
}
