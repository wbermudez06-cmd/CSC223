/**
 * This file contains the implementation of a generic <see cref="SymbolTable{TKey, TValue}"/>
 * that models a dictionary using two parallel doubly linked lists (DLLs) for keys and values.
 *
 * The SymbolTable supports scope-based behavior commonly found in programming languages,
 * including optional parent scopes used to represent nested environments. Each symbol table
 * instance mainFtains its own local bindings while allowing parent relationships to enable
 * variable shadowing and scope isolation.
 *
 * Key features of this implementation include:
 * - Prevention of duplicate keys within the same local scope
 * - Parallel storage of keys and values using doubly linked lists
 * - Local-scope lookup methods that do not search parent scopes
 * - Support for basic dictionary operations such as Add, Remove, Clear, Contains, and CopyTo
 * - Enumeration over key-value pairs
 *
 * @authors Willow Bermudez, Locke Dimmock
 * @date Feb 6, 2026
 */ 

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Xunit.Abstractions;

public interface IDictionary<TKey, TValue> :
System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<TKey, TValue>>,
System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<TKey, TValue>>;

// implement DLL class into this file
// in order to create two DLLs for the dictionary
public class SymbolTable<TKey, TValue> : IDictionary<TKey, TValue>
{
    // attribute DLLs to facilitate dictionary mapping
    public DLL<TKey> keys;
    public DLL<TValue> values;
    private readonly SymbolTable<TKey, TValue> _parent;

    /// <summary>
    /// Initializes a symbol table without a 
    /// specified parent scope
    /// </summary>
    public SymbolTable() : this(null) { }

    /// <summary>
    /// initializes symbol table when a parent is given
    /// </summary>
    /// <param name="parent"></param>
    public SymbolTable(SymbolTable<TKey, TValue> parent)
    {
        keys = new DLL<TKey>();
        values = new DLL<TValue>();
        _parent = parent;
    }

    /// <summary>
    /// non implemented requirements for the IDictionary interface
    /// </summary>
    public int Count => keys.size;

    public bool IsReadOnly => false;

    /// <summary>
    /// Adds a key and value pair to respective 
    /// DLL for mapping
    /// </summary>
    /// <param name="item"></param>
    /// <exception cref="ArgumentException"></exception>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        // throws an exception if the key already 
        // exists - no duplicates
        if (keys.Contains(item.Key))
        // change to ContainsKeyLocal + tests
        {
            throw new ArgumentException();
        }
        // adds the values to keys DLL and values DLL
        keys.Add(item.Key);     
        values.Add(item.Value); 
    }

    /// <summary>
    /// Clears both the keys and values DLLs,
    /// essentially clearing the whole dictionary
    /// </summary>
    public void Clear()
    {
        keys.Clear();
        values.Clear();
    }

    /// <summary>
    /// Coppies the key, value pairs to a dictionary
    /// from the information in the DLL.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="arrayIndex"></param>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        // save the array index as a variable
        int index = arrayIndex;
        // loops through by index while index < length of keys
        while (index < keys.size)
        {
            // adds the elements to the array, increasing index
            array[index] = new KeyValuePair<TKey, TValue>(keys[index], values[index]);
            index++;
        }
    }

    /// <summary>
    /// sets up the enumeration to increase elements
    /// to facilitate for loop iterations
    /// </summary>
    /// <returns></returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        int idx = 0;
        while (idx <= keys.size)
        {
            yield return new KeyValuePair<TKey, TValue>(keys[idx], values[idx]);
            idx--;
        }
    }

    /// <summary>
    /// Removes both the key and value from their 
    /// respective Doubly linked list
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        // make if statement for potentail of duplicate keys, remove both
        // saves if the key was removed
        int idx = keys.IndexOf(item.Key);
        bool rem_key = keys.Remove(item.Key);
        // saves if value was removed
        values.RemoveAt(idx);
        bool rem_val = values.Contains(item.Value);
        // returns true if both removals were successfull
        if (rem_key is true && rem_val is false) return true;
        // else returns false
        return false;
    }

    /// <summary>
    /// Checks that the dictionary contains both the key
    /// and the value and that they match up to each other.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool ContainsKey(TKey key)
    {
        if (key == null) throw new ArgumentException();
        if (ContainsKeyLocal(key)) return true;
        if (_parent != null) return _parent.ContainsKey(key);

        return false;
    }
    /// <summary>
    /// Checks if the local scope contains the key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool ContainsKeyLocal(TKey key)
    {
        foreach (TKey item in keys)
        {
            if (EqualityComparer<TKey>.Default.Equals(item, key)) return true;
        }
        return false;
        // if (keys.(item.Key) is false)
        // {
        //     // returns false if it is not found
        //     return false;
        // }
        // // saves the index where the matching key can be found
        // int key_idx = keys.IndexOf(item.Key);

        // // walks through the values DLL
        // int val_idx = 0;
        // foreach (TValue val in values)
        // {
        //     // once it gets to the value at the matching index for its 
        //     // mapped key it return if the value matches the imput value
        //     if (val_idx == key_idx)
        //     {
        //         return EqualityComparer<TValue>.Default.Equals(val, item.Value);
        //     }
        //     // increases index until it matches found index
        //     val_idx++;
        // }
        // // return false automaticaly if unable to execute
        // return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (TryGetValueLocal(key, out TValue localval))
        {
            value = localval;
            return true;
        }
        if (_parent != null) return _parent.TryGetValue(key, out value);
        value = default;
        return false;

    }
    /// <summary>
    /// Finds the value that mateches up to a specified 
    /// key in the dictionary.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public bool TryGetValueLocal(TKey key, out TValue value)
    {
        // throws an exception if input is null
        if (key == null)
        {
            throw new ArgumentNullException();
        }

        // finds the index that the key is at
        int key_idx = keys.IndexOf(key);
        // loops through DLL of values
        int count = 0;
        foreach (TValue val in values)
        {
            // if index of DLL matches to found key index,
            // save value and return true
            // USE INDEXING
            if (count == key_idx)
            {
                value = val;
                return true;
            }
            // increment
            count++;
        }
        // save some default value and return false
        value = default;
        return false;
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        if (item.Key == null) throw new ArgumentNullException(nameof(item.Key));

        int idx = keys.IndexOf(item.Key);
        if (idx >= 0)
        {
            if (EqualityComparer<TValue>.Default.Equals(values[idx], item.Value))
            {
                return true;
            }
        }

        if (_parent != null)
        {
            return _parent.Contains(item);
        }

        return false;
    }
}