using System.Collections.Generic;

namespace Landwerth.ProgramAnalysis
{
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
}