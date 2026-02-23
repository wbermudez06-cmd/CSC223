using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DLL.Tests;

public class DLLTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_CreatesEmptyList()
    {
        var dll = new DLL<int>();
        Assert.Equal(0, dll.Size());
        Assert.True(dll.IsEmpty());
    }

    #endregion

    #region PushFront Tests

    [Fact]
    public void PushFront_AddsElementToEmptyList()
    {
        var dll = new DLL<int>();
        dll.PushFront(5);
        Assert.Equal(1, dll.Size());
        Assert.Equal(5, dll.Front());
    }

    [Fact]
    public void PushFront_AddsMultipleElements()
    {
        var dll = new DLL<int>();
        dll.PushFront(1);
        dll.PushFront(2);
        dll.PushFront(3);
        Assert.Equal(3, dll.Size());
        Assert.Equal(3, dll.Front());
        Assert.Equal(1, dll.Back());
    }

    [Theory]
    [InlineData("hello")]
    [InlineData("world")]
    [InlineData("")]
    public void PushFront_WorksWithStrings(string value)
    {
        var dll = new DLL<string>();
        dll.PushFront(value);
        Assert.Equal(value, dll.Front());
    }

    #endregion

    #region PushBack Tests

    [Fact]
    public void PushBack_AddsElementToEmptyList()
    {
        var dll = new DLL<int>();
        dll.PushBack(5);
        Assert.Equal(1, dll.Size());
        Assert.Equal(5, dll.Back());
    }

    [Fact]
    public void PushBack_AddsMultipleElements()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        Assert.Equal(3, dll.Size());
        Assert.Equal(1, dll.Front());
        Assert.Equal(3, dll.Back());
    }

    [Fact]
    public void PushBack_MaintainsOrder()
    {
        var dll = new DLL<int>();
        for (int i = 1; i <= 5; i++)
        {
            dll.PushBack(i);
        }
        Assert.Equal(1, dll.Front());
        Assert.Equal(5, dll.Back());
    }

    #endregion

    #region PopFront Tests

    [Fact]
    public void PopFront_RemovesAndReturnsFirstElement()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        int popped = dll.PopFront();
        Assert.Equal(1, popped);
        Assert.Equal(2, dll.Size());
        Assert.Equal(2, dll.Front());
    }

    [Fact]
    public void PopFront_OnSingleElement_MakesListEmpty()
    {
        var dll = new DLL<int>();
        dll.PushFront(42);
        
        int popped = dll.PopFront();
        Assert.Equal(42, popped);
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void PopFront_OnEmptyList_ThrowsException()
    {
        var dll = new DLL<int>();
        Assert.Throws<InvalidOperationException>(() => dll.PopFront());
    }

    [Fact]
    public void PopFront_MultiplePops_WorksCorrectly()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        Assert.Equal(1, dll.PopFront());
        Assert.Equal(2, dll.PopFront());
        Assert.Equal(3, dll.PopFront());
        Assert.True(dll.IsEmpty());
    }

    #endregion

    #region PopBack Tests

    [Fact]
    public void PopBack_RemovesAndReturnsLastElement()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        int popped = dll.PopBack();
        Assert.Equal(3, popped);
        Assert.Equal(2, dll.Size());
        Assert.Equal(2, dll.Back());
    }

    [Fact]
    public void PopBack_OnSingleElement_MakesListEmpty()
    {
        var dll = new DLL<int>();
        dll.PushBack(42);
        
        int popped = dll.PopBack();
        Assert.Equal(42, popped);
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void PopBack_OnEmptyList_ThrowsException()
    {
        var dll = new DLL<int>();
        Assert.Throws<InvalidOperationException>(() => dll.PopBack());
    }

    [Fact]
    public void PopBack_MultiplePops_WorksCorrectly()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        Assert.Equal(3, dll.PopBack());
        Assert.Equal(2, dll.PopBack());
        Assert.Equal(1, dll.PopBack());
        Assert.True(dll.IsEmpty());
    }

    #endregion

    #region Front Tests

    [Fact]
    public void Front_ReturnsFirstElement_WithoutRemoving()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        
        Assert.Equal(1, dll.Front());
        Assert.Equal(2, dll.Size()); // Size unchanged
    }

    [Fact]
    public void Front_OnEmptyList_ThrowsException()
    {
        var dll = new DLL<int>();
        Assert.Throws<InvalidOperationException>(() => dll.Front());
    }

    [Fact]
    public void Front_AfterPushFront_ReturnsNewElement()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushFront(2);
        Assert.Equal(2, dll.Front());
    }

    #endregion

    #region Back Tests

    [Fact]
    public void Back_ReturnsLastElement_WithoutRemoving()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        
        Assert.Equal(2, dll.Back());
        Assert.Equal(2, dll.Size()); // Size unchanged
    }

    [Fact]
    public void Back_OnEmptyList_ThrowsException()
    {
        var dll = new DLL<int>();
        Assert.Throws<InvalidOperationException>(() => dll.Back());
    }

    [Fact]
    public void Back_AfterPushBack_ReturnsNewElement()
    {
        var dll = new DLL<int>();
        dll.PushFront(1);
        dll.PushBack(2);
        Assert.Equal(2, dll.Back());
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_ItemExists_ReturnsTrue()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        Assert.True(dll.Contains(2));
    }

    [Fact]
    public void Contains_ItemDoesNotExist_ReturnsFalse()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        
        Assert.False(dll.Contains(5));
    }

    [Fact]
    public void Contains_OnEmptyList_ReturnsFalse()
    {
        var dll = new DLL<int>();
        Assert.False(dll.Contains(1));
    }

    [Fact]
    public void Contains_WithDefaultValue_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(0);
        Assert.True(dll.Contains(0));
    }

    [Theory]
    [InlineData("apple")]
    [InlineData("banana")]
    [InlineData("cherry")]
    public void Contains_WorksWithStrings(string value)
    {
        var dll = new DLL<string>();
        dll.PushBack("apple");
        dll.PushBack("banana");
        dll.PushBack("cherry");
        
        Assert.True(dll.Contains(value));
    }

    #endregion

    #region Remove Tests

    [Fact]
    public void Remove_ItemExists_RemovesAndReturnsTrue()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        bool removed = dll.Remove(2);
        Assert.True(removed);
        Assert.Equal(2, dll.Size());
        Assert.False(dll.Contains(2));
    }

    [Fact]
    public void Remove_ItemDoesNotExist_ReturnsFalse()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        
        bool removed = dll.Remove(5);
        Assert.False(removed);
        Assert.Equal(2, dll.Size());
    }

    [Fact]
    public void Remove_FirstElement_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        dll.Remove(1);
        Assert.Equal(2, dll.Front());
        Assert.Equal(2, dll.Size());
    }

    [Fact]
    public void Remove_LastElement_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        dll.Remove(3);
        Assert.Equal(2, dll.Back());
        Assert.Equal(2, dll.Size());
    }

    [Fact]
    public void Remove_OnlyElement_MakesListEmpty()
    {
        var dll = new DLL<int>();
        dll.PushBack(42);
        
        dll.Remove(42);
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void Remove_DuplicateValues_RemovesFirstOccurrence()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(2);
        dll.PushBack(3);
        
        dll.Remove(2);
        Assert.Equal(3, dll.Size());
        Assert.True(dll.Contains(2)); // Second occurrence still exists
    }

    #endregion

    #region Clear Tests

    [Fact]
    public void Clear_EmptiesList()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        
        dll.Clear();
        Assert.True(dll.IsEmpty());
        Assert.Equal(0, dll.Size());
    }

    [Fact]
    public void Clear_OnEmptyList_Works()
    {
        var dll = new DLL<int>();
        dll.Clear();
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void Clear_AllowsNewElements()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.Clear();
        dll.PushBack(2);
        
        Assert.Equal(1, dll.Size());
        Assert.Equal(2, dll.Front());
    }

    #endregion

    #region IsEmpty Tests

    [Fact]
    public void IsEmpty_OnNewList_ReturnsTrue()
    {
        var dll = new DLL<int>();
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void IsEmpty_AfterAddingElements_ReturnsFalse()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        Assert.False(dll.IsEmpty());
    }

    [Fact]
    public void IsEmpty_AfterRemovingAllElements_ReturnsTrue()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PopFront();
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void IsEmpty_AfterClear_ReturnsTrue()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.Clear();
        Assert.True(dll.IsEmpty());
    }

    #endregion

    #region Size Tests

    [Fact]
    public void Size_OnNewList_ReturnsZero()
    {
        var dll = new DLL<int>();
        Assert.Equal(0, dll.Size());
    }

    [Fact]
    public void Size_AfterPushFront_Increments()
    {
        var dll = new DLL<int>();
        dll.PushFront(1);
        Assert.Equal(1, dll.Size());
        dll.PushFront(2);
        Assert.Equal(2, dll.Size());
    }

    [Fact]
    public void Size_AfterPushBack_Increments()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        Assert.Equal(1, dll.Size());
        dll.PushBack(2);
        Assert.Equal(2, dll.Size());
    }

    [Fact]
    public void Size_AfterPop_Decrements()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PopFront();
        Assert.Equal(1, dll.Size());
    }

    [Fact]
    public void Size_AfterRemove_Decrements()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.Remove(1);
        Assert.Equal(1, dll.Size());
    }

    #endregion

    #region Indexer Tests

    [Fact]
    public void Indexer_Get_ReturnsCorrectValue()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        Assert.Equal(10, dll[0]);
        Assert.Equal(20, dll[1]);
        Assert.Equal(30, dll[2]);
    }

    [Fact]
    public void Indexer_Set_UpdatesValue()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        dll[1] = 99;
        Assert.Equal(99, dll[1]);
    }

    [Fact]
    public void Indexer_Set_OutOfRange_ThrowsException()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => dll[5] = 99);
    }

    [Fact]
    public void Indexer_Set_NegativeIndex_ThrowsException()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        
        Assert.Throws<ArgumentOutOfRangeException>(() => dll[-1] = 99);
    }

    #endregion

    #region IndexOf Tests

    [Fact]
    public void IndexOf_ItemExists_ReturnsCorrectIndex()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        Assert.Equal(0, dll.IndexOf(10));
        Assert.Equal(1, dll.IndexOf(20));
        Assert.Equal(2, dll.IndexOf(30));
    }

    [Fact]
    public void IndexOf_ItemDoesNotExist_ReturnsNegativeOne()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        
        Assert.Equal(-1, dll.IndexOf(99));
    }

    [Fact]
    public void IndexOf_EmptyList_ReturnsNegativeOne()
    {
        var dll = new DLL<int>();
        Assert.Equal(-1, dll.IndexOf(10));
    }

    [Fact]
    public void IndexOf_DuplicateItems_ReturnsFirstIndex()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(10);
        
        Assert.Equal(0, dll.IndexOf(10));
    }

    #endregion

    #region Insert (at index) Tests

    [Fact]
    public void Insert_AtBeginning_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(20);
        dll.PushBack(30);
        
        dll.Insert(0, 10);
        Assert.Equal(10, dll[0]);
        Assert.Equal(20, dll[1]);
        Assert.Equal(30, dll[2]);
    }

    [Fact]
    public void Insert_InMiddle_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(30);
        
        dll.Insert(1, 20);
        Assert.Equal(10, dll[0]);
        Assert.Equal(20, dll[1]);
        Assert.Equal(30, dll[2]);
    }

    [Fact]
    public void Insert_AtEnd_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        
        dll.Insert(2, 30);
        Assert.Equal(30, dll[2]);
    }

    #endregion

    #region RemoveAt Tests

    [Fact]
    public void RemoveAt_FirstElement_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        dll.RemoveAt(0);
        Assert.Equal(20, dll[0]);
        Assert.Equal(30, dll[1]);
    }

    [Fact]
    public void RemoveAt_MiddleElement_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        dll.RemoveAt(1);
        Assert.Equal(10, dll[0]);
        Assert.Equal(30, dll[1]);
    }

    [Fact]
    public void RemoveAt_LastElement_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        dll.RemoveAt(2);
        Assert.Equal(10, dll[0]);
        Assert.Equal(20, dll[1]);
    }

    #endregion

    #region Add Tests

    [Fact]
    public void Add_AppendsToEnd()
    {
        var dll = new DLL<int>();
        dll.Add(10);
        dll.Add(20);
        dll.Add(30);
        
        Assert.Equal(30, dll.Back());
        Assert.Equal(3, dll.Size());
    }

    [Fact]
    public void Add_ToEmptyList_Works()
    {
        var dll = new DLL<int>();
        dll.Add(42);
        
        Assert.Equal(42, dll.Front());
        Assert.Equal(42, dll.Back());
    }

    #endregion

    #region CopyTo Tests

    [Fact]
    public void CopyTo_CopiesToArray()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        int[] array = new int[5];
        dll.CopyTo(array, 0);
        
        Assert.Equal(10, array[0]);
        Assert.Equal(20, array[1]);
        Assert.Equal(30, array[2]);
    }

    [Fact]
    public void CopyTo_WithOffset_Works()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        
        int[] array = new int[5];
        dll.CopyTo(array, 2);
        
        Assert.Equal(0, array[0]);
        Assert.Equal(0, array[1]);
        Assert.Equal(10, array[2]);
        Assert.Equal(20, array[3]);
    }

    #endregion

    #region Enumerator Tests

    [Fact]
    public void GetEnumerator_IteratesAllElements()
    {
        var dll = new DLL<int>();
        dll.PushBack(10);
        dll.PushBack(20);
        dll.PushBack(30);
        
        var list = new List<int>();
        foreach (var item in dll)
        {
            list.Add(item);
        }
        
        Assert.Equal(new List<int> { 10, 20, 30 }, list);
    }

    [Fact]
    public void GetEnumerator_OnEmptyList_NoIterations()
    {
        var dll = new DLL<int>();
        
        int count = 0;
        foreach (var item in dll)
        {
            count++;
        }
        
        Assert.Equal(0, count);
    }

    [Fact]
    public void GetEnumerator_CanUseLinq()
    {
        var dll = new DLL<int>();
        dll.PushBack(1);
        dll.PushBack(2);
        dll.PushBack(3);
        dll.PushBack(4);
        
        var evenNumbers = dll.Where(x => x % 2 == 0).ToList();
        Assert.Equal(new List<int> { 2, 4 }, evenNumbers);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Integration_MixedOperations_WorksCorrectly()
    {
        var dll = new DLL<int>();
        
        dll.PushBack(1);
        dll.PushFront(0);
        dll.PushBack(2);
        // List: 0, 1, 2
        
        Assert.Equal(3, dll.Size());
        Assert.Equal(0, dll.Front());
        Assert.Equal(2, dll.Back());
        
        dll.Remove(1);
        // List: 0, 2
        
        Assert.Equal(2, dll.Size());
        Assert.Equal(0, dll.PopFront());
        // List: 2
        
        Assert.Equal(1, dll.Size());
        Assert.Equal(2, dll.Back());
    }

    [Fact]
    public void Integration_QueueBehavior_Works()
    {
        var dll = new DLL<string>();
        
        dll.PushBack("first");
        dll.PushBack("second");
        dll.PushBack("third");
        
        Assert.Equal("first", dll.PopFront());
        Assert.Equal("second", dll.PopFront());
        Assert.Equal("third", dll.PopFront());
        Assert.True(dll.IsEmpty());
    }

    [Fact]
    public void Integration_StackBehavior_Works()
    {
        var dll = new DLL<string>();
        
        dll.PushBack("first");
        dll.PushBack("second");
        dll.PushBack("third");
        
        Assert.Equal("third", dll.PopBack());
        Assert.Equal("second", dll.PopBack());
        Assert.Equal("first", dll.PopBack());
        Assert.True(dll.IsEmpty());
    }

    #endregion
}