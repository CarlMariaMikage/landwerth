using System.Collections.Generic;
using System.Linq;

namespace Landwerth.ProgramAnalysis
{
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

        public static SyntaxTree Parse(string text) 
        {
            var parser = new Parser(text);
            return parser.Parse();
        }
    }
}