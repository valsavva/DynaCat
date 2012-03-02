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
        Plus,
        Minus,
        Multiply,
        Divide,
        Modulo,
        Dot,
        Comma,
        Or,
        And,
        Not,

        Op_E,
        Op_NE,
        Op_G,
        Op_GE,
        Op_L,
        Op_LE,
        Semi
    }
}
