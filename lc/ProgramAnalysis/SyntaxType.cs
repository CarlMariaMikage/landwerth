namespace Landwerth.ProgramAnalysis
{
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
}