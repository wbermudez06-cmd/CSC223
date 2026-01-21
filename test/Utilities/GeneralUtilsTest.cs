using Xunit;
using Xunit.Sdk;

namespace Xunit.Sdk.Tests;

public class GeneralUtilsTests
{
    private readonly generalUtils _utils = new generalUtils();

    #region Contains Tests

    [Fact]
    public void Contains_ItemExists_ReturnsTrue()
    {
        int[] array = { 1, 2, 3, 4, 5 };
        Assert.True(_utils.Contains(array, 3));
    }

    [Fact]
    public void Contains_ItemDoesNotExist_ReturnsFalse()
    {
        int[] array = { 1, 2, 3, 4, 5 };
        Assert.False(_utils.Contains(array, 10));
    }

    [Fact]
    public void Contains_EmptyArray_ReturnsFalse()
    {
        int[] array = { };
        Assert.False(_utils.Contains(array, 1));
    }

    [Fact]
    public void Contains_StringArray_Works()
    {
        string[] array = { "hello", "world", "test" };
        Assert.True(_utils.Contains(array, "world"));
        Assert.False(_utils.Contains(array, "missing"));
    }

    #endregion

    #region GetIndentation Tests

    [Fact]
    public void GetIndentation_Level0_ReturnsEmptyString()
    {
        Assert.Equal("", _utils.GetIndentation(0));
    }

    [Fact]
    public void GetIndentation_Level1_Returns4Spaces()
    {
        Assert.Equal("    ", _utils.GetIndentation(1));
    }

    [Fact]
    public void GetIndentation_Level3_Returns12Spaces()
    {
        Assert.Equal("            ", _utils.GetIndentation(3));
    }

    [Fact]
    public void GetIndentation_Level5_Returns20Spaces()
    {
        string result = _utils.GetIndentation(5);
        Assert.Equal(20, result.Length);
        Assert.All(result, c => Assert.Equal(' ', c));
    }

    #endregion

    #region IsValidVariable Tests

    [Fact]
    public void IsValidVariable_AllLowercase_ReturnsTrue()
    {
        Assert.True(_utils.IsValidVariable("variable"));
    }

    [Fact]
    public void IsValidVariable_ContainsUppercase_ReturnsFalse()
    {
        Assert.False(_utils.IsValidVariable("Variable"));
    }

    [Fact]
    public void IsValidVariable_ContainsDigit_ReturnsFalse()
    {
        Assert.False(_utils.IsValidVariable("var1able"));
    }

    [Fact]
    public void IsValidVariable_ContainsUnderscore_ReturnsFalse()
    {
        Assert.False(_utils.IsValidVariable("var_iable"));
    }

    [Fact]
    public void IsValidVariable_EmptyString_ReturnsTrue()
    {
        Assert.True(_utils.IsValidVariable(""));
    }

    #endregion

    #region IsValidOperator Tests

    [Theory]
    [InlineData("+")]
    [InlineData("-")]
    [InlineData("*")]
    [InlineData("/")]
    [InlineData("//")]
    [InlineData("%")]
    [InlineData("**")]
    public void IsValidOperator_ValidOperators_ReturnsTrue(string op)
    {
        Assert.True(_utils.IsValidOperator(op));
    }

    [Theory]
    [InlineData("&")]
    [InlineData("||")]
    [InlineData("===")]
    [InlineData("^")]
    [InlineData("")]
    public void IsValidOperator_InvalidOperators_ReturnsFalse(string op)
    {
        Assert.False(_utils.IsValidOperator(op));
    }

    #endregion

    #region CountOccurrances Tests

    [Fact]
    public void CountOccurrances_CharacterExists_ReturnsCorrectCount()
    {
        Assert.Equal(3, _utils.CountOccurrances("hello world", 'l'));
    }

    [Fact]
    public void CountOccurrances_CharacterDoesNotExist_ReturnsZero()
    {
        Assert.Equal(0, _utils.CountOccurrances("hello", 'z'));
    }

    [Fact]
    public void CountOccurrances_EmptyString_ReturnsZero()
    {
        Assert.Equal(0, _utils.CountOccurrances("", 'a'));
    }

    [Fact]
    public void CountOccurrances_AllSameCharacter_ReturnsLength()
    {
        Assert.Equal(5, _utils.CountOccurrances("aaaaa", 'a'));
    }

    #endregion

    #region ToCamelCase Tests

    [Fact]
    public void ToCamelCase_SingleWord_ReturnsLowercase()
    {
        Assert.Equal("hello", _utils.ToCamelCase("hello"));
    }

    [Fact]
    public void ToCamelCase_TwoWords_ReturnsCamelCase()
    {
        Assert.Equal("helloWorld", _utils.ToCamelCase("hello world"));
    }

    [Fact]
    public void ToCamelCase_MultipleWords_ReturnsCamelCase()
    {
        Assert.Equal("thisIsATest", _utils.ToCamelCase("this is a test"));
    }

    [Fact]
    public void ToCamelCase_MixedCase_NormalizesToCamelCase()
    {
        Assert.Equal("helloWorld", _utils.ToCamelCase("HELLO WORLD"));
    }

    [Fact]
    public void ToCamelCase_EmptyString_ReturnsEmptyString()
    {
        Assert.Equal("", _utils.ToCamelCase(""));
    }

    #endregion

    #region IsPasswordStrong Tests

    [Fact]
    public void IsPasswordStrong_MeetsAllRequirements_ReturnsTrue()
    {
        Assert.True(_utils.IsPasswordStrong("Pass1234!"));
    }

    [Fact]
    public void IsPasswordStrong_TooShort_ReturnsFalse()
    {
        Assert.False(_utils.IsPasswordStrong("Pass1!"));
    }

    [Fact]
    public void IsPasswordStrong_NoLowercase_ReturnsFalse()
    {
        Assert.False(_utils.IsPasswordStrong("PASS1234!"));
    }

    [Fact]
    public void IsPasswordStrong_NoUppercase_ReturnsFalse()
    {
        Assert.False(_utils.IsPasswordStrong("pass1234!"));
    }

    [Fact]
    public void IsPasswordStrong_NoDigit_ReturnsFalse()
    {
        Assert.False(_utils.IsPasswordStrong("Password!"));
    }

    [Fact]
    public void IsPasswordStrong_NoSpecialChar_ReturnsFalse()
    {
        Assert.False(_utils.IsPasswordStrong("Password1"));
    }

    [Fact]
    public void IsPasswordStrong_ExactlyEightChars_ReturnsTrue()
    {
        Assert.True(_utils.IsPasswordStrong("Pass123!"));
    }

    #endregion

    #region GetUniqueItems Tests

    [Fact]
    public void GetUniqueItems_ListWithDuplicates_ReturnsUniqueItems()
    {
        var list = new List<int> { 1, 2, 2, 3, 3, 3, 4 };
        var result = _utils.GetUniqueItems(list);
        Assert.Equal(new List<int> { 1, 2, 3, 4 }, result);
    }

    [Fact]
    public void GetUniqueItems_ListWithoutDuplicates_ReturnsSameItems()
    {
        var list = new List<int> { 1, 2, 3, 4 };
        var result = _utils.GetUniqueItems(list);
        Assert.Equal(list, result);
    }

    [Fact]
    public void GetUniqueItems_EmptyList_ReturnsEmptyList()
    {
        var list = new List<int>();
        var result = _utils.GetUniqueItems(list);
        Assert.Empty(result);
    }

    [Fact]
    public void GetUniqueItems_NullList_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _utils.GetUniqueItems<int>(null));
    }

    [Fact]
    public void GetUniqueItems_DoesNotModifyOriginalList()
    {
        var list = new List<int> { 1, 2, 2, 3 };
        var originalCount = list.Count;
        _utils.GetUniqueItems(list);
        Assert.Equal(originalCount, list.Count);
    }

    #endregion

    #region CalculateAverage Tests

    [Fact]
    public void CalculateAverage_ValidArray_ReturnsCorrectAverage()
    {
        int[] numbers = { 1, 2, 3, 4, 5 };
        Assert.Equal(3.0, _utils.CalculateAverage(numbers));
    }

    [Fact]
    public void CalculateAverage_SingleElement_ReturnsElement()
    {
        int[] numbers = { 42 };
        Assert.Equal(42.0, _utils.CalculateAverage(numbers));
    }

    [Fact]
    public void CalculateAverage_NegativeNumbers_ReturnsCorrectAverage()
    {
        int[] numbers = { -10, -5, 0, 5, 10 };
        Assert.Equal(0.0, _utils.CalculateAverage(numbers));
    }

    [Fact]
    public void CalculateAverage_NullArray_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _utils.CalculateAverage(null));
    }

    [Fact]
    public void CalculateAverage_EmptyArray_ThrowsArgumentException()
    {
        int[] numbers = { };
        Assert.Throws<ArgumentException>(() => _utils.CalculateAverage(numbers));
    }

    [Fact]
    public void CalculateAverage_ReturnsDouble()
    {
        int[] numbers = { 1, 2 };
        var result = _utils.CalculateAverage(numbers);
        Assert.Equal(1.5, result);
    }

    #endregion

    #region Duplicates Tests

    [Fact]
    public void Duplicates_ArrayWithDuplicates_ReturnsDuplicateItems()
    {
        int[] array = { 1, 2, 2, 3, 3, 3, 4 };
        var result = _utils.Duplicates(array);
        Assert.Contains(2, result);
        Assert.Contains(3, result);
        Assert.DoesNotContain(1, result);
        Assert.DoesNotContain(4, result);
    }

    [Fact]
    public void Duplicates_NoDuplicates_ReturnsEmptyArray()
    {
        int[] array = { 1, 2, 3, 4, 5 };
        var result = _utils.Duplicates(array);
        Assert.Empty(result);
    }

    [Fact]
    public void Duplicates_EmptyArray_ReturnsEmptyArray()
    {
        int[] array = { };
        var result = _utils.Duplicates(array);
        Assert.Empty(result);
    }

    [Fact]
    public void Duplicates_AllSameElements_ReturnsSingleItem()
    {
        int[] array = { 5, 5, 5, 5 };
        var result = _utils.Duplicates(array);
        Assert.Single(result);
        Assert.Contains(5, result);
    }

    [Fact]
    public void Duplicates_StringArray_Works()
    {
        string[] array = { "a", "b", "b", "c", "c" };
        var result = _utils.Duplicates(array);
        Assert.Contains("b", result);
        Assert.Contains("c", result);
        Assert.DoesNotContain("a", result);
    }

    #endregion
}