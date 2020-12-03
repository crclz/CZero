using CZero.Lexical.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CZero.Lexical.Test
{
    public class LexerTest
    {
        static void AssertJsonEqual(object a, object b)
        {
            var typeA = a.GetType();
            Assert.IsType(typeA, b);

            var aJson = Newtonsoft.Json.JsonConvert.SerializeObject(a);
            var bJson = Newtonsoft.Json.JsonConvert.SerializeObject(b);
            Assert.Equal(aJson, bJson);
        }

        [Fact]
        void LexerSmokeTest()
        {
            var sourceCode =
@"fn foo(i: int) -> int {
    let a=1;const _b=2 as ifelse// boring
    return -i+break 1.0E-5+'\n'0268==
    putint(foo(*123456));
}
";
            var expectedTokens = new Token[]
            {
                new KeywordToken(Keyword.Fn,(0,0)),
                new IdentifierToken("foo",(0,3)),
                new OperatorToken(Operator.LeftParen,(0,6)),
                new IdentifierToken("i",(0,7)),
                new OperatorToken(Operator.Colon,(0,8)),
                new IdentifierToken("int",(0,10)),
                new OperatorToken(Operator.RightParen,(0,13)),
                new OperatorToken(Operator.Arrow,(0,15)),
                new IdentifierToken("int",(0,18)),
                new OperatorToken(Operator.LeftBrace,(0,22)),

                new KeywordToken(Keyword.Let,(1,4)),
                new IdentifierToken("a",(1,8)),
                new OperatorToken(Operator.Assign,(1,9)),
                new UInt64LiteralToken(1,(1,10)),
                new OperatorToken(Operator.Semicolon,(1,11)),
                new KeywordToken(Keyword.Const,(1,12)),
                new IdentifierToken("_b",(1,18)),
                new OperatorToken(Operator.Assign,(1,20)),
                new UInt64LiteralToken(2,(1,21)),
                new KeywordToken(Keyword.As,(1,23)),
                new IdentifierToken("ifelse",(1,26)),

                new KeywordToken(Keyword.Return,(2,4)),
                new OperatorToken(Operator.Minus,(2,11)),
                new IdentifierToken("i",(2,12)),
                new OperatorToken(Operator.Plus,(2,13)),
                new KeywordToken(Keyword.Break,(2,14)),
                new DoubleLiteralToken(1.0E-5,(2,20)),
                new OperatorToken(Operator.Plus,(2,26)),
                new CharLiteralToken('\n',(2,27)),
                new UInt64LiteralToken(0268,(2,31)),
                new OperatorToken(Operator.Equal,(2,35)),

                new IdentifierToken("putint",(3,4)),
                new OperatorToken(Operator.LeftParen,(3,10)),
                new IdentifierToken("foo",(3,11)),
                new OperatorToken(Operator.LeftParen,(3,14)),
                new OperatorToken(Operator.Mult,(3,15)),
                new UInt64LiteralToken(123456,(3,16)),
                new OperatorToken(Operator.RightParen,(3,22)),
                new OperatorToken(Operator.RightParen,(3,23)),
                new OperatorToken(Operator.Semicolon,(3,24)),

                new OperatorToken(Operator.RightBrace,(4,0)),
            };

            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();

            Assert.Equal(expectedTokens.Length, tokens.Count);

            for (var i = 0; i < expectedTokens.Length; i++)
            {
                var expectedToken = expectedTokens[i];
                var actualToken = tokens[i];

                AssertJsonEqual(expectedToken, actualToken);
            }

            // TODO: check both are end
        }


        [Fact]
        void LexerSmokeTestSecond()
        {
            var sourceCode =
@"fn+let-const*as/while//hello
continue:=if->(else=> _==int){""as\nc\rc\tc""
<=p>1.5E-3!=231231231231</p>
}
";
            var expectedTokens = new Token[]
            {
                new KeywordToken(Keyword.Fn,(0,0)), new OperatorToken(Operator.Plus,(0,2)), new KeywordToken(Keyword.Let,(0,3)),
                new OperatorToken(Operator.Minus,(0,6)), new KeywordToken(Keyword.Const,(0,7)), new OperatorToken(Operator.Mult,(0,12)),
                new KeywordToken(Keyword.As,(0,13)), new OperatorToken(Operator.Divide,(0,15)),new KeywordToken(Keyword.While,(0,16)),

                new KeywordToken(Keyword.Continue,(1,0)), new OperatorToken(Operator.Colon,(1,8)), new OperatorToken(Operator.Assign,(1,9)),
                new KeywordToken(Keyword.If,(1,10)), new OperatorToken(Operator.Arrow,(1,12)), new OperatorToken(Operator.LeftParen,(1,14)),
                new KeywordToken(Keyword.Else,(1,15)), new OperatorToken(Operator.Assign,(1,19)), new OperatorToken(Operator.GreaterThan,(1,20)),
                new IdentifierToken("_",(1,22)), new OperatorToken(Operator.Equal,(1,23)), new IdentifierToken("int",(1,25)),
                new OperatorToken(Operator.RightParen,(1,28)), new OperatorToken(Operator.LeftBrace,(1,29)),
                new StringLiteralToken("as\nc\rc\tc",(1,30)),

                new OperatorToken(Operator.LessEqual,(2,0)), new IdentifierToken("p",(2,2)),new OperatorToken(Operator.GreaterThan,(2,3)),
                new DoubleLiteralToken(1.5E-3,(2,4)), new OperatorToken(Operator.NotEqual,(2,10)), new UInt64LiteralToken(231231231231,(2,12)),
                new OperatorToken(Operator.LessThan,(2,24)), new OperatorToken(Operator.Divide,(2,25)), new IdentifierToken("p",(2,26)),
                new OperatorToken(Operator.GreaterThan,(2,27)),

                new OperatorToken(Operator.RightBrace,(3,0)),
            };

            var lexer = new Lexer(sourceCode);
            var tokens = lexer.Parse().ToList();

            Assert.Equal(expectedTokens.Length, tokens.Count);

            for (var i = 0; i < expectedTokens.Length; i++)
            {
                var expectedToken = expectedTokens[i];
                var actualToken = tokens[i];

                AssertJsonEqual(expectedToken, actualToken);
            }

            // TODO: check both are end
        }
    }
}
