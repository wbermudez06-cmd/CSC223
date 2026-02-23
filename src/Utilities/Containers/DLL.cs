/**
 * This file contains a generic implementation of a doubly linked list data
 * structure. The DLL class stores elements in nodes that maintain references
 * to both the previous and next nodes, allowing bidirectional traversal of the
 * list.
 *
 * Capabilities:
 * - Supports efficient insertion of elements at both the front and back of the list.
 * - Supports efficient removal of elements from both the front and back of the list.
 * - Allows removal/insertion of elements by value/node.
 * - Provides access to the first and last elements without removing them.
 * - Checking whether a value exists in the list.
 * - Maintains a count of elements in the list.
 * - Checking if the list is empty and allows the list to be cleared.
 *
 * @authors Willow Bermudez, Locke Dimmock
 * @date Jan 30, 2026
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;

/// <summary>
/// class implementation of a doubly linked list and its capabilities
/// </summary>
/// <typeparam name="T"></typeparam>
public class DLL<T> : IEnumerable<T>, IList<T>
{
    /// <summary>
    /// DNode creation class to make nodes
    /// </summary>
    protected class DNode
    {
        // Creates instance variables for a DNode
        public T value;
        public DNode prev;
        public DNode next;

        // Constructor that matches input values to instance 
        // variables for node creation
        public DNode(T data, DNode left = null, DNode right = null)
        {
            value = data;
            prev = left;
            next = right;
        }
    }

    // instance variables for a doubly linked list 
    // (sentinel nodes and size)
    private DNode head;
    private DNode tail;
    public int size;

    /// <summary>
    /// Constructor for DLL class, creates a doubly linked list
    /// </summary>
    public DLL()
    {
        // creates sentinel nodes
        tail = new DNode(default);
        head = new DNode(default);
        // points sentinel nodes to each other - indicates empty list
        tail.prev = head;
        head.next = tail;
        // sets size of the list to zero
        size = 0;
    }

    /// <summary>
    /// Method to insert a new node with a specified 
    /// item after a specified existing node
    /// </summary>
    /// <param name="node"></param>
    /// <param name="item"></param>
    private void Insert(DNode node, T item)
    {
        // saves the node to be shifted
        DNode shift_node = node.next;
        // creates a new node with the specified value,
        // places it before the shift node
        DNode new_node = new DNode(item, node, shift_node);
        // set where nodes point to account for this change
        node.next = new_node;
        shift_node.prev = new_node;
        // updates size of the list by 1
        size++;
    }

    /// <summary>
    /// Removes a specified node from the list by 
    // pointing next and previous to each other
    /// </summary>
    private void Remove(DNode node)
    {
        node.prev.next = node.next;
        node.next.prev = node.prev;
        // decreases size of the list by 1
        size--;
    }

    /// <summary>
    /// Returns a node based on a given index
    /// </summary>
    /// <param name="index"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private DNode GetNode(int index)
    {
        // throwns an exception if index is out of range
        if (index > size - 1)
        {
            throw new ArgumentOutOfRangeException();
        }
        // walks through list by index
        int curr_idx = 0;
        DNode curr_node = head.next;
        while (curr_idx != index)
        {
            curr_node = curr_node.next;
            curr_idx++;
        }
        // returns the node found at that index
        return curr_node;
    }

    /// <summary>
    /// Checks that an item is found within the list
    /// </summary>
    /// <param name="item"></param>
    public bool Contains(T item = default)
    {
        // walks through the list, returns true if the item is found
        DNode curr_node = head.next;
        while (curr_node != tail)
        {
            if (EqualityComparer<T>.Default.Equals(curr_node.value, item))
            {
                return true;
            }
            curr_node = curr_node.next;
        }
        // returns false if reaches end of the list and value isn't found
        return false;
    }

    /// <summary>
    /// Returns the size of the doubly linked list
    /// </summary>
    public int Size()
    {
        return size;
    }

    /// <summary>
    /// Returns a meaningful string that tells the size of
    /// the list and the elements within the list
    /// </summary>
    public String ToString()
    {
        // creates a list to save values to
        List<T> val_list = new List<T>();
        DNode curr_node = head.next;
        // walks through DLL, adding node values to the list
        while (curr_node != tail)
        {
            val_list.Add(curr_node.value);
            curr_node = curr_node.next;
        }
        // returns a meaningful string with size and elements in list
        return ($"There are {size} elements in this list: {val_list}");
    }

    /// <summary>
    /// Removes a node that has a specific value from the list
    /// </summary>
    /// <param name="item"></param>
    public bool Remove(T item)
    {
        // walks through the list checking if the value of a node
        // equals the specified item 
        DNode curr_node = head.next;
        while (curr_node != tail)
        {
            if (EqualityComparer<T>.Default.Equals(curr_node.value, item))
            {
                // removes node and returns true
                Remove(curr_node);
                return true;
            }
            curr_node = curr_node.next;
        }
        // returns false if value not found and node isn't removed
        return false;
    }

    /// <summary>
    /// Returns the value at the first node in the list
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T Front()
    {
        // throws an exception if trying to return an element 
        // from an empty list
        if (IsEmpty() is true)
        {
            throw new InvalidOperationException();
        }
        // returns the value of the node at front of the list
        return head.next.value;
    }

    /// <summary>
    /// Returns the value of the last node in the list
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T Back()
    {
        // throws an exception if trying to return an element
        // from an empty list
        if (IsEmpty() is true)
        {
            throw new InvalidOperationException();
        }
        // returns the value of the node at back of the list
        return tail.prev.value;
    }

    /// <summary>
    /// Adds a node with an input item to the front of the list
    /// </summary>
    /// <param name="item"></param>
    public void PushFront(T item)
    {
        Insert(head, item);
    }

    /// <summary>
    /// Adds a node with an input item to the back of the list
    /// </summary>
    /// <param name="item"></param>
    public void PushBack(T item)
    {
        Insert(tail.prev, item);
    }

    /// <summary>
    /// Removes and returns the value of the node at the front of the list
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T PopFront()
    {
        // throws an exception if trying to return from an empty list
        if (IsEmpty() is true)
        {
            throw new InvalidOperationException();
        }
        // saves the value before removing it
        DNode popped_node = head.next;
        Remove(head.next);
        // returns the value that was removed
        return popped_node.value;
    }

    /// <summary>
    /// Removes and returns the value of the node at the front of the list
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public T PopBack()
    {
        // throws an exception if trying to return from an empty list
        if (IsEmpty() is true)
        {
            throw new InvalidOperationException();
        }
        // saves the value before removing it
        DNode popped_node = tail.prev;
        Remove(tail.prev);
        // returns the value that was removed
        return popped_node.value;
    }

    /// <summary>
    /// Removes all the elements in the list, sets size back to 0
    /// </summary>
    public void Clear()
    {
        head.next = tail;
        tail.prev = head;
        size = 0;
    }

    /// <summary>
    /// Checks if the doubly linked list is empty
    /// </summary>
    public bool IsEmpty()
    {
        if (size == 0) return true;
        return false;
    }

    /// <summary>
    /// Classes to implement IEnumerator
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public T this[int index]
    {
        get
        {
            int i = 0;
            DNode current = head.next;

            while (i < index)
            {
                current = current.next;
                i++;
            }

            return current.value;
        }
        set
        {
            if (index < 0 || index >= size)
                throw new ArgumentOutOfRangeException(nameof(index));

            int i = 0;
            DNode current = head.next;

            while (i < index)
            {
                current = current.next;
                i++;
            }

            current.value = value;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        DNode cur_node = head.next;
        while (cur_node != tail)
        {
            yield return cur_node.value;
            cur_node = cur_node.next;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Classes to implement IList
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public int IndexOf(T item)
    {
        int idx = 0;
        DNode cur_node = head.next;
        while (cur_node != tail)
        {
            if (EqualityComparer<T>.Default.Equals(cur_node.value, item))
            {
                return idx;
            }
            cur_node = cur_node.next;
            idx++;
        }
        return idx - idx -1;
    }

    public void Insert(int index, T item)
    {
        DNode new_node = new DNode(item);
        DNode cur_node = head;
        int count = 0;
        while (count != index)
        {
            cur_node = cur_node.next;
            count++;
        }
        new_node.next = cur_node.next;
        new_node.prev = cur_node;
        cur_node.next.prev = new_node;
        cur_node.next = new_node;
    }

    public void RemoveAt(int index)
    {
        DNode cur_node = head.next;
        int count = 0;
        while (count != index)
        {
        cur_node = cur_node.next;
        count++;
        }
        cur_node.prev.next = cur_node.next;
        cur_node.next.prev = cur_node.prev;;
    }

    public void Add(T item)
    {
        DNode new_node = new DNode(item);
        new_node.prev = tail.prev;
        new_node.next = tail;
        tail.prev.next = new_node;
        tail.prev = new_node; ;
        size++;
    }

    void ICollection<T>.Clear()
    {
        Clear();
    }

    bool ICollection<T>.Contains(T item)
    {
        return Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        DNode cur_node = head.next;
        int index = arrayIndex;
        while (cur_node != tail)
        {
            array[index] = cur_node.value;
            cur_node = cur_node.next;
            index++;
        }
    }

    
   

   

    
}

