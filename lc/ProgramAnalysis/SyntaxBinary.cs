using System.Collections.Generic;

namespace Landwerth.ProgramAnalysis
{
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
}