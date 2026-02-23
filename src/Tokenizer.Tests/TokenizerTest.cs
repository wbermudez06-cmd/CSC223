using Xunit;
using System;
using System.Collections.Generic;
using Tokenizer;

namespace Tokenizer.Tests;

public class TokenizerImplTests
{
    private readonly TokenizerImpl _tokenizer = new TokenizerImpl();

    #region Basic Token Tests

    [Fact]
    public void Tokenize_EmptyString_ReturnsEmptyList()
    {
        var result = _tokenizer.Tokenize("");
        Assert.Empty(result);
    }

    [Fact]
    public void Tokenize_WhitespaceOnly_ReturnsEmptyList()
    {
        var result = _tokenizer.Tokenize("   \t\n  ");
        Assert.Empty(result);
    }

    #endregion

    #region Variable Tests

    [Fact]
    public void Tokenize_SingleVariable_ReturnsVariableToken()
    {
        var result = _tokenizer.Tokenize("variable");
        
        Assert.Single(result);
        Assert.Equal("variable", result[0].token);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
    }

    [Theory]
    [InlineData("x")]
    [InlineData("myVar")]
    [InlineData("var123")]
    [InlineData("_private")]
    [InlineData("camelCase")]
    [InlineData("CONSTANT")]
    public void Tokenize_VariableNames_ReturnsVariableTokens(string varName)
    {
        var result = _tokenizer.Tokenize(varName);
        
        Assert.Single(result);
        Assert.Equal(varName, result[0].token);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
    }

    [Fact]
    public void Tokenize_MultipleVariables_ReturnsMultipleTokens()
    {
        var result = _tokenizer.Tokenize("var1 var2 var3");
        
        Assert.Equal(3, result.Count);
        Assert.All(result, t => Assert.Equal(TokenType.VARIABLE, t.type));
    }

    [Fact]
    public void Tokenize_ReturnKeyword_ReturnsReturnToken()
    {
        var result = _tokenizer.Tokenize("return");
        
        Assert.Single(result);
        Assert.Equal("return", result[0].token);
        Assert.Equal(TokenType.RETURN, result[0].type);
    }

    [Fact]
    public void Tokenize_ReturnWithVariable_ReturnsBothTokens()
    {
        var result = _tokenizer.Tokenize("return x");
        
        Assert.Equal(2, result.Count);
        Assert.Equal(TokenType.RETURN, result[0].type);
        Assert.Equal(TokenType.VARIABLE, result[1].type);
    }

    #endregion

    #region Integer Tests

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("42")]
    [InlineData("123456")]
    public void Tokenize_Integer_ReturnsIntegerToken(string number)
    {
        var result = _tokenizer.Tokenize(number);
        
        Assert.Single(result);
        Assert.Equal(number, result[0].token);
        Assert.Equal(TokenType.INTEGER, result[0].type);
    }

    [Fact]
    public void Tokenize_MultipleIntegers_ReturnsMultipleTokens()
    {
        var result = _tokenizer.Tokenize("1 2 3 456");
        
        Assert.Equal(4, result.Count);
        Assert.All(result, t => Assert.Equal(TokenType.INTEGER, t.type));
    }

    #endregion

    #region Float Tests

    [Theory]
    [InlineData("3.14")]
    [InlineData("0.5")]
    [InlineData("123.456")]
    [InlineData("1.0")]
    public void Tokenize_Float_ReturnsFloatToken(string number)
    {
        var result = _tokenizer.Tokenize(number);
        
        Assert.Single(result);
        Assert.Equal(number, result[0].token);
        Assert.Equal(TokenType.FLOAT, result[0].type);
    }

    [Fact]
    public void Tokenize_FloatWithMultipleDecimals_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _tokenizer.Tokenize("3.14.15"));
    }

    [Fact]
    public void Tokenize_MixedIntegersAndFloats_ReturnsCorrectTypes()
    {
        var result = _tokenizer.Tokenize("1 2.5 3 4.0");
        
        Assert.Equal(4, result.Count);
        Assert.Equal(TokenType.INTEGER, result[0].type);
        Assert.Equal(TokenType.FLOAT, result[1].type);
        Assert.Equal(TokenType.INTEGER, result[2].type);
        Assert.Equal(TokenType.FLOAT, result[3].type);
    }

    #endregion

    #region Operator Tests - Single Character

    [Fact]
    public void Tokenize_PlusOperator_ReturnsPlusToken()
    {
        var result = _tokenizer.Tokenize("+");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.PLUS, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Fact]
    public void Tokenize_MinusOperator_ReturnsMinusToken()
    {
        var result = _tokenizer.Tokenize("-");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.MINUS, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Fact]
    public void Tokenize_ModulusOperator_ReturnsModulusToken()
    {
        var result = _tokenizer.Tokenize("%");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.MODULUS, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Theory]
    [InlineData("+")]
    [InlineData("-")]
    [InlineData("%")]
    public void Tokenize_SingleCharOperators_ReturnsCorrectTokens(string op)
    {
        var result = _tokenizer.Tokenize(op);
        
        Assert.Single(result);
        Assert.Equal(op, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    #endregion

    #region Operator Tests - Multiplication and Exponentiation

    [Fact]
    public void Tokenize_SingleAsterisk_ReturnsMultiplicationToken()
    {
        var result = _tokenizer.Tokenize("*");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.TIMES, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Fact]
    public void Tokenize_DoubleAsterisk_ReturnsExponentiationToken()
    {
        var result = _tokenizer.Tokenize("**");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.EXPONENTIATION, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Fact]
    public void Tokenize_MultiplicationInExpression_Works()
    {
        var result = _tokenizer.Tokenize("2 * 3");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.INTEGER, result[0].type);
        Assert.Equal(TokenConstants.TIMES, result[1].token);
        Assert.Equal(TokenType.INTEGER, result[2].type);
    }

    [Fact]
    public void Tokenize_ExponentiationInExpression_Works()
    {
        var result = _tokenizer.Tokenize("2 ** 3");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.INTEGER, result[0].type);
        Assert.Equal(TokenConstants.EXPONENTIATION, result[1].token);
        Assert.Equal(TokenType.INTEGER, result[2].type);
    }

    #endregion

    #region Operator Tests - Division

    [Fact]
    public void Tokenize_SingleSlash_ReturnsFloatDivisionToken()
    {
        var result = _tokenizer.Tokenize("/");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.FLOAT_DIVISION, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Fact]
    public void Tokenize_DoubleSlash_ReturnsIntegerDivisionToken()
    {
        var result = _tokenizer.Tokenize("//");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.INTEGER_DIVISION, result[0].token);
        Assert.Equal(TokenType.OPERATOR, result[0].type);
    }

    [Fact]
    public void Tokenize_FloatDivisionInExpression_Works()
    {
        var result = _tokenizer.Tokenize("10 / 2");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.INTEGER, result[0].type);
        Assert.Equal(TokenConstants.FLOAT_DIVISION, result[1].token);
        Assert.Equal(TokenType.INTEGER, result[2].type);
    }

    [Fact]
    public void Tokenize_IntegerDivisionInExpression_Works()
    {
        var result = _tokenizer.Tokenize("10 // 2");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.INTEGER, result[0].type);
        Assert.Equal(TokenConstants.INTEGER_DIVISION, result[1].token);
        Assert.Equal(TokenType.INTEGER, result[2].type);
    }

    #endregion

    #region Assignment Operator Tests

    [Fact]
    public void Tokenize_AssignmentOperator_ReturnsAssignmentToken()
    {
        var result = _tokenizer.Tokenize(":=");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.ASSIGNMENT, result[0].token);
        Assert.Equal(TokenType.ASSIGNMENT, result[0].type);
    }

    [Fact]
    public void Tokenize_ColonWithoutEquals_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _tokenizer.Tokenize(":"));
    }

    [Fact]
    public void Tokenize_ColonWithOtherChar_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _tokenizer.Tokenize(":x"));
    }

    [Fact]
    public void Tokenize_AssignmentInStatement_Works()
    {
        var result = _tokenizer.Tokenize("x := 5");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
        Assert.Equal(TokenType.ASSIGNMENT, result[1].type);
        Assert.Equal(TokenType.INTEGER, result[2].type);
    }

    #endregion

    #region Parenthesis Tests

    [Fact]
    public void Tokenize_LeftParenthesis_ReturnsLeftParenToken()
    {
        var result = _tokenizer.Tokenize("(");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.LEFT_PAREN, result[0].token);
        Assert.Equal(TokenType.LEFT_PAREN, result[0].type);
    }

    [Fact]
    public void Tokenize_RightParenthesis_ReturnsRightParenToken()
    {
        var result = _tokenizer.Tokenize(")");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.RIGHT_PAREN, result[0].token);
        Assert.Equal(TokenType.RIGHT_PAREN, result[0].type);
    }

    [Fact]
    public void Tokenize_MatchedParentheses_Works()
    {
        var result = _tokenizer.Tokenize("(x)");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.LEFT_PAREN, result[0].type);
        Assert.Equal(TokenType.VARIABLE, result[1].type);
        Assert.Equal(TokenType.RIGHT_PAREN, result[2].type);
    }

    [Fact]
    public void Tokenize_ExpressionWithParentheses_Works()
    {
        var result = _tokenizer.Tokenize("(2 + 3) * 4");
        
        Assert.Equal(7, result.Count);
        Assert.Equal(TokenType.LEFT_PAREN, result[0].type);
        Assert.Equal(TokenType.INTEGER, result[1].type);
        Assert.Equal(TokenType.OPERATOR, result[2].type);
        Assert.Equal(TokenType.INTEGER, result[3].type);
        Assert.Equal(TokenType.RIGHT_PAREN, result[4].type);
        Assert.Equal(TokenType.OPERATOR, result[5].type);
        Assert.Equal(TokenType.INTEGER, result[6].type);
    }

    #endregion

    #region Curly Brace Tests

    [Fact]
    public void Tokenize_LeftCurlyBrace_ReturnsLeftCurlyToken()
    {
        var result = _tokenizer.Tokenize("{");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.LEFT_CURLY, result[0].token);
        Assert.Equal(TokenType.LEFT_CURLY, result[0].type);
    }

    [Fact]
    public void Tokenize_RightCurlyBrace_ReturnsRightCurlyToken()
    {
        var result = _tokenizer.Tokenize("}");
        
        Assert.Single(result);
        Assert.Equal(TokenConstants.RIGHT_CURLY, result[0].token);
        Assert.Equal(TokenType.RIGHT_CURLY, result[0].type);
    }

    [Fact]
    public void Tokenize_MatchedCurlyBraces_Works()
    {
        var result = _tokenizer.Tokenize("{x}");
        
        Assert.Equal(3, result.Count);
        Assert.Equal(TokenType.LEFT_CURLY, result[0].type);
        Assert.Equal(TokenType.VARIABLE, result[1].type);
        Assert.Equal(TokenType.RIGHT_CURLY, result[2].type);
    }

    #endregion

    #region Complex Expression Tests

    [Fact]
    public void Tokenize_SimpleExpression_TokenizesCorrectly()
    {
        var result = _tokenizer.Tokenize("x + 5");
        
        Assert.Equal(3, result.Count);
        Assert.Equal("x", result[0].token);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
        Assert.Equal("+", result[1].token);
        Assert.Equal(TokenType.OPERATOR, result[1].type);
        Assert.Equal("5", result[2].token);
        Assert.Equal(TokenType.INTEGER, result[2].type);
    }

    [Fact]
    public void Tokenize_ComplexExpression_TokenizesCorrectly()
    {
        var result = _tokenizer.Tokenize("result := (a + b) * 2.5");
        
        Assert.Equal(9, result.Count);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
        Assert.Equal(TokenType.ASSIGNMENT, result[1].type);
        Assert.Equal(TokenType.LEFT_PAREN, result[2].type);
        Assert.Equal(TokenType.VARIABLE, result[3].type);
        Assert.Equal(TokenType.OPERATOR, result[4].type);
        Assert.Equal(TokenType.VARIABLE, result[5].type);
        Assert.Equal(TokenType.RIGHT_PAREN, result[6].type);
        Assert.Equal(TokenType.OPERATOR, result[7].type);
        Assert.Equal(TokenType.FLOAT, result[8].type);
    }

    [Fact]
    public void Tokenize_AllOperators_Works()
    {
        var result = _tokenizer.Tokenize("+ - * / // % **");
        
        Assert.Equal(7, result.Count);
        Assert.All(result, t => Assert.Equal(TokenType.OPERATOR, t.type));
    }

    [Fact]
    public void Tokenize_ExpressionWithoutSpaces_Works()
    {
        var result = _tokenizer.Tokenize("x+y*z");
        
        Assert.Equal(5, result.Count);
        Assert.Equal("x", result[0].token);
        Assert.Equal("+", result[1].token);
        Assert.Equal("y", result[2].token);
        Assert.Equal("*", result[3].token);
        Assert.Equal("z", result[4].token);
    }

    [Fact]
    public void Tokenize_ExpressionWithExtraSpaces_IgnoresSpaces()
    {
        var result = _tokenizer.Tokenize("  x   +   5  ");
        
        Assert.Equal(3, result.Count);
        Assert.Equal("x", result[0].token);
        Assert.Equal("+", result[1].token);
        Assert.Equal("5", result[2].token);
    }

    [Fact]
    public void Tokenize_ReturnStatement_Works()
    {
        var result = _tokenizer.Tokenize("return x + 1");
        
        Assert.Equal(4, result.Count);
        Assert.Equal(TokenType.RETURN, result[0].type);
        Assert.Equal(TokenType.VARIABLE, result[1].type);
        Assert.Equal(TokenType.OPERATOR, result[2].type);
        Assert.Equal(TokenType.INTEGER, result[3].type);
    }

    [Fact]
    public void Tokenize_NestedParentheses_Works()
    {
        var result = _tokenizer.Tokenize("  ((a + b) * (c - d))     ");
        
        Assert.Equal(13, result.Count);
        Assert.Equal(TokenType.LEFT_PAREN, result[0].type);
        Assert.Equal(TokenType.LEFT_PAREN, result[1].type);
        Assert.Equal(TokenType.RIGHT_PAREN, result[6].type);
        Assert.Equal(TokenType.RIGHT_PAREN, result[12].type);
    }

    #endregion

    #region Error Handling Tests

    [Theory]
    [InlineData("@")]
    [InlineData("#")]
    [InlineData("$")]
    [InlineData("&")]
    [InlineData("!")]
    public void Tokenize_UnexpectedCharacter_ThrowsException(string invalidChar)
    {
        Assert.Throws<ArgumentException>(() => _tokenizer.Tokenize(invalidChar));
    }

    [Fact]
    public void Tokenize_InvalidCharacterInExpression_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _tokenizer.Tokenize("x + @"));
    }

    [Fact]
    public void Tokenize_ColonAtEndOfString_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _tokenizer.Tokenize("x :"));
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public void Tokenize_NumberStartingWithZero_Works()
    {
        var result = _tokenizer.Tokenize("0123");
        
        Assert.Single(result);
        Assert.Equal("0123", result[0].token);
        Assert.Equal(TokenType.INTEGER, result[0].type);
    }

    [Fact]
    public void Tokenize_FloatStartingWithZero_Works()
    {
        var result = _tokenizer.Tokenize("0.5");
        
        Assert.Single(result);
        Assert.Equal("0.5", result[0].token);
        Assert.Equal(TokenType.FLOAT, result[0].type);
    }

    [Fact]
    public void Tokenize_VariableNamedReturn_IsKeyword()
    {
        var result = _tokenizer.Tokenize("return");
        
        Assert.Single(result);
        Assert.Equal(TokenType.RETURN, result[0].type);
    }

    [Fact]
    public void Tokenize_VariableSimilarToReturn_IsVariable()
    {
        var result = _tokenizer.Tokenize("returns");
        
        Assert.Single(result);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
    }

    [Fact]
    public void Tokenize_UnderscoreOnlyVariable_Works()
    {
        var result = _tokenizer.Tokenize("_");
        
        Assert.Single(result);
        Assert.Equal("_", result[0].token);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
    }

    [Fact]
    public void Tokenize_ConsecutiveOperators_Works()
    {
        var result = _tokenizer.Tokenize("+-*/");
        
        Assert.Equal(4, result.Count);
        Assert.All(result, t => Assert.Equal(TokenType.OPERATOR, t.type));
    }

    #endregion

    #region Real-World Example Tests

    [Fact]
    public void Tokenize_FunctionDefinition_Works()
    {
        var result = _tokenizer.Tokenize("{ x := 5 return x }");
        
        Assert.Equal(7, result.Count);
        Assert.Equal(TokenType.LEFT_CURLY, result[0].type);
        Assert.Equal(TokenType.VARIABLE, result[1].type);
        Assert.Equal(TokenType.ASSIGNMENT, result[2].type);
        Assert.Equal(TokenType.INTEGER, result[3].type);
        Assert.Equal(TokenType.RETURN, result[4].type);
        Assert.Equal(TokenType.VARIABLE, result[5].type);
        Assert.Equal(TokenType.RIGHT_CURLY, result[6].type);
    }

    [Fact]
    public void Tokenize_QuadraticFormula_Works()
    {
        var result = _tokenizer.Tokenize("x := (b + (b ** 2 - 4 * a * c)) / (2 * a)");
        
        Assert.Equal(23, result.Count);
        Assert.Equal("x", result[0].token);
        Assert.Equal(TokenType.ASSIGNMENT, result[1].type);
    }

    [Fact]
    public void Tokenize_MixedIntegerAndFloatOperations_Works()
    {
        var result = _tokenizer.Tokenize("result := 10 / 3 + 2.5 * 4");
        
        Assert.Equal(9, result.Count);
        Assert.Equal(TokenType.VARIABLE, result[0].type);
        Assert.Equal(TokenType.ASSIGNMENT, result[1].type);
        Assert.Equal(TokenType.INTEGER, result[2].type);
        Assert.Equal(TokenConstants.FLOAT_DIVISION, result[3].token);
        Assert.Equal(TokenType.INTEGER, result[4].type);
        Assert.Equal(TokenConstants.PLUS, result[5].token);
        Assert.Equal(TokenType.FLOAT, result[6].type);
        Assert.Equal(TokenConstants.TIMES, result[7].token);
        Assert.Equal(TokenType.INTEGER, result[8].type);
    }

    #endregion
}