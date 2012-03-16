using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lunohod.Xge
{
    class Scanner
    {
        public static readonly Dictionary<string, TokenType> CharTokenMapping = new Dictionary<string, TokenType>
        {
            { "(", TokenType.LeftPar },
            { ")", TokenType.RightPar },
            { "@", TokenType.At },
            { "+", TokenType.Plus },
            { "-", TokenType.Minus },
            { "*", TokenType.Multiply },
            { "/", TokenType.Divide },
            { "%", TokenType.Modulo },
            { ".", TokenType.Dot },
            { ",", TokenType.Comma },
            { "&", TokenType.And },
            { "|", TokenType.Or },
            { "!", TokenType.Not },
            { ":", TokenType.Colon },
            { ";", TokenType.SemiColon },
            { "~", TokenType.Squiggle },
            { "=", TokenType.Assign },

            { "==",  TokenType.Op_E  },
            { "!=", TokenType.Op_NE },
            { ">",  TokenType.Op_G  },
            { ">=", TokenType.Op_GE },
            { "<",  TokenType.Op_L  },
            { "<=", TokenType.Op_LE }
        };

        private string text;
        private int i;

        public Scanner()
        {
        }

        public void Initialize(string text)
        {
            this.text = text;
            this.i = 0;
        }

        public Token NextToken()
        {
            if (this.text == null)
                return Token.Eof;

            TokenType tokenType = TokenType.Eof;

            while (i < text.Length)
            {
                int start = i;
                string tokenText = null;

                char ch = text[i];

                // whitespace - skipping it
                if (char.IsWhiteSpace(text[i]))
                {
                    for (; i < text.Length && char.IsWhiteSpace(text[i]); i++) { }
                    continue;
                }
                // num constant
                else if (char.IsDigit(ch))
                {
                    tokenType = TokenType.NumConst;

                    for (; i < text.Length && char.IsDigit(text[i]); i++) { }
                    if (i < text.Length && text[i] == '.')
                    {
                        i++;
                        for (; i < text.Length && char.IsDigit(text[i]); i++) { }
                    }

                }
                // str const
                else if (ch == '\'')
                {
                    i++;

                    tokenType = TokenType.StrConst;

                    // TODO need escaping support
                    while (true)
                    {
                        for (; i < text.Length && (text[i] != '\''); i++) { }
                        if (i + 1 < text.Length && (text[i + 1] == '\''))
                            i += 2;
                        else
                            break;
                    }


                    if (i >= text.Length)
                        Error("Invalid string constant");

                    i++;

                }
                // id
                else if (char.IsLetter(ch))
                {
                    tokenType = TokenType.Id;

                    for (; i < text.Length && (char.IsLetterOrDigit(text[i]) || text[i] == '_'); i++) { }
                }
                else
                {
                    if (!CharTokenMapping.TryGetValue("" + ch, out tokenType))
                        Error(string.Format("Invalid character: {0}", ch));
                    i++;

                    if (i < text.Length && text[i] == '=')
                    {
                        if (tokenType == TokenType.Assign)
                            tokenType = TokenType.Op_E;
                        else if (tokenType == TokenType.Not)
                            tokenType = TokenType.Op_NE;
                        else if (tokenType == TokenType.Op_G)
                            tokenType = TokenType.Op_GE;
                        else if (tokenType == TokenType.Op_L)
                            tokenType = TokenType.Op_LE;
                        i++;
                    }
                }

                tokenText = text.Substring(start, i - start);

                return new Token(tokenType, tokenText);
            }

            return Token.Eof;
        }

        private void Error(string p)
        {
            throw new InvalidOperationException(string.Format("Syntax error: {0}, Pos: {1}", p, i));
        }
    }
}
