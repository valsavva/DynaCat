using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    public enum TokenType
    {
        Eof,
        NumConst,
        StrConst,
        Id,
        LeftPar,
        RightPar,
        At,
        Dot,
        Comma,
        Colon,
        SemiColon,
        Squiggle,

        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,

        Or,
        And,
        Not,

        Op_E,
        Op_NE,
        Op_G,
        Op_GE,
        Op_L,
        Op_LE,
        Assign,
    }
}
