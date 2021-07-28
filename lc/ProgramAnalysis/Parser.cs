using System.Collections.Generic;

namespace Landwerth.ProgramAnalysis
{
    class Parser 
    {
        private readonly SyntaxToken[] tkns;

        private List<string> diagnostics = new List<string>();
        private int pos;

        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();

            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.NextToken();

                if((token.Type != SyntaxType.Whitespace) && (token.Type != SyntaxType.UndefinedToken)) //do not parse whitespaces or undefined tokens
                {
                    tokens.Add(token);
                }

            } while (token.Type != SyntaxType.EndOfFile);

            tkns = tokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);
        }

        public IEnumerable<string> Diagnostics => diagnostics;

        private SyntaxToken LookAhead(int off)
        {
            var index = pos + off;
            if(index >= tkns.Length)
                return tkns[tkns.Length - 1];
            return tkns[index];
        }

        private SyntaxToken NextToken()
        {
            var current = Current;
            pos++;
            return current;
        }

        private SyntaxToken Check(SyntaxType type)
        {
            if(Current.Type == type)
                return NextToken();
            
            diagnostics.Add($"Alert: Unprecedented token <{Current.Type}>, expected <{type}>");
            return new SyntaxToken(type, Current.Position, null, null);
        }

        private SyntaxToken Current => LookAhead(0);

        private SyntaxExpression ParseExpression()
        {
            return ParseTerm();
        }

        public SyntaxTree Parse()
        {
            var expr = ParseTerm();
            var endOfFile = Check(SyntaxType.EndOfFile);
            return new SyntaxTree(diagnostics, expr, endOfFile);
        }

        private SyntaxExpression ParseTerm()
        {
            var left = ParseFactor();

            while ((Current.Type == SyntaxType.PlusOperator) || (Current.Type == SyntaxType.MinusOperator))
            {
                var operatorToken = NextToken();
                var right = ParseFactor();
                left = new SyntaxBinary(left, operatorToken, right);
            }

            return left;
        }

        private SyntaxExpression ParseFactor()
        {
            var left = ParsePrimaryExpression();

            while ((Current.Type == SyntaxType.TimesOperator) || (Current.Type == SyntaxType.SlashOperator) || (Current.Type == SyntaxType.ModOperator))
            {
                var operatorToken = NextToken();
                var right = ParsePrimaryExpression();
                left = new SyntaxBinary(left, operatorToken, right);
            }

            return left;
        }

        private SyntaxExpression ParsePrimaryExpression()
        {
            if(Current.Type == SyntaxType.OpenBracketsOperator)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = Check(SyntaxType.CloseBracketsOperator);
                return new SyntaxBrackets(left, expression, right);
            }

            var numberToken = Check(SyntaxType.Number);
            return new SyntaxNumber(numberToken);
        }
    }
}