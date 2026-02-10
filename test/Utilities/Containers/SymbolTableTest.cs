using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Containers.Tests
{
    public class SymbolTableTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithoutParent_CreatesEmptyTable()
        {
            var table = new SymbolTable<string, int>();
            Assert.Equal(0, table.Count);
        }


        #endregion

        #region Add Tests

        [Theory]
        [InlineData("x", 100)]
        [InlineData("variable", 42)]
        [InlineData("test", -5)]
        public void Add_VariousKeyValuePairs_StoresCorrectly(string key, int value)
        {
            var table = new SymbolTable<string, int>();
            var kvp = new KeyValuePair<string, int>(key, value);
            table.Add(kvp);
            Assert.True(table.ContainsKey(key) is true);
        }

        [Fact]
        public void Add_DuplicateKey_ThrowsArgumentException()
        {
            var table = new SymbolTable<string, int>();
            var kvp1 = new KeyValuePair<string, int>("a", 10);
            var kvp2 = new KeyValuePair<string, int>("a", 20);
            table.Add(kvp1);
            Assert.Throws<ArgumentException>(() => table.Add(kvp2));
        }

        [Fact]
        public void Add_MultipleKeyValuePairs_IncreasesCount()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 1));
            table.Add(new KeyValuePair<string, int>("b", 2));
            table.Add(new KeyValuePair<string, int>("c", 3));
            Assert.Equal(3, table.Count);
        }

        #endregion

        #region ContainsKey Tests

        [Fact]
        public void ContainsKey_ExistingKey_ReturnsTrue()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 10));
            Assert.True(table.ContainsKey("a") is true);
        }

        [Fact]
        public void ContainsKey_NonExistingKey_ReturnsFalse()
        {
            var table = new SymbolTable<string, int>();
            Assert.True(table.ContainsKey("nonexistent") is false);
        }

        [Fact]
        public void ContainsKey_ChecksParentScope()
        {
            var parent = new SymbolTable<string, int>();
            parent.Add(new KeyValuePair<string, int>("a", 10));
            var child = new SymbolTable<string, int>(parent);
            Assert.True(child.ContainsKey("a") is true);
        }
        #endregion

        #region ContainsKeyLocal Tests

        [Fact]
        public void ContainsKeyLocal_ExistingKeyInCurrentScope_ReturnsTrue()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 10));
            Assert.True(table.ContainsKeyLocal("a") is true);
        }

        [Fact]
        public void ContainsKeyLocal_KeyOnlyInParent_ReturnsFalse()
        {
            var parent = new SymbolTable<string, int>();
            parent.Add(new KeyValuePair<string, int>("a", 10));
            var child = new SymbolTable<string, int>(parent);
            Assert.True(child.ContainsKeyLocal("a") is false);
        }

        [Theory]
        [InlineData("x")]
        [InlineData("y")]
        [InlineData("z")]
        public void ContainsKeyLocal_NonExistingKeys_ReturnsFalse(string key)
        {
            var table = new SymbolTable<string, int>();
            Assert.True(table.ContainsKeyLocal(key) is false);
        }

        [Fact]
        public void ContainsKeyLocal_NullKey_ThrowsArgumentNullException()
        {
            var table = new SymbolTable<string, int>();
            Assert.Throws<ArgumentNullException>(() => table.ContainsKeyLocal(null));
        }

        #endregion

        #region TryGetValue Tests

        [Fact]
        public void TryGetValue_ExistingKey_ReturnsTrueWithValue()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 42));
            bool result = table.TryGetValue("a", out int value);
            Assert.True(result is true);
            Assert.Equal(42, value);
        }

        [Fact]
        public void TryGetValue_NonExistingKey_ReturnsFalse()
        {
            var table = new SymbolTable<string, int>();
            bool result = table.TryGetValue("missing", out int value);
            Assert.True(result is false);
            Assert.Equal(default(int), value);
        }

        [Fact]
        public void TryGetValue_KeyInParentScope_ReturnsTrueWithValue()
        {
            var parent = new SymbolTable<string, int>();
            parent.Add(new KeyValuePair<string, int>("a", 100));
            var child = new SymbolTable<string, int>(parent);
            bool result = child.TryGetValue("a", out int value);
            Assert.True(result is true);
            Assert.Equal(100, value);
        }

        [Fact]
        public void TryGetValue_NullKey_ThrowsArgumentNullException()
        {
            var table = new SymbolTable<string, int>();
            Assert.Throws<ArgumentNullException>(() => table.TryGetValue(null, out int value));
        }

        #endregion

        #region TryGetValueLocal Tests

        [Fact]
        public void TryGetValueLocal_ExistingKeyInCurrentScope_ReturnsTrueWithValue()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 55));
            bool result = table.TryGetValueLocal("a", out int value);
            Assert.True(result is true);
            Assert.Equal(55, value);
        }

        [Fact]
        public void TryGetValueLocal_KeyOnlyInParent_ReturnsFalse()
        {
            var parent = new SymbolTable<string, int>();
            parent.Add(new KeyValuePair<string, int>("a", 100));
            var child = new SymbolTable<string, int>(parent);
            bool result = child.TryGetValueLocal("a", out int value);
            Assert.True(result is false);
            Assert.Equal(default(int), value);
        }

        [Fact]
        public void TryGetValueLocal_NullKey_ThrowsArgumentNullException()
        {
            var table = new SymbolTable<string, int>();
            Assert.Throws<ArgumentNullException>(() => table.TryGetValueLocal(null, out int value));
        }

        #endregion

        #region Remove and Clear Tests

        [Theory]
        [InlineData("x", 1)]
        [InlineData("y", 2)]
        [InlineData("z", 3)]
        public void Remove_ExistingKeyValuePair_RemovesAndReturnsTrue(string key, int value)
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("x", 1));
            table.Add(new KeyValuePair<string, int>("y", 2));
            table.Add(new KeyValuePair<string, int>("z", 3));
            var kvpToRemove = new KeyValuePair<string, int>(key, value);
            bool result = table.Remove(kvpToRemove);
            Assert.True(result is true);
            Assert.True(table.ContainsKey(key) is false);
            Assert.Equal(2, table.Count);
        }

        [Fact]
        public void Remove_NonExistingKeyValuePair_ReturnsFalse()
        {
            var table = new SymbolTable<string, int>();
            var kvp = new KeyValuePair<string, int>("missing", 0);
            Assert.True(table.Remove(kvp) is false);
        }

        [Fact]
        public void Clear_RemovesAllElements()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 1));
            table.Add(new KeyValuePair<string, int>("b", 2));
            table.Clear();
            Assert.Equal(0, table.Count);
            Assert.True(table.ContainsKey("a") is false);
        }

        #endregion

        #region Keys and Values Tests

        [Fact]
        public void Keys_ReturnsAllKeys()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 1));
            table.Add(new KeyValuePair<string, int>("b", 2));
            table.Add(new KeyValuePair<string, int>("c", 3));
            var keys = table.keys.ToList();
            Assert.Equal(3, keys.Count);
            Assert.Contains("a", keys);
            Assert.Contains("b", keys);
            Assert.Contains("c", keys);
        }

        [Fact]
        public void Values_ReturnsAllValues()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 10));
            table.Add(new KeyValuePair<string, int>("b", 20));
            table.Add(new KeyValuePair<string, int>("c", 30));
            var values = table.values.ToList();
            Assert.Equal(3, values.Count);
            Assert.Contains(10, values);
            Assert.Contains(20, values);
            Assert.Contains(30, values);
        }

        #endregion

        #region Shadowing Tests

        [Fact]
        public void Shadowing_ChildVariableHidesParentVariable()
        {
            var parent = new SymbolTable<string, int>();
            parent.Add(new KeyValuePair<string, int>("a", 10));
            var child = new SymbolTable<string, int>(parent);
            child.Add(new KeyValuePair<string, int>("a", 20));
            
            // Child should see shadowed value
            bool childResult = child.TryGetValue("a", out int childValue);
            Assert.True(childResult is true);
            Assert.Equal(20, childValue);
            
            // Parent should still have original value
            bool parentResult = parent.TryGetValue("a", out int parentValue);
            Assert.True(parentResult is true);
            Assert.Equal(10, parentValue);
        }

        [Fact]
        public void Shadowing_MultipleNestedScopes_AccessesCorrectValue()
        {
            var global = new SymbolTable<string, int>();
            global.Add(new KeyValuePair<string, int>("a", 10));
            global.Add(new KeyValuePair<string, int>("b", 5));
            
            var inner = new SymbolTable<string, int>(global);
            inner.Add(new KeyValuePair<string, int>("a", 20));
            inner.Add(new KeyValuePair<string, int>("c", 15));
            
            var innermost = new SymbolTable<string, int>(inner);
            innermost.Add(new KeyValuePair<string, int>("b", 30));
            
            // Verify innermost can see correct values from all scopes
            innermost.TryGetValue("a", out int aValue);
            Assert.Equal(20, aValue); // From inner scope
            
            innermost.TryGetValue("b", out int bValue);
            Assert.Equal(30, bValue); // From innermost scope
            
            innermost.TryGetValue("c", out int cValue);
            Assert.Equal(15, cValue); // From inner scope
        }

        [Fact]
        public void Shadowing_NullValueInChild_FallsBackToParent()
        {
            var parent = new SymbolTable<string, string>();
            parent.Add(new KeyValuePair<string, string>("x", "parent_value"));
            
            var child = new SymbolTable<string, string>(parent);
            child.Add(new KeyValuePair<string, string>("x", null));
            
            bool result = child.TryGetValue("x", out string value);
            Assert.True(result is true);
            Assert.Equal("parent_value", value);
        }

        [Fact]
        public void Shadowing_AssignmentScenario_BehavesCorrectly()
        {
            // Simulates: x := (10); { y := (20); x := (x + y) }
            // This test verifies the lookup behavior when a variable is
            // declared in inner scope with null but should read from parent
            var outer = new SymbolTable<string, int>();
            outer.Add(new KeyValuePair<string, int>("x", 10));
            
            var inner = new SymbolTable<string, int>(outer);
            inner.Add(new KeyValuePair<string, int>("y", 20));
            inner.Add(new KeyValuePair<string, int>("x", default)); // x declared but not assigned
            
            // When reading x before assignment in inner scope
            bool result = inner.TryGetValue("x", out int xValue);
            Assert.True(result is true);
            Assert.Equal(10, xValue); // Should get parent's value
            
            // Verify parent is unchanged
            outer.TryGetValue("x", out int outerX);
            Assert.Equal(10, outerX);
        }

        [Theory]
        [InlineData("a", 10, 20)]
        [InlineData("x", 5, 15)]
        [InlineData("var", 100, 200)]
        public void Shadowing_VariousVariables_MaintainsSeparateValues(string key, int parentValue, int childValue)
        {
            var parent = new SymbolTable<string, int>();
            parent.Add(new KeyValuePair<string, int>(key, parentValue));
            
            var child = new SymbolTable<string, int>(parent);
            child.Add(new KeyValuePair<string, int>(key, childValue));
            
            child.TryGetValue(key, out int childResult);
            Assert.Equal(childValue, childResult);
            
            parent.TryGetValue(key, out int parentResult);
            Assert.Equal(parentValue, parentResult);
        }

        #endregion

        #region Collection Interface Tests

        [Fact]
        public void Contains_ExistingKeyValuePair_ReturnsTrue()
        {
            var table = new SymbolTable<string, int>();
            var kvp = new KeyValuePair<string, int>("a", 10);
            table.Add(kvp);
            Assert.True(table.Contains(kvp) is true);
        }

        [Fact]
        public void Contains_WrongValue_ReturnsFalse()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 10));
            var kvp = new KeyValuePair<string, int>("a", 20);
            Assert.True(table.Contains(kvp) is false);
        }

        [Fact]
        public void CopyTo_CopiesAllElements()
        {
            var table = new SymbolTable<string, int>();
            table.Add(new KeyValuePair<string, int>("a", 1));
            table.Add(new KeyValuePair<string, int>("b", 2));
            var array = new KeyValuePair<string, int>[3];
            table.CopyTo(array, 0);
            Assert.Contains(new KeyValuePair<string, int>("a", 1), array);
            Assert.Contains(new KeyValuePair<string, int>("b", 2), array);
        }

        [Fact]
        public void IsReadOnly_ReturnsFalse()
        {
            var table = new SymbolTable<string, int>();
            Assert.True(table.IsReadOnly is false);
        }

        #endregion

        #region Complex DEC Scenario Tests

        [Fact]
        public void DECExample_GlobalAndInnerScope_WorksCorrectly()
        {
            // Simulates: a := (2 ** 4); b := (a + 5); { a := 30; c := ((a * b) // 3) }
            var global = new SymbolTable<string, int>();
            global.Add(new KeyValuePair<string, int>("a", 16));
            global.Add(new KeyValuePair<string, int>("b", 21));
            
            var inner = new SymbolTable<string, int>(global);
            inner.Add(new KeyValuePair<string, int>("a", 30));
            inner.Add(new KeyValuePair<string, int>("c", 112));
            
            // Verify shadowing - inner should see its own 'a'
            inner.TryGetValue("a", out int innerA);
            Assert.Equal(30, innerA);
            
            // Global should still have original 'a'
            global.TryGetValue("a", out int globalA);
            Assert.Equal(16, globalA);
            
            // Inner should access parent's 'b'
            inner.TryGetValue("b", out int innerB);
            Assert.Equal(21, innerB);
            
            // Inner has its own 'c'
            inner.TryGetValue("c", out int innerC);
            Assert.Equal(112, innerC);
            
            // Global should not have 'c'
            Assert.True(global.ContainsKey("c") is false);
        }

        #endregion
    }
}