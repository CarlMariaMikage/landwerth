using System;
using System.Collections.Generic;
using System.Linq;

namespace rmrc
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Rainer Maria Rilke Compiler, Ausf. 0.0.0.1");
            while(true) 
            {
                Console.Write("rmrc> ");
                var line = Console.ReadLine();
                if(string.IsNullOrWhiteSpace(line))
                    return;
                
                var parser = new Parser(line);
                var syntaxTree = parser.Parse();

                var colour = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Display(syntaxTree.Root);
                Console.ForegroundColor = colour;

                if (!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine(result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var diagnostics in syntaxTree.Diagnostics)
                        Console.WriteLine(diagnostics);

                    Console.ForegroundColor = colour;
                }
            }
        }

        static void Display(SyntaxNode node, String indent = "", bool isLast = true)
        {
            
            var marker = (isLast) ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Type);

            if((node is SyntaxToken t) && (t.Value != null))
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += (isLast) ? "\t" : "│   ";

            var lastChild = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
                Display(child, indent, child == lastChild);
        }
    }

    enum SyntaxType
    {
        Number,
        Whitespace,
        PlusOperator,
        MinusOperator,
        TimesOperator,
        SlashOperator,
        ModOperator,
        OpenBracketsOperator,
        CloseBracketsOperator,
        UndefinedToken,
        EndOfFile,
        NumberExpression,
        BinaryExpression,
        BracketsExpression
    }

    class SyntaxToken : SyntaxNode
    {
        public SyntaxToken(SyntaxType type, int position, string text, object value)
        {
            Type = type;
            Position = position;
            Text = text;
            Value = value;
        }

        public override SyntaxType Type { get; }
        public int Position { get; }
        public string Text { get; }
        public object Value { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Enumerable.Empty<SyntaxNode>();
        }
    }

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

    abstract class SyntaxNode
    {
        public abstract SyntaxType Type{ get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }

    abstract class SyntaxExpression : SyntaxNode
    {

    }

    sealed class SyntaxNumber : SyntaxExpression
    {
        public SyntaxNumber(SyntaxToken numberToken)
        {
            NumberToken = numberToken;
        }

        public override SyntaxType Type => SyntaxType.NumberExpression;
        public SyntaxToken NumberToken { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return NumberToken;
        }
    }

    sealed class SyntaxBinary : SyntaxExpression
    {
        public SyntaxBinary(SyntaxExpression left, SyntaxToken operatorToken, SyntaxExpression right)
        {
            Left = left;
            OperatorToken = operatorToken;
            Right = right;
        }
        
        public override SyntaxType Type => SyntaxType.BinaryExpression;
        public SyntaxExpression Left { get; }
        public SyntaxToken OperatorToken { get; }
        public SyntaxExpression Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatorToken;
            yield return Right;
        }
    }

    sealed class SyntaxBrackets : SyntaxExpression
    {
        public SyntaxBrackets(SyntaxToken openBracketsOperator, SyntaxExpression expression, SyntaxToken closeBracketsOperator)
        {
            OpenBracketsOperator = openBracketsOperator;
            Expression = expression;
            CloseBracketsOperator = closeBracketsOperator;
        }

        public override SyntaxType Type => SyntaxType.BracketsExpression;
        public SyntaxToken OpenBracketsOperator { get; }
        public SyntaxExpression Expression { get; }
        public SyntaxToken CloseBracketsOperator { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenBracketsOperator;
            yield return Expression;
            yield return CloseBracketsOperator;
        }
    }

    sealed class SyntaxTree 
    {
        public SyntaxTree(IEnumerable<string> diagnostics, SyntaxExpression root, SyntaxToken endOfFile) 
        {
            Diagnostics = diagnostics.ToArray();
            Root = root;
            EndOfFile = endOfFile;
        }

        public IReadOnlyList<string> Diagnostics { get; }
        public SyntaxExpression Root { get; }
        public SyntaxToken EndOfFile { get; }
    }
    
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

    class Evaluator
    {
        private readonly SyntaxExpression rt;

        public Evaluator(SyntaxExpression root)
        {
            rt = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(rt);
        }

        private int EvaluateExpression(SyntaxExpression root)
        {
            if(root is SyntaxNumber n)
                return (int) n.NumberToken.Value;
            
            if(root is SyntaxBinary b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                switch(b.OperatorToken.Type)
                {
                    case SyntaxType.PlusOperator :
                        return left + right;
                    case SyntaxType.MinusOperator :
                        return left - right;
                    case SyntaxType.TimesOperator :
                        return left * right;
                    case SyntaxType.SlashOperator :
                        return left / right;
                    case SyntaxType.ModOperator :
                        return left % right;
                    default :
                        throw new Exception($"Unprecedented binary operator {b.OperatorToken.Type}.");
                }
            }

            if(root is SyntaxBrackets p)
                return EvaluateExpression(p.Expression);

            throw new Exception($"Unprecedented node {root.Type}.");
        }
    }
}
