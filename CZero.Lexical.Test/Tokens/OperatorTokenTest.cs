using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Lexical.Test.Tokens
{
    public class OperatorTokenTest
    {
        static IReadOnlyDictionary<string, Operator> ManualReference = new Dictionary<string, Operator>
            {
                {"+", Operator.Plus },
                {"-", Operator.Minus },
                {"*", Operator.Mult },
                {"/", Operator.Divide },
                {"=", Operator.Assign },
                {"==", Operator.Equal },
                {"!=", Operator.NotEqual },
                {"<", Operator.LessThan },
                {">", Operator.GreaterThan },
                {"<=", Operator.LessEqual },
                {">=", Operator.GreaterEqual },
                {"(", Operator.LeftParen },
                {")", Operator.RightParen },
                {"{", Operator.LeftBrace },
                {"}", Operator.RightBrace },
                {"->", Operator.Arrow },
                {",", Operator.Comma },
                {":", Operator.Colon },
                {";", Operator.Semicolon }
            };

        [Fact]
        void GenerationTest()
        {
            var refer = OperatorToken.GenerateReference();

            var referenceValues = refer.OrderBy(p => p.Key).ToList();

            var manualValue = ManualReference.OrderBy(p => p.Key).ToList();

            Assert.Equal(referenceValues, manualValue);
        }
    }
}
