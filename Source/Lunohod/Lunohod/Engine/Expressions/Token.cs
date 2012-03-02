using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
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

        public override string ToString()
        {
            return string.Format("Type: {0} Value: '{1}'", this.TokenType, this.Text);
        }

    }
}
