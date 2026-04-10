/*
 * Description:
 * This file contains unit tests for the GeneralUtils class. Because all methods
 * in GeneralUtils are private, a nested TestableGeneralUtils class is used along
 * with reflection to invoke and test each private method.
 *
 * The tests verify correct behavior across a variety of normal, edge, and
 * error cases, including input validation, boundary conditions, and expected
 * exceptions.
 * @author Claude.io
 */
namespace Utilities.Tests
{
    /// <summary>
    /// Unit tests for the GeneralUtils class.
    /// </summary>
    public class GeneralUtilsTests
    {
        /// <summary>
        /// Helper class that exposes private methods of GeneralUtils for testing
        /// purposes using reflection.
        /// </summary>
        private class TestableGeneralUtils : GeneralUtils
        {
            public bool TestContains<T>(T[] array, T item)
            /// Generate test for the private Contains method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("Contains", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (bool)method?.MakeGenericMethod(typeof(T)).Invoke(this, new object[] { array, item })!;
            }

            public string TestGetIndentation(int level)
            /// Generates test for the private GetIndentation method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("GetIndentation", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (string)method?.Invoke(this, new object[] { level })!;
            }

            public bool TestIsValidVariable(string name)
            /// Generates test for the IsValidVariable method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("IsValidVariable", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (bool)method?.Invoke(this, new object[] { name })!;
            }

            public bool TestIsValidOperator(string op)
            /// Generates test for the private IsValidVariable method using reflection
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("IsValidOperator", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (bool)method?.Invoke(this, new object[] { op })!;
            }

            public int TestCountOccurrences(string s, char c)
            /// Generates test for the private CountOccurences method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("CountOccurences", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (int)method?.Invoke(this, new object[] { s, c })!;
            }

            public string TestToCamelCase(string s)
            /// Generates test for the private ToCamelCase method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("ToCamelCase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (string)method?.Invoke(this, new object[] { s })!;
            }

            public bool TestIsPasswordStrong(string pwd)
            /// Generates test for the private IsPasswordStrong method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("IsPasswordStrong", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (bool)method?.Invoke(this, new object[] { pwd })!;
            }

            public List<T> TestGetUniqueItems<T>(List<T> list)
            /// Generates test for the private GetUniqueItems method using reflection.
            /// Unwraps TargetInvocationException so the real exception is exposed.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("GetUniqueItems", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                try
                {
                    return (List<T>)method!.MakeGenericMethod(typeof(T))
                        .Invoke(this, new object[] { list })!;
                }
                catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException != null)
                {
                    // Rethrow the original exception thrown by GetUniqueItems
                    throw ex.InnerException; 
                }
            }

            public double TestCalculateAverage(int[] numbers)
            /// Generates test for the private CalculateAverage method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("CalculateAverage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (double)method?.Invoke(this, new object[] { numbers })!;
            }

            public T[] TestDuplicates<T>(T[] array)
            /// Generates test for the private Duplicates method using reflection.
            {
                var method = typeof(GeneralUtils)
                    .GetMethod("Duplicates", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (T[])method?.MakeGenericMethod(typeof(T)).Invoke(this, new object[] { array })!;
            }
        }

        /// <summary>
        /// Shared test utility instance used by all unit tests.
        /// </summary>
        private readonly TestableGeneralUtils _utils = new TestableGeneralUtils();

        /// <summary>
        /// Tests the Contains method with multiple scenarios.
        /// </summary>
        [Fact]
        public void Contains_VariousScenarios_ReturnsExpectedResults()
        {
            // checks that item exists, returns true
            int[] array1 = { 1, 2, 3, 4, 5 };
            Assert.True(_utils.TestContains(array1, 3));

            // checks that if item does not exist, returns false
            Assert.False(_utils.TestContains(array1, 10));

            // in case of empty array, return false
            int[] array2 = { };
            Assert.False(_utils.TestContains(array2, 1));

            // checks that method can be used with strings
            string[] array3 = { "apple", "banana", "cherry" };
            // returns true when found
            Assert.True(_utils.TestContains(array3, "banana"));
            // retuns false when not found
            Assert.False(_utils.TestContains(array3, "grape"));
        }

        /// <summary>
        /// Tests indentation generation for various indentation levels.
        /// </summary>
        [Fact]
        public void GetIndentation_VariousLevels_ReturnsCorrectSpacing()
        {
            // returns an empty indentation string 
            Assert.Equal("", _utils.TestGetIndentation(0));

            // returns correct number of indentation spaces, and verifies counted length
            string result = _utils.TestGetIndentation(4);
            Assert.Equal("    ", result);
            Assert.Equal(4, result.Length);

            // verifies by count
            Assert.Equal(9, _utils.TestGetIndentation(9).Length);
        }

        /// <summary>
        /// Tests variable name is lowercase.
        /// </summary>
        [Fact]
        public void IsValidVariable_VariousInputs_ReturnsExpectedResults()
        {
            // all lowercase - valid, returns true
            Assert.True(_utils.TestIsValidVariable("variable"));

            // contains uppercase - invalid, returns false
            Assert.False(_utils.TestIsValidVariable("Variable"));
            Assert.False(_utils.TestIsValidVariable("variAble"));

            // contains digits - returns true (implementation only checks ToLower())
            Assert.True(_utils.TestIsValidVariable("variable1"));
        }

        /// <summary>
        /// Tests operator validation.
        /// </summary>
        [Fact]
        public void IsValidOperator_VariousOperators_ReturnsExpectedResults()
        {
            // valid operators, returns true
            Assert.True(_utils.TestIsValidOperator("+"));
            Assert.True(_utils.TestIsValidOperator("-"));
            Assert.True(_utils.TestIsValidOperator("*"));
            Assert.True(_utils.TestIsValidOperator("/"));
            Assert.True(_utils.TestIsValidOperator("//"));
            Assert.True(_utils.TestIsValidOperator("%"));
            Assert.True(_utils.TestIsValidOperator("**"));

            // invalid operators, return false
            Assert.False(_utils.TestIsValidOperator("&"));
            Assert.False(_utils.TestIsValidOperator("^"));
            Assert.False(_utils.TestIsValidOperator("++"));
            Assert.False(_utils.TestIsValidOperator("==="));
            Assert.False(_utils.TestIsValidOperator(""));
            Assert.False(_utils.TestIsValidOperator("abc"));
        }

        /// <summary>
        /// Tests character occurrence counting.
        /// </summary>
        [Fact]
        public void CountOccurrences_VariousStrings_ReturnsCorrectCount()
        {
            // character exists multiple times
            Assert.Equal(3, _utils.TestCountOccurrences("hello world", 'l'));

            // caracter does not exist
            Assert.Equal(0, _utils.TestCountOccurrences("hello world", 'z'));

            // empty string
            Assert.Equal(0, _utils.TestCountOccurrences("", 'a'));

            // single character
            Assert.Equal(1, _utils.TestCountOccurrences("a", 'a'));
        }

        /// <summary>
        /// Tests camel-case string conversion behavior.
        /// </summary>
        [Fact]
        public void ToCamelCase_VariousInputs_ReturnsCorrectFormat()
        {
            // two words
            Assert.Equal("HelloWorld", _utils.TestToCamelCase("hello world"));

            // three words
            Assert.Equal("HelloWorldTest", _utils.TestToCamelCase("hello world test"));

            // single word
            Assert.Equal("hello", _utils.TestToCamelCase("Hello"));

            // different lower and uppercase words
            Assert.Equal("Helloworld", _utils.TestToCamelCase("hello World"));
        }

        /// <summary>
        /// Tests password strength validation.
        /// </summary>
        [Fact]
        public void IsPasswordStrong_VariousPasswords_ReturnsExpectedResults()
        {
            // valid password, returns true
            Assert.True(_utils.TestIsPasswordStrong("Abcdef1!"));

            // input is too short, returns false
            Assert.False(_utils.TestIsPasswordStrong("Abc1!"));

            // input includes no uppercase, returns false
            Assert.False(_utils.TestIsPasswordStrong("abcdef1!"));

            // input includes no lowercase, returns false
            Assert.False(_utils.TestIsPasswordStrong("ABCDEF1!"));

            // input includes no digits, return false
            Assert.False(_utils.TestIsPasswordStrong("Abcdefgh!"));

            // input includes no special character, return false
            Assert.False(_utils.TestIsPasswordStrong("Abcdef12"));

            // exactly 8 characters - valid input, returns true
            Assert.True(_utils.TestIsPasswordStrong("Abcdef1!"));
        }

        /// <summary>
        /// Tests retrieval of unique items from a list.
        /// </summary>
        [Fact]
        public void GetUniqueItems_VariousLists_ReturnsUniqueItems()
        {
            // List with duplicates
            List<int> list1 = new List<int> { 1, 2, 3, 2, 4, 3, 5 };
            List<int> result1 = _utils.TestGetUniqueItems(list1);
            Assert.Equal(5, result1.Count);
            Assert.Contains(1, result1);
            Assert.Contains(2, result1);
            Assert.Contains(3, result1);
            Assert.Contains(4, result1);
            Assert.Contains(5, result1);

            // list without duplicates
            List<string> list2 = new List<string> { "a", "b", "c" };
            Assert.Equal(3, _utils.TestGetUniqueItems(list2).Count);

            // empty list
            List<int> list3 = new List<int>();
            Assert.Empty(_utils.TestGetUniqueItems(list3));

            // null list throws exception
            Assert.Throws<ArgumentException>(() => _utils.TestGetUniqueItems<int>(null));
        }

        /// <summary>
        /// Tests average calculation for integer arrays.
        /// </summary>
        [Fact]
        public void CalculateAverage_VariousArrays_ReturnsCorrectAverage()
        {
            // valid array
            Assert.Equal(3.0, _utils.TestCalculateAverage(new int[] { 1, 2, 3, 4, 5 }));

            // single element in array
            Assert.Equal(5.0, _utils.TestCalculateAverage(new int[] { 5 }));

            // negative numbers included in array
            Assert.Equal(-4.0, _utils.TestCalculateAverage(new int[] { -2, -4, -6 }));

            // mixed numbers
            Assert.Equal(20.0, _utils.TestCalculateAverage(new int[] { 10, 20, 30 }));
        }

        /// <summary>
        /// Tests duplicate detection behavior.
        /// </summary>
        [Fact]
        public void Duplicates_VariousArrays_ReturnsItemsWithCountLessThanOrEqualTwo()
        {
            // array with duplicates (returns items with count <= 2)
            int[] array1 = { 1, 2, 3, 2, 4, 3, 5, 3 };
            int[] result1 = _utils.TestDuplicates(array1);
            // items: 1(count=1), 2(count=2), 3(count=3), 4(count=1), 5(count=1)
            // returns items with count <= 2: 1, 2, 4, 5
            Assert.Contains(1, result1);
            Assert.Contains(2, result1);
            Assert.Contains(4, result1);
            Assert.Contains(5, result1);
            Assert.DoesNotContain(3, result1);

            // no duplicates - all items have count=1
            string[] array2 = { "a", "b", "c" };
            Assert.Equal(3, _utils.TestDuplicates(array2).Length);

            // all same item - count > 2, returns empty
            int[] array3 = { 1, 1, 1, 1 };
            Assert.Empty(_utils.TestDuplicates(array3));

            // empty array
            int[] array4 = { };
            Assert.Empty(_utils.TestDuplicates(array4));
        }
    }
}
