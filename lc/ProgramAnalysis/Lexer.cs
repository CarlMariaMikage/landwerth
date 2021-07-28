using System;
using System.Collections.Generic;

namespace Landwerth.ProgramAnalysis
{
    class Lexer 
    {
        private readonly string txt;
        private int pos;
        private List<string> diagnostics = new List<string>();

        public Lexer(String text) 
        {
            txt = text;
        }

        public IEnumerable<string> Diagnostics => diagnostics;

        private char Current
        {
            get
            {
                if(pos >= txt.Length)
                    return '\0';
                return txt[pos];
            }
        }

        private void Next()
        {
            pos++;
        }

        public SyntaxToken NextToken() 
        {
            //<numbers>
            //+ - . / % ( )
            //<whitespace>

            if(pos >= txt.Length)
            {
                return new SyntaxToken(SyntaxType.EndOfFile, pos, "\0", null);
            }

            if(char.IsDigit(Current))
            {
                var s = pos;

                while(char.IsDigit(Current))
                    Next();

                var l = pos - s;
                var text = txt.Substring(s, l);
                if(!int.TryParse(text, out var value))
                    diagnostics.Add($"The number {txt} is not a valid Int32.");

                return new SyntaxToken(SyntaxType.Number, s, text, value);
            }

            if(char.IsWhiteSpace(Current))
            {
                var s = pos;

                while(char.IsWhiteSpace(Current))
                    Next();

                var l = pos - s;
                var text = txt.Substring(s, l);
                int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxType.Whitespace, s, text, null);
            }

            switch(Current)
            {
                case '+' :
                    return new SyntaxToken(SyntaxType.PlusOperator, pos++, "+", null);
                case '-' :
                    return new SyntaxToken(SyntaxType.MinusOperator, pos++, "-", null);
                case '.' :
                    return new SyntaxToken(SyntaxType.TimesOperator, pos++, ".", null);
                case '/' :
                    return new SyntaxToken(SyntaxType.SlashOperator, pos++, "/", null);
                case '%' :
                    return new SyntaxToken(SyntaxType.ModOperator, pos++, "%", null);
                case '(' :
                    return new SyntaxToken(SyntaxType.OpenBracketsOperator, pos++, "(", null);
                case ')' :
                    return new SyntaxToken(SyntaxType.CloseBracketsOperator, pos++, ")", null);
                default:
                    diagnostics.Add($"Alert: Undefined token input: '{Current}'");
                    return new SyntaxToken(SyntaxType.UndefinedToken, pos++, txt.Substring(pos - 1, 1), null);
            }
            
        }
    }
}