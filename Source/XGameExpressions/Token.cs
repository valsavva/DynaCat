﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nomnom.XGameExpressions
{
    class Token
    {
        public static readonly Token Eof = new Token(TokenType.Eof);

        public TokenType TokenType { get; private set; }
        public string Text { get; set; }

        public Token(TokenType tokenType, string text = null)
        {
            this.TokenType = tokenType;
            this.Text = text;
        }
    }
}
