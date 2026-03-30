using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

public class SymbolTableTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_WithoutParent_CreatesEmptySymbolTable()
    {
        // Arrange & Act
        var symbolTable = new SymbolTable<string, int>();

        // Assert
        Assert.Equal(0, symbolTable.Count);
        Assert.NotNull(symbolTable.keys);
        Assert.NotNull(symbolTable.values);
    }

    [Fact]
    public void Constructor_WithParent_CreatesSymbolTableWithParentReference()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        var child = new SymbolTable<string, int>(parent);

        // Assert
        Assert.Empty(child); // Child is empty
        Assert.True(child.ContainsKey("x")); // But can access parent's keys
    }

    [Fact]
    public void Constructor_WithNullParent_WorksCorrectly()
    {
        // Arrange & Act
        var symbolTable = new SymbolTable<string, int>(null);

        // Assert
        Assert.Empty(symbolTable);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_CanCreateMultipleLevelsOfScopes(int levels)
    {
        // Arrange
        SymbolTable<string, int> current = new SymbolTable<string, int>();
        
        // Act
        for (int i = 0; i < levels; i++)
        {
            current.Add(new KeyValuePair<string, int>($"var{i}", i));
            current = new SymbolTable<string, int>(current);
        }

        // Assert
        Assert.Empty(current); // Innermost scope is empty
    }

    #endregion

    #region Count Tests

    [Fact]
    public void Count_ReturnsZero_WhenSymbolTableIsEmpty()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act & Assert
        Assert.Equal(0, symbolTable.Count);
    }

    [Fact]
    public void Count_ReturnsCorrectNumber_AfterAddingItems()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        symbolTable.Add(new KeyValuePair<string, int>("z", 30));

        // Assert
        Assert.Equal(3, symbolTable.Count);
    }

    [Fact]
    public void Count_ReturnsCorrectNumber_AfterRemovingItems()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));

        // Act
        symbolTable.Remove(new KeyValuePair<string, int>("x", 10));

        // Assert
        Assert.Single(symbolTable);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(10)]
    public void Count_ReturnsCorrectNumber_ForVariousQuantities(int itemCount)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        for (int i = 0; i < itemCount; i++)
        {
            symbolTable.Add(new KeyValuePair<string, int>($"key{i}", i));
        }

        // Assert
        Assert.Equal(itemCount, symbolTable.Count);
    }

    [Fact]
    public void Count_OnlyReturnsLocalCount_NotParentCount()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        parent.Add(new KeyValuePair<string, int>("y", 20));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("z", 30));

        // Act & Assert
        Assert.Single(child); // Only local items
        Assert.Equal(2, parent.Count);
    }

    #endregion

    #region IsReadOnly Tests

    [Fact]
    public void IsReadOnly_AlwaysReturnsFalse()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act & Assert
        Assert.False(symbolTable.IsReadOnly);
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse_EvenWithParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        var child = new SymbolTable<string, int>(parent);

        // Act & Assert
        Assert.False(child.IsReadOnly);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_AddsKeyValuePair_ToEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        symbolTable.Add(item);

        // Assert
        Assert.Single(symbolTable);
        Assert.True(symbolTable.ContainsKeyLocal("x"));
    }

    [Fact]
    public void Add_AddsMultipleKeyValuePairs()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        symbolTable.Add(new KeyValuePair<string, int>("z", 30));

        // Assert
        Assert.Equal(3, symbolTable.Count);
        Assert.True(symbolTable.ContainsKeyLocal("x"));
        Assert.True(symbolTable.ContainsKeyLocal("y"));
        Assert.True(symbolTable.ContainsKeyLocal("z"));
    }

    [Fact]
    public void Add_ThrowsArgumentException_WhenDuplicateKeyExists()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            symbolTable.Add(new KeyValuePair<string, int>("x", 20)));
    }

    [Fact]
    public void Add_AllowsSameKeyInChildScope_WhenKeyExistsInParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);

        // Act (should not throw)
        child.Add(new KeyValuePair<string, int>("x", 20));

        // Assert
        Assert.Single(child);
        Assert.True(child.ContainsKeyLocal("x"));
    }

    [Theory]
    [InlineData("key1", 100)]
    [InlineData("key2", 200)]
    [InlineData("key3", 300)]
    public void Add_WorksWithVariousKeyValuePairs(string key, int value)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        var item = new KeyValuePair<string, int>(key, value);

        // Act
        symbolTable.Add(item);

        // Assert
        Assert.True(symbolTable.ContainsKeyLocal(key));
        Assert.True(symbolTable.TryGetValueLocal(key, out int retrievedValue));
        Assert.Equal(value, retrievedValue);
    }

    [Fact]
    public void Add_WorksWithNullValues()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, string>();
        var item = new KeyValuePair<string, string>("x", null);

        // Act
        symbolTable.Add(item);

        // Assert
        Assert.True(symbolTable.ContainsKeyLocal("x"));
        Assert.True(symbolTable.TryGetValueLocal("x", out string value));
        Assert.Null(value);
    }

    [Fact]
    public void Add_MaintainsParallelStructure_BetweenKeysAndValues()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        symbolTable.Add(new KeyValuePair<string, int>("a", 1));
        symbolTable.Add(new KeyValuePair<string, int>("b", 2));
        symbolTable.Add(new KeyValuePair<string, int>("c", 3));

        // Assert
        Assert.True(symbolTable.TryGetValueLocal("a", out int val1));
        Assert.Equal(1, val1);
        Assert.True(symbolTable.TryGetValueLocal("b", out int val2));
        Assert.Equal(2, val2);
        Assert.True(symbolTable.TryGetValueLocal("c", out int val3));
        Assert.Equal(3, val3);
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_RemovesAllItems_FromSymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        symbolTable.Add(new KeyValuePair<string, int>("z", 30));

        // Act
        symbolTable.Clear();

        // Assert
        Assert.Empty(symbolTable);
        Assert.False(symbolTable.ContainsKeyLocal("x"));
        Assert.False(symbolTable.ContainsKeyLocal("y"));
        Assert.False(symbolTable.ContainsKeyLocal("z"));
    }

    [Fact]
    public void Clear_WorksOnEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        symbolTable.Clear();

        // Assert
        Assert.Empty(symbolTable);
    }

    [Fact]
    public void Clear_DoesNotAffectParentScope()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("y", 20));

        // Act
        child.Clear();

        // Assert
        Assert.Empty(child);
        Assert.Single(parent);
        Assert.True(parent.ContainsKeyLocal("x"));
    }

    [Fact]
    public void Clear_AllowsAddingAfterClearing()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Clear();

        // Act
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));

        // Assert
        Assert.Single(symbolTable);
        Assert.True(symbolTable.ContainsKeyLocal("y"));
    }

    #endregion

    #region CopyTo Tests

    [Fact]
    public void CopyTo_CopiesAllElements_ToArray()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        symbolTable.Add(new KeyValuePair<string, int>("z", 30));
        var array = new KeyValuePair<string, int>[3];

        // Act
        symbolTable.CopyTo(array, 0);

        // Assert
        Assert.Contains(new KeyValuePair<string, int>("x", 10), array);
        Assert.Contains(new KeyValuePair<string, int>("y", 20), array);
        Assert.Contains(new KeyValuePair<string, int>("z", 30), array);
    }

    [Fact]
    public void CopyTo_CopiesElements_StartingAtSpecifiedIndex()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        var array = new KeyValuePair<string, int>[5];

        // Act
        symbolTable.CopyTo(array, 2);

        // Assert
        Assert.Equal(default(KeyValuePair<string, int>), array[0]);
        Assert.Equal(default(KeyValuePair<string, int>), array[1]);
        Assert.NotEqual(default(KeyValuePair<string, int>), array[2]);
        Assert.NotEqual(default(KeyValuePair<string, int>), array[3]);
    }

    [Fact]
    public void CopyTo_WorksWithEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        var array = new KeyValuePair<string, int>[3];

        // Act
        symbolTable.CopyTo(array, 0);

        // Assert
        Assert.All(array, item => Assert.Equal(default(KeyValuePair<string, int>), item));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void CopyTo_WorksWithVariousStartIndices(int startIndex)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var array = new KeyValuePair<string, int>[5];

        // Act
        symbolTable.CopyTo(array, startIndex);

        // Assert
        Assert.NotEqual(default(KeyValuePair<string, int>), array[startIndex]);
    }

    #endregion

    #region GetEnumerator Tests

    [Fact]
    public void GetEnumerator_AllowsIterationOverKeyValuePairs()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        symbolTable.Add(new KeyValuePair<string, int>("z", 30));

        // Act
        var items = new List<KeyValuePair<string, int>>();
        foreach (var item in symbolTable)
        {
            items.Add(item);
        }

        // Assert
        Assert.Equal(3, items.Count);
        Assert.Contains(new KeyValuePair<string, int>("x", 10), items);
        Assert.Contains(new KeyValuePair<string, int>("y", 20), items);
        Assert.Contains(new KeyValuePair<string, int>("z", 30), items);
    }

    [Fact]
    public void GetEnumerator_WorksWithEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        var items = new List<KeyValuePair<string, int>>();
        foreach (var item in symbolTable)
        {
            items.Add(item);
        }

        // Assert
        Assert.Empty(items);
    }

    [Fact]
    public void GetEnumerator_OnlyIteratesLocalScope_NotParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("y", 20));

        // Act
        var items = new List<KeyValuePair<string, int>>();
        foreach (var item in child)
        {
            items.Add(item);
        }

        // Assert
        Assert.Single(items);
        Assert.Contains(new KeyValuePair<string, int>("y", 20), items);
        Assert.DoesNotContain(new KeyValuePair<string, int>("x", 10), items);
    }

    [Fact]
    public void GetEnumerator_CanBeUsedWithLinq()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        symbolTable.Add(new KeyValuePair<string, int>("z", 30));

        // Act
        var count = symbolTable.Count();
        var hasX = symbolTable.Any(kvp => kvp.Key == "x");

        // Assert
        Assert.Equal(3, count);
        Assert.True(hasX);
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void Remove_RemovesKeyValuePair_WhenExists()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        symbolTable.Add(new KeyValuePair<string, int>("y", 20));
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        bool result = symbolTable.Remove(item);

        // Assert
        Assert.True(result);
        Assert.Single(symbolTable);
        Assert.False(symbolTable.ContainsKeyLocal("x"));
    }

    [Fact]
    public void Remove_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var item = new KeyValuePair<string, int>("y", 20);

        // Act
        bool result = symbolTable.Remove(item);

        // Assert
        Assert.False(result);
        Assert.Single(symbolTable);
    }

    [Fact]
    public void Remove_ReturnsFalse_WhenKeyExistsButValueDoesNotMatch()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var item = new KeyValuePair<string, int>("x", 99);

        // Act
        bool result = symbolTable.Remove(item);

        // Assert
        Assert.False(result);
        Assert.Single(symbolTable);
        Assert.True(symbolTable.ContainsKeyLocal("x"));
    }

    [Fact]
    public void Remove_WorksOnSymbolTableWithOneItem()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        bool result = symbolTable.Remove(item);

        // Assert
        Assert.True(result);
        Assert.Empty(symbolTable);
    }

    [Theory]
    [InlineData("a", 1)]
    [InlineData("b", 2)]
    [InlineData("c", 3)]
    public void Remove_WorksWithVariousKeyValuePairs(string key, int value)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>(key, value));
        var item = new KeyValuePair<string, int>(key, value);

        // Act
        bool result = symbolTable.Remove(item);

        // Assert
        Assert.True(result);
        Assert.False(symbolTable.ContainsKeyLocal(key));
    }

    [Fact]
    public void Remove_DoesNotAffectParentScope()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));

        // Act
        child.Remove(new KeyValuePair<string, int>("x", 20));

        // Assert
        Assert.Empty(child);
        Assert.True(parent.ContainsKeyLocal("x"));
    }

    #endregion

    #region ContainsKey Tests

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExistsInLocalScope()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.ContainsKey("x");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.ContainsKey("y");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExistsInParentScope()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);

        // Act
        bool result = child.ContainsKey("x");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExistsInGrandparentScope()
    {
        // Arrange
        var grandparent = new SymbolTable<string, int>();
        grandparent.Add(new KeyValuePair<string, int>("x", 10));
        var parent = new SymbolTable<string, int>(grandparent);
        var child = new SymbolTable<string, int>(parent);

        // Act
        bool result = child.ContainsKey("x");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_ThrowsArgumentException_WhenKeyIsNull()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => symbolTable.ContainsKey(null));
    }

    [Theory]
    [InlineData("key1")]
    [InlineData("key2")]
    [InlineData("key3")]
    public void ContainsKey_WorksWithVariousKeys(string key)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>(key, 100));

        // Act
        bool result = symbolTable.ContainsKey(key);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyShadowsParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));

        // Act
        bool result = child.ContainsKey("x");

        // Assert
        Assert.True(result);
    }

    #endregion

    #region ContainsKeyLocal Tests

    [Fact]
    public void ContainsKeyLocal_ReturnsTrue_WhenKeyExistsInLocalScope()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.ContainsKeyLocal("x");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKeyLocal_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.ContainsKeyLocal("y");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ContainsKeyLocal_ReturnsFalse_WhenKeyOnlyExistsInParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);

        // Act
        bool result = child.ContainsKeyLocal("x");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ContainsKeyLocal_ReturnsTrue_WhenKeyExistsInBothLocalAndParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));

        // Act
        bool result = child.ContainsKeyLocal("x");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsKeyLocal_WorksWithEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        bool result = symbolTable.ContainsKeyLocal("x");

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("b")]
    [InlineData("c")]
    public void ContainsKeyLocal_WorksWithVariousKeys(string key)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>(key, 100));

        // Act
        bool result = symbolTable.ContainsKeyLocal(key);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region TryGetValue Tests

    [Fact]
    public void TryGetValue_ReturnsTrue_WhenKeyExistsInLocalScope()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.TryGetValue("x", out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(10, value);
    }

    [Fact]
    public void TryGetValue_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.TryGetValue("y", out int value);

        // Assert
        Assert.False(result);
        Assert.Equal(default(int), value);
    }

    [Fact]
    public void TryGetValue_ReturnsTrue_WhenKeyExistsInParentScope()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);

        // Act
        bool result = child.TryGetValue("x", out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(10, value);
    }

    [Fact]
    public void TryGetValue_ReturnsLocalValue_WhenKeyExistsInBothLocalAndParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));

        // Act
        bool result = child.TryGetValue("x", out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(20, value); // Should return local value, not parent's
    }

    [Fact]
    public void TryGetValue_ReturnsTrue_WhenKeyExistsInGrandparentScope()
    {
        // Arrange
        var grandparent = new SymbolTable<string, int>();
        grandparent.Add(new KeyValuePair<string, int>("x", 10));
        var parent = new SymbolTable<string, int>(grandparent);
        var child = new SymbolTable<string, int>(parent);

        // Act
        bool result = child.TryGetValue("x", out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(10, value);
    }

    [Theory]
    [InlineData("key1", 100)]
    [InlineData("key2", 200)]
    [InlineData("key3", 300)]
    public void TryGetValue_WorksWithVariousKeyValuePairs(string key, int expectedValue)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>(key, expectedValue));

        // Act
        bool result = symbolTable.TryGetValue(key, out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedValue, value);
    }

    [Fact]
    public void TryGetValue_WorksWithNullValues()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, string>();
        symbolTable.Add(new KeyValuePair<string, string>("x", null));

        // Act
        bool result = symbolTable.TryGetValue("x", out string value);

        // Assert
        Assert.True(result);
        Assert.Null(value);
    }

    #endregion

    #region TryGetValueLocal Tests

    [Fact]
    public void TryGetValueLocal_ReturnsTrue_WhenKeyExistsInLocalScope()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.TryGetValueLocal("x", out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(10, value);
    }

    [Fact]
    public void TryGetValueLocal_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));

        // Act
        bool result = symbolTable.TryGetValueLocal("y", out int value);

        // Assert
        Assert.False(result);
        Assert.Equal(default(int), value);
    }

    [Fact]
    public void TryGetValueLocal_ReturnsFalse_WhenKeyOnlyExistsInParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);

        // Act
        bool result = child.TryGetValueLocal("x", out int value);

        // Assert
        Assert.False(result);
        Assert.Equal(default(int), value);
    }

    [Fact]
    public void TryGetValueLocal_ReturnsTrue_WhenKeyExistsInBothLocalAndParent()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));

        // Act
        bool result = child.TryGetValueLocal("x", out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(20, value);
    }

    [Fact]
    public void TryGetValueLocal_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            symbolTable.TryGetValueLocal(null, out int value));
    }

    [Fact]
    public void TryGetValueLocal_WorksWithEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        bool result = symbolTable.TryGetValueLocal("x", out int value);

        // Assert
        Assert.False(result);
        Assert.Equal(default(int), value);
    }

    [Theory]
    [InlineData("a", 1)]
    [InlineData("b", 2)]
    [InlineData("c", 3)]
    public void TryGetValueLocal_WorksWithVariousKeyValuePairs(string key, int expectedValue)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>(key, expectedValue));

        // Act
        bool result = symbolTable.TryGetValueLocal(key, out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(expectedValue, value);
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_ReturnsTrue_WhenKeyValuePairExistsInLocalScope()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        bool result = symbolTable.Contains(item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenKeyExistsButValueDoesNotMatch()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var item = new KeyValuePair<string, int>("x", 99);

        // Act
        bool result = symbolTable.Contains(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>("x", 10));
        var item = new KeyValuePair<string, int>("y", 20);

        // Act
        bool result = symbolTable.Contains(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_ReturnsTrue_WhenKeyValuePairExistsInParentScope()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        bool result = child.Contains(item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenKeyExistsInParentButValueDoesNotMatch()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        var item = new KeyValuePair<string, int>("x", 99);

        // Act
        bool result = child.Contains(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_ReturnsTrue_WhenShadowingOccursAndLocalValueMatches()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));
        var item = new KeyValuePair<string, int>("x", 20);

        // Act
        bool result = child.Contains(item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenShadowingOccursAndLocalValueDoesNotMatch()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("x", 20));
        var item = new KeyValuePair<string, int>("x", 10); // Parent's value

        // Act
        bool result = child.Contains(item);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Contains_ThrowsArgumentNullException_WhenKeyIsNull()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        var item = new KeyValuePair<string, int>(null, 10);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => symbolTable.Contains(item));
    }

    [Fact]
    public void Contains_WorksWithNullValues()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, string>();
        symbolTable.Add(new KeyValuePair<string, string>("x", null));
        var item = new KeyValuePair<string, string>("x", null);

        // Act
        bool result = symbolTable.Contains(item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsTrue_WhenKeyValuePairExistsInGrandparentScope()
    {
        // Arrange
        var grandparent = new SymbolTable<string, int>();
        grandparent.Add(new KeyValuePair<string, int>("x", 10));
        var parent = new SymbolTable<string, int>(grandparent);
        var child = new SymbolTable<string, int>(parent);
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        bool result = child.Contains(item);

        // Assert
        Assert.True(result);
    }

    [Theory]
    [InlineData("key1", 100)]
    [InlineData("key2", 200)]
    [InlineData("key3", 300)]
    public void Contains_WorksWithVariousKeyValuePairs(string key, int value)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        symbolTable.Add(new KeyValuePair<string, int>(key, value));
        var item = new KeyValuePair<string, int>(key, value);

        // Act
        bool result = symbolTable.Contains(item);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Contains_ReturnsFalse_WhenEmptySymbolTable()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();
        var item = new KeyValuePair<string, int>("x", 10);

        // Act
        bool result = symbolTable.Contains(item);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Integration and Complex Scenario Tests

    [Fact]
    public void ComplexScenario_MultiLevelScopeWithShadowing()
    {
        // Arrange
        var global = new SymbolTable<string, int>();
        global.Add(new KeyValuePair<string, int>("x", 1));
        global.Add(new KeyValuePair<string, int>("y", 2));

        var function = new SymbolTable<string, int>(global);
        function.Add(new KeyValuePair<string, int>("x", 10));
        function.Add(new KeyValuePair<string, int>("z", 30));

        var block = new SymbolTable<string, int>(function);
        block.Add(new KeyValuePair<string, int>("x", 100));

        // Act & Assert
        Assert.True(block.TryGetValue("x", out int x));
        Assert.Equal(100, x);

        Assert.True(block.TryGetValue("y", out int y));
        Assert.Equal(2, y);

        Assert.True(block.TryGetValue("z", out int z));
        Assert.Equal(30, z);

        Assert.False(block.TryGetValueLocal("y", out _));
        Assert.True(block.ContainsKeyLocal("x"));
    }

    [Fact]
    public void ComplexScenario_AddRemoveClear()
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act & Assert - Add
        symbolTable.Add(new KeyValuePair<string, int>("a", 1));
        symbolTable.Add(new KeyValuePair<string, int>("b", 2));
        symbolTable.Add(new KeyValuePair<string, int>("c", 3));
        Assert.Equal(3, symbolTable.Count);

        // Remove
        symbolTable.Remove(new KeyValuePair<string, int>("b", 2));
        Assert.Equal(2, symbolTable.Count);
        Assert.False(symbolTable.ContainsKeyLocal("b"));

        // Clear
        symbolTable.Clear();
        Assert.Empty(symbolTable);
    }

    [Fact]
    public void ComplexScenario_EnumerationWithParentScope()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        parent.Add(new KeyValuePair<string, int>("y", 20));

        var child = new SymbolTable<string, int>(parent);
        child.Add(new KeyValuePair<string, int>("z", 30));

        // Act
        var childItems = new List<KeyValuePair<string, int>>();
        foreach (var item in child)
        {
            childItems.Add(item);
        }

        // Assert
        Assert.Single(childItems);
        Assert.Contains(new KeyValuePair<string, int>("z", 30), childItems);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void StressTest_AddAndRetrieveMultipleItems(int itemCount)
    {
        // Arrange
        var symbolTable = new SymbolTable<string, int>();

        // Act
        for (int i = 0; i < itemCount; i++)
        {
            symbolTable.Add(new KeyValuePair<string, int>($"key{i}", i * 10));
        }

        // Assert
        Assert.Equal(itemCount, symbolTable.Count);
        for (int i = 0; i < itemCount; i++)
        {
            Assert.True(symbolTable.TryGetValue($"key{i}", out int value));
            Assert.Equal(i * 10, value);
        }
    }

    [Fact]
    public void ComplexScenario_FourLevelHierarchy()
    {
        // Arrange
        var level1 = new SymbolTable<string, string>();
        level1.Add(new KeyValuePair<string, string>("a", "level1"));

        var level2 = new SymbolTable<string, string>(level1);
        level2.Add(new KeyValuePair<string, string>("b", "level2"));

        var level3 = new SymbolTable<string, string>(level2);
        level3.Add(new KeyValuePair<string, string>("c", "level3"));

        var level4 = new SymbolTable<string, string>(level3);
        level4.Add(new KeyValuePair<string, string>("d", "level4"));

        // Act & Assert
        Assert.True(level4.TryGetValue("a", out string valA));
        Assert.Equal("level1", valA);
        Assert.True(level4.TryGetValue("b", out string valB));
        Assert.Equal("level2", valB);
        Assert.True(level4.TryGetValue("c", out string valC));
        Assert.Equal("level3", valC);
        Assert.True(level4.TryGetValue("d", out string valD));
        Assert.Equal("level4", valD);
    }

    [Fact]
    public void ComplexScenario_MixedOperations()
    {
        // Arrange
        var parent = new SymbolTable<string, int>();
        parent.Add(new KeyValuePair<string, int>("x", 10));
        
        var child = new SymbolTable<string, int>(parent);
        
        // Act & Assert
        child.Add(new KeyValuePair<string, int>("y", 20));
        Assert.Single(child);
        
        child.Add(new KeyValuePair<string, int>("x", 30)); // Shadow parent
        Assert.Equal(2, child.Count);
        
        Assert.True(child.TryGetValue("x", out int x));
        Assert.Equal(30, x); // Local value
        
        child.Remove(new KeyValuePair<string, int>("x", 30));
        Assert.Single(child);
        
        Assert.True(child.TryGetValue("x", out x));
        Assert.Equal(10, x); // Now gets parent value
    }

    #endregion
}