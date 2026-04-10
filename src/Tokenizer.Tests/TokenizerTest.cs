using Xunit;
using System;
using Tokenizer;

namespace Tokenizer.Tests
{
    public class TokenizerTest
    {
        // ── Token: ToString ──────────────────────────────────────────────────

        [Theory]
        [InlineData("x",      TokenType.VARIABLE,   "VARIABLE,x")]
        [InlineData("return", TokenType.RETURN,      "RETURN,return")]
        [InlineData("42",     TokenType.INTEGER,     "INTEGER,42")]
        [InlineData("3.14",   TokenType.FLOAT,       "FLOAT,3.14")]
        [InlineData("+",      TokenType.OPERATOR,    "OPERATOR,+")]
        [InlineData(":=",     TokenType.ASSIGNMENT,  "ASSIGNMENT,:=")]
        [InlineData("(",      TokenType.LEFT_PAREN,  "LEFT_PAREN,(")]
        [InlineData(")",      TokenType.RIGHT_PAREN, "RIGHT_PAREN,)")]
        [InlineData("{",      TokenType.LEFT_CURLY,  "LEFT_CURLY,{")]
        [InlineData("}",      TokenType.RIGHT_CURLY, "RIGHT_CURLY,}")]
        public void Token_ToString_ReturnsCorrectFormat(string value, TokenType type, string expected)
        {
            var token = new Token(value, type);
            Assert.Equal(expected, token.ToString());
        }

        // ── Token: Equals ────────────────────────────────────────────────────

        [Fact]
        public void Token_Equals_SameValueAndType_ReturnsTrue()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            var t2 = new Token("x", TokenType.VARIABLE);
            Assert.True(t1.Equals(t2));
        }

        [Fact]
        public void Token_Equals_DifferentValue_ReturnsFalse()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            var t2 = new Token("y", TokenType.VARIABLE);
            Assert.False(t1.Equals(t2));
        }

        [Fact]
        public void Token_Equals_DifferentType_ReturnsFalse()
        {
            var t1 = new Token("+", TokenType.OPERATOR);
            var t2 = new Token("+", TokenType.ASSIGNMENT);
            Assert.False(t1.Equals(t2));
        }

        [Fact]
        public void Token_Equals_Null_ReturnsFalse()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            Assert.False(t1.Equals(null));
        }

        [Fact]
        public void Token_Equals_ObjectOverload_NonToken_ReturnsFalse()
        {
            var t1 = new Token("x", TokenType.VARIABLE);
            Assert.False(t1.Equals("not a token"));
        }

        // ── TokenConstants ───────────────────────────────────────────────────

        [Fact]
        public void TokenConstants_Values_AreCorrect()
        {
            Assert.Equal("+",  TokenConstants.PLUS);
            Assert.Equal("(",  TokenConstants.LEFT_PAREN);
            Assert.Equal("{",  TokenConstants.LEFT_CURLY);
            Assert.Equal(":=", TokenConstants.ASSIGNMENT);
            Assert.Equal(".",  TokenConstants.DECIMAL_POINT);
        }

        // ── Tokenizer: Full Statement (Table 1 from assignment) ───────────────

        [Fact]
        public void Tokenize_SimpleAssignment_ReturnsCorrectSequence()
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize("x := (1 + 2)");

            Assert.Equal(7, tokens.Count);
            Assert.True(tokens[0].Equals(new Token("x",  TokenType.VARIABLE)));
            Assert.True(tokens[1].Equals(new Token(":=", TokenType.ASSIGNMENT)));
            Assert.True(tokens[2].Equals(new Token("(",  TokenType.LEFT_PAREN)));
            Assert.True(tokens[3].Equals(new Token("1",  TokenType.INTEGER)));
            Assert.True(tokens[4].Equals(new Token("+",  TokenType.OPERATOR)));
            Assert.True(tokens[5].Equals(new Token("2",  TokenType.INTEGER)));
            Assert.True(tokens[6].Equals(new Token(")",  TokenType.RIGHT_PAREN)));
        }

        [Fact]
        public void Tokenize_ReturnStatement_ReturnsCorrectSequence()
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize("return x");

            Assert.Equal(2, tokens.Count);
            Assert.True(tokens[0].Equals(new Token("return", TokenType.RETURN)));
            Assert.True(tokens[1].Equals(new Token("x",      TokenType.VARIABLE)));
        }

        [Fact]
        public void Tokenize_CurlyBraces_RecognisedAsScope()
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize("{ x := 1 }");

            Assert.Equal(5, tokens.Count);
            Assert.True(tokens[0].Equals(new Token("{", TokenType.LEFT_CURLY)));
            Assert.True(tokens[4].Equals(new Token("}", TokenType.RIGHT_CURLY)));
        }

        // ── Tokenizer: Variables ──────────────────────────────────────────────

        [Theory]
        [InlineData("x")]
        [InlineData("abc")]
        [InlineData("myvar")]
        public void Tokenize_LowercaseAlpha_RecognisedAsVariable(string input)
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize(input);

            Assert.Single(tokens);
            Assert.True(tokens[0].Equals(new Token(input, TokenType.VARIABLE)));
        }

        // ── Tokenizer: Integers ───────────────────────────────────────────────

        [Theory]
        [InlineData("0")]
        [InlineData("42")]
        [InlineData("100")]
        public void Tokenize_Integer_RecognisedAsInteger(string input)
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize(input);

            Assert.Single(tokens);
            Assert.True(tokens[0].Equals(new Token(input, TokenType.INTEGER)));
        }

        // ── Tokenizer: Floats ─────────────────────────────────────────────────

        [Theory]
        [InlineData("3.14")]
        [InlineData("0.5")]
        [InlineData("10.01")]
        public void Tokenize_Float_RecognisedAsFloat(string input)
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize(input);

            Assert.Single(tokens);
            Assert.True(tokens[0].Equals(new Token(input, TokenType.FLOAT)));
        }

        [Fact]
        public void Tokenize_FloatMissingDecimalDigits_ThrowsArgumentException()
        {
            var tokenizer = new TokenizerImpl();
            Assert.Throws<ArgumentException>(() => tokenizer.Tokenize("3."));
        }

        // ── Tokenizer: Operators ──────────────────────────────────────────────

        [Theory]
        [InlineData("+")]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("%")]
        [InlineData("^")]
        public void Tokenize_SingleCharOperators_RecognisedAsOperator(string op)
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize(op);

            Assert.Single(tokens);
            Assert.True(tokens[0].Equals(new Token(op, TokenType.OPERATOR)));
        }

        [Fact]
        public void Tokenize_FloatDivision_RecognisedAsOperator()
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize("/");

            Assert.Single(tokens);
        }

        [Fact]
        public void Tokenize_IntegerDivision_RecognisedAsOperator()
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize("//");

            Assert.Single(tokens);
            Assert.True(tokens[0].Equals(new Token("//", TokenType.OPERATOR)));
        }

        // ── Tokenizer: Assignment ─────────────────────────────────────────────

        [Fact]
        public void Tokenize_Assignment_RecognisedAsAssignment()
        {
            var tokenizer = new TokenizerImpl();
            var tokens = tokenizer.Tokenize(":=");

            Assert.Single(tokens);
            Assert.True(tokens[0].Equals(new Token(":=", TokenType.ASSIGNMENT)));
        }

        [Fact]
        public void Tokenize_ColonWithoutEquals_ThrowsArgumentException()
        {
            var tokenizer = new TokenizerImpl();
            Assert.Throws<ArgumentException>(() => tokenizer.Tokenize(":"));
        }

        // ── Tokenizer: Whitespace & Empty Input ───────────────────────────────

        [Fact]
        public void Tokenize_EmptyString_ReturnsEmptyList()
        {
            var tokenizer = new TokenizerImpl();
            Assert.Empty(tokenizer.Tokenize(""));
        }

        [Fact]
        public void Tokenize_WhitespaceOnly_ReturnsEmptyList()
        {
            var tokenizer = new TokenizerImpl();
            Assert.Empty(tokenizer.Tokenize("   "));
        }

        // ── Tokenizer: Invalid Characters ─────────────────────────────────────

        [Theory]
        [InlineData("@")]
        [InlineData("$")]
        [InlineData("!")]
        [InlineData("#")]
        public void Tokenize_InvalidCharacter_ThrowsArgumentException(string input)
        {
            var tokenizer = new TokenizerImpl();
            Assert.Throws<ArgumentException>(() => tokenizer.Tokenize(input));
        }
    }
}
