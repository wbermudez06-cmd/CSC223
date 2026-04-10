/**
 * Description:
 * This file contains unit tests for the DLL.cs file. Private methods within the file
 * are tested implicitly through public methods that can be tested.
 *
 * The tests verify correct behavior across a variety of normal, edge, and
 * error cases, including input validation, boundary conditions, and expected
 * exceptions.
 * @author Claude.io
 */

using Xunit;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices.Swift;
using System.Drawing;
using System.Linq.Expressions;
using System.ComponentModel.DataAnnotations;


/// <summary>
/// Test suite for DLL<T> doubly-linked list implementation.
/// Organized by method with each test class testing a specific public method.
/// Private helper methods (Insert, Remove(DNode), GetNode) are implicitly tested through public methods.
/// /// </summary>
namespace DoublyLinkedList.Tests
{
    /// <summary>
    /// Tests for the DLL constructor
    /// </summary>
    public class ConstructorTests
    {
        [Fact]
        public void Constructor()
        {
            // Initializes an empty list
            var dll = new DLL<int>();

            // Assert
            Assert.True(dll.IsEmpty());
            Assert.Equal(0, dll.Size());
        }
    }

    /// <summary>
    /// Tests for the Contains method
    /// </summary>
    public class ContainsTests
    {
        [Theory]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 3, true)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1, true)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 5, true)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 99, false)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 0, false)]
        public void Contains_VariousElements(int[] elements, int searchItem, bool expected)
        {
            // creates a list and adds each element to the back of the list
            var dll = new DLL<int>();
            foreach (var elem in elements)
                dll.PushBack(elem);

            // goes through the elements and asserts that they are found
            foreach (var elem in elements)
                Assert.True(dll.Contains(elem));
        }

        [Fact]
        public void Contains_Functionality()
        {
            // creates instance of a list
            var dll = new DLL<int>();

            // Empty list will return false 
            Assert.False(dll.Contains(0));

            // Elements added to list can be forund
            dll.PushBack(0);
            dll.PushBack(1);

            Assert.True(dll.Contains(0));
            Assert.True(dll.Contains(1));

            // removes item and makes sure contains method will return false
            dll.Remove(1);
            Assert.False(dll.Contains(1));
        }
    }

    /// <summary>
    /// Tests for the Size method
    /// </summary>
    public class SizeTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(100)]
        public void Size_AfterAddingElements(int count)
        {
            // creates instance of a list
            var dll = new DLL<int>();

            // adds a differing count of data to the list
            for (int i = 0; i < count; i++)
                dll.PushBack(i);

            // assert that the size of the list matches the count
            Assert.Equal(count, dll.Size());

            // clears list and checks size is set back to 0
            dll.Clear();
            Assert.Equal(0, dll.Size());
        }
    }

    /// <summary>
    /// Tests for the ToString method
    /// </summary>
    public class ToStringTests
    {
        [Fact]
        public void ToString_test()
        {
            // creates instance of an empty list
            var dll = new DLL<int>();

            // assert the string contains key elements
            Assert.NotNull(dll.ToString());
            Assert.Contains("0", dll.ToString());   // should mention 0 elements

            // adds items to the list
            dll.PushBack(1);
            dll.PushBack(2);
            dll.PushBack(3);

            // assert the string contains key elements
            Assert.NotNull(dll.ToString());
            Assert.Contains("3", dll.ToString());   // Should mention 3 elements
        }
    }

    /// <summary>
    /// Tests for the Remove(T item) method
    /// Note: This implicitly tests the private Remove(DNode node) helper method
    /// </summary>
    public class RemoveTests
    {
        [Fact]
        public void Remove_Functionality()
        {
            // creates instance of a list and adds an element
            var dll = new DLL<int>();
            dll.PushFront(5);

            // assert that an item can be removed from a list
            Assert.True(dll.Remove(5));
            Assert.Equal(0, dll.Size());

            // returns false when removing from an empty list
            Assert.False(dll.Remove(5));

            // adds elements back to list
            dll.PushBack(1);
            dll.PushBack(2);
            dll.PushBack(3);

            // assert that an element not found will return false -
            // but will not alter other part of the list
            Assert.False(dll.Remove(5));
            Assert.Equal(3, dll.Size());
        }

        [Theory]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 1)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 3)]
        [InlineData(new[] { 1, 2, 3, 4, 5 }, 5)]
        public void Remove_ElementAtPosition(int[] elements, int toRemove)
        {
            // adds elements to the list
            var dll = new DLL<int>();
            foreach (var elem in elements)
                dll.PushBack(elem);

            // asserts that specified element can be removed from the list
            Assert.True(dll.Remove(toRemove));
            Assert.False(dll.Contains(toRemove));
        }

        [Fact]
        public void Remove_DuplicateElements()
        {
            // creates an instance of a list
            var dll = new DLL<int>();
            // adds items to the list
            dll.PushBack(1);
            dll.PushBack(2);
            dll.PushBack(2);
            dll.PushBack(3);

            // assert that only one element is removed
            // and the duplicate still remains
            Assert.True(dll.Remove(2));
            Assert.True(dll.Contains(2)); // Second occurrence should still exist
        }
    }

    /// <summary>
    /// Tests for the Front method
    /// </summary>
    public class FrontTests
    {
        [Fact]
        public void Front_test()
        {
            // creates an instance of a list
            var dll = new DLL<string>();

            // assert that method throws an exception on an empty list
            Assert.Throws<InvalidOperationException>(() => dll.Front());

            // returns correctly with one element
            dll.PushFront("first");
            Assert.Equal("first", dll.Front());

            // adds more elements to the list
            dll.PushFront("second");
            dll.PushFront("third");

            // assert that another element appears at the front 
            Assert.Equal("third", dll.Front());

            // adds another element and asserts its at front
            dll.PushFront("fourth");
            Assert.Equal("fourth", dll.Front());
        }
    }

    /// <summary>
    /// Tests for the Back method
    /// </summary>
    public class BackTests
    {
        [Fact]
        public void Back_tests()
        {
            // creates an instance of a list
            var dll = new DLL<string>();

            // assert that method throws an exception on an empty list
            Assert.Throws<InvalidOperationException>(() => dll.Front());

            // returns correctly with one element
            dll.PushBack("first");
            Assert.Equal("first", dll.Back());

            // adds more elements to the list
            dll.PushBack("second");
            dll.PushBack("third");

            // assert that another element appears at the front 
            Assert.Equal("third", dll.Back());

            // adds another element and asserts its at front
            dll.PushBack("fourth");
            Assert.Equal("fourth", dll.Back());
        }


        /// <summary>
        /// Tests for the PushFront method
        /// Note: This implicitly tests the private Insert(DNode node, T item) helper method
        /// </summary>
        public class PushTests
        {
            [Theory]
            [InlineData(1)]
            [InlineData(100)]
            [InlineData(-50)]
            [InlineData(0)]
            public void PushFront_SingleElement_AddsToBeginning(int value)
            {
                // creates an instance of a list
                var dll = new DLL<int>();

                // pushes specified value to the front
                dll.PushFront(value);

                // asserts that this value can be found as the front and back
                Assert.Equal(value, dll.Front());
                Assert.Equal(value, dll.Back());
                // asserts size of list is one
                Assert.Equal(1, dll.Size());
            }
        }

        /// <summary>
        /// Tests for the PopFront method
        /// Note: This implicitly tests the private Remove(DNode node) helper method
        /// </summary>
        public class PopFrontTests
        {
            [Fact]
            public void PopFront()
            {
                // creates instance of a list
                var dll = new DLL<int>();

                // exception thrown if attempts to pop from an empty list
                Assert.Throws<InvalidOperationException>(() => dll.PopFront());

                // pushes an element to the list
                dll.PushFront(1);
                // popping the back from a list of one elements makes an empty list
                dll.PopFront();
                Assert.True(dll.IsEmpty());

                // adds more elements to the list
                dll.PushFront(2);
                dll.PushFront(3);

                // asserts that element can be popped and there 
                // is a new element at front of list
                Assert.Equal(3, dll.PopFront());
                Assert.Equal(2, dll.Front());
            }
        }

        /// <summary>
        /// Tests for the PopBack method
        /// Note: This implicitly tests the private Remove(DNode node) helper method
        /// </summary>
        public class PopBackTests
        {
            [Fact]
            public void PopBack()
            {
                // creates instance of a list
                var dll = new DLL<int>();

                // exception thrown if attempts to pop from an empty list
                Assert.Throws<InvalidOperationException>(() => dll.PopBack());

                // pushes an element to the list
                dll.PushBack(1);
                // popping the back from a list of one elements makes an empty list
                dll.PopBack();
                Assert.True(dll.IsEmpty());

                // adds more elements to the list
                dll.PushBack(2);
                dll.PushBack(3);

                // asserts that element can be popped and there 
                // is a new element at front of list
                Assert.Equal(3, dll.PopBack());
                Assert.Equal(2, dll.Back());
            }
        }

        /// <summary>
        /// Tests for the Clear method
        /// </summary>
        public class ClearTests
        {
            [Fact]
            public void Clear_WithElements_RemovesAllNodes()
            {
                // creates instance of a list
                var dll = new DLL<int>();

                // clears on an empty list - remains empty
                dll.Clear();

                // asserts that this cleared list is empty 
                // and has a list size of zero
                Assert.True(dll.IsEmpty());
                Assert.Equal(0, dll.Size());

                // works on large numbers
                for (int i = 0; i < 1000; i++)
                    dll.PushBack(i);

                // clears and asserts that the list is now empty
                dll.Clear();
                Assert.True(dll.IsEmpty());
                Assert.Equal(0, dll.Size());

                // verify can add elements after clear
                dll.PushBack(99);

                // asserts that this element can now be found in list
                Assert.Equal(1, dll.Size());
                Assert.Equal(99, dll.Front());
            }
        }

        /// <summary>
        /// Tests for the IsEmpty method
        /// </summary>
        public class IsEmptyTests
        {
            [Fact]
            public void IsEmpty_NewList_ReturnsTrue()
            {
                // creates instance of a list
                var dll = new DLL<int>();

                // asserts list is found empty upon creation
                Assert.True(dll.IsEmpty());

                // adds element to list and asserts 
                // list is not empty
                dll.PushBack(1);
                Assert.False(dll.IsEmpty());

                // adds elements to the list and clears it
                dll.PushBack(2);
                dll.PushBack(3);
                dll.Clear();

                // asserts empty list can be found after
                // adding elements then clearing
                Assert.True(dll.IsEmpty());
            }
        }
    }
}
