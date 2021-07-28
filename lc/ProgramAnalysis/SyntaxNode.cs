using System.Collections.Generic;

namespace Landwerth.ProgramAnalysis
{
    abstract class SyntaxNode
    {
        public abstract SyntaxType Type{ get; }

        public abstract IEnumerable<SyntaxNode> GetChildren();
    }
}