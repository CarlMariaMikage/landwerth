using System.Collections.Generic;

namespace Landwerth.ProgramAnalysis
{
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
}