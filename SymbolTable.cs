using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
    public SymbolTable()
    {
        keys = new DLL<TKey>();
        values = new DLL<TValue>();
    }

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
    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

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
    /// Checks that the dictionary contains both the key
    /// and the value and that they match up to each other.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        // first verifies if key is in the keys DLL
        if (keys.Contains(item.Key) is false)
        {
            // returns false if it is not found
            return false;
        }
        // saves the index where the matching key can be found
        int key_idx = keys.IndexOf(item.Key);

        // walks through the values DLL
        int val_idx = 0;
        foreach (TValue val in values)
        {
            // once it gets to the value at the matching index for its 
            // mapped key it return if the value matches the imput value
            if (val_idx == key_idx)
            {
                return EqualityComparer<TValue>.Default.Equals(val, item.Value);
            }
            // increases index until it matches found index
            val_idx++;
        }
        // return false automaticaly if unable to execute
        return false;
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
        // saves if the key was removed
        bool rem_key = keys.Remove(item.Key);
        // saves if value was removed
        bool rem_val = values.Remove(item.Value);
        // returns true if both removals were successfull
        if (rem_key is true && rem_val is true) return true;
        // else returns false
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
        // throws and exception if the input key is null
        if (key == null)
        {
            throw new ArgumentNullException();
        }
        // retuns if the key can be found in DLL or not
        return (keys.Contains(key));
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
            if (count == key_idx)
            {
                value = val;
                return true;
            }
            // increading one
            count++;
        }
        // save some default value and return false
        value = default;
        return false;
    } 
    

}