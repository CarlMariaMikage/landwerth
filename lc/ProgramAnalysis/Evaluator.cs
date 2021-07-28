using System;

namespace Landwerth.ProgramAnalysis
{

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