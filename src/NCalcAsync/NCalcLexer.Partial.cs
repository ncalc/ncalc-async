﻿namespace NCalcAsync
{
    public partial class NCalcLexer
    {
        public override void EmitErrorMessage(string msg)
        {
            throw new EvaluationException(msg);
        }
    }
}