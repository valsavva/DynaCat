using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lunohod.Xge;
using Lunohod.Objects;
using System.Drawing;

namespace XGameExpressions
{
    class Program
    {
        static XLevel l;

        static void Main(string[] args)
        {
            l = new XLevel() { Id = "whatever" };
            l.Subcomponents =new XObjectCollection() {
                new XSystem { Id="system" },
                new XBlock() { Id = "A", Bounds = new RectangleF(10,10,20,20) }
            };

            CB("true & true");
            CB("true & false");
            CB("false & false");
            CB("true | true");
            CB("true | false");
            CB("false | false");

            CN("system.Rnd(0,5)");
            CN("A.X*2");
            CN("A.X*2*40.5 + 50*2)");
        }

        private static void CN(string p)
        {
            var nexp = Compiler.CompileNumExpression(l, p);

            Console.WriteLine(nexp.ToString() + " = " + nexp.GetValue());
        }

        private static void CB(string p)
        {
            var nexp = Compiler.CompileBoolExpression(l, p);

            Console.WriteLine(nexp.ToString() + " = " + nexp.GetValue());
        }

    }
}
