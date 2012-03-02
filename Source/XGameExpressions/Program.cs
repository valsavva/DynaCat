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
            l.Subcomponents =new XObjectCollection() { new XBlock() { Id = "A", Bounds = new RectangleF(10,10,20,20) } };

            CompilerBool("true & true");
            CompilerBool("true & false");
            CompilerBool("false & false");
            CompilerBool("true | true");
            CompilerBool("true | false");
            CompilerBool("false | false");
            CompilerBool("true & (5 > 3) & Yeah()");
            Compiler("A.X*2");
            Compiler("A.X*2*40.5 + 50*2)");
        }

        private static void Compiler(string p)
        {
            Compiler compiler = new Compiler(l);
            var nexp = compiler.CompileNumExpression(p);

            Console.WriteLine(nexp.ToString() + " = " + nexp.Value);
        }

        private static void CompilerBool(string p)
        {
            Compiler compiler = new Compiler(l);
            var nexp = compiler.CompileBoolExpression(p);

            Console.WriteLine(nexp.ToString() + " = " + nexp.Value);
        }

    }
}
