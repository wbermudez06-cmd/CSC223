/**
 * This file defines the GeneralUtils class, which contains a collection of
 * private utility methods for working with arrays, strings, and collections.
 * These methods support common validation, transformation, and analysis tasks
 * such as checking for duplicates, validating input formats, and computing
 * simple statistics.
 *
 * Notes:
 * - All methods are private and intended for internal use only.
 * - No external state is modified by these methods.
 * 
 * @author Willow Bermudez, Locke Dimmock
 * @date Jan 21, 2026
*/
namespace Utilities;

/// <summary>
/// General utility methods for working with arrays, strings, and collections.
/// </summary>
public class GeneralUtils
{
    /// <summary>
    /// Checks whether a specific item exists in the given array.
    /// </summary>
    /// <param name="array">The array to search.</param>
    /// <param name="item">The item to locate.</param>
    /// <returns>
    /// True if the item is found in the array; otherwise, false.
    /// </returns>
    private bool Contains<T>(T[] array, T item)
    {
        // creates variable index to track
        // loops through the array by index 
        int index = 0;
        while (index < array.Length)
        {
            // returns true if the item is found
            if (array[index].Equals(item)) return true;
            index++;
        }
        // returns false if the item is not found
        return false;
    }

    /// <summary>
    /// Generates a string containing spaces for indentation.
    /// </summary>
    /// <param name="level">The indentation level.</param>
    /// <returns>A string consisting of spaces.</returns>
    private string GetIndentation(int level)
    /// returns string with spaces of indicated amount
    {
        // creates a string variable to return
        // creates a track number for counting
        string result = "";
        int track_num = 0;
        // adds spaces to the result until the it reaches input amount
        while (track_num != level)
        {
            result += " ";
            track_num++;
        }
        // returns the string of indentations
        return result;
    }

    /// <summary>
    /// Determines whether a variable name contains only lowercase letters.
    /// </summary>
    /// <param name="name">The variable name to validate.</param>
    /// <returns>
    /// True if the name contains no uppercase letters; otherwise, false.
    /// </returns>
    private bool IsValidVariable(String name)
    {
        // checks if input string is all lowercase
        if (name == name.ToLower())
        {
            // returns true if it is
            return true;
        }
        // returns false if not
        return false;
    }

    /// <summary>
    /// Checks whether a string is a valid operator.
    /// </summary>
    /// <param name="op">The possible operator string.</param>
    /// <returns>True if the operator is valid; otherwise, false.</returns>
    private bool IsValidOperator(string op)
    {
        // creates a list of comparable operator strings
        List<string> operators = new List<string> { "+", "-", "*", "/", "//", "%", "**" };
        // loops through compable list
        foreach (string check in operators)
        {
            // checks if input matches an element in the comparble list
            if (op == check)
            {
                // returns true if if does
                return true;
            }
        }
        // returns false if not
        return false;
    }

    /// <summary>
    /// Counts how many times a character appears in a string.
    /// </summary>
    /// <param name="s">The string to look through.</param>
    /// <param name="c">The character to count.</param>
    /// <returns>The number of occurrences of the character.</returns>
    private int CountOccurences(string s, char c)
    {
        // creates variable for counting appearances, starting at 0
        int appears = 0;
        // loops through characters in the string
        foreach (char check in s)
        {
            // if indicated character appears, increase its count by 1
            if (check == c) appears++;
        }
        // return the amount counted
        return appears;
    }

    /// <summary>
    /// Converts a space-separated string into a camel-style format by changing to 
    /// lowercase or uppercase the first character of each word.
    /// </summary>
    /// <param name="s">The input string.</param>
    /// <returns>The changed string.</returns>
    private string ToCamelCase(string s)
    {
        // creates an empty string for returning result
        string result = "";
        // parses the input string into seperate strings
        string[] parsed = s.Split();
        // loops throught the words found
        foreach (string element in parsed)
        {
            // checks if first character is uppercase
            if (Char.IsUpper(element[0]))
            {
                // changes it to lowercase and adds that character to result
                result += Char.ToLower(element[0]);
            }
            // checks if first character is lowercase
            else if (Char.IsLower(element[0]))
            {
                // changes it to lowercase and adds that character to result
                result += Char.ToUpper(element[0]);
            }
            // adds the rest of that word to the result
            result += element[1..];
        }
        // returns the string result
        return result;
    }

    /// <summary>
    /// Determines whether a password meets requirements.
    /// </summary>
    /// <param name="pwd">The password to validate.</param>
    /// <returns>
    /// True if the password is strong; otherwise, false.
    /// </returns>
    private bool IsPasswordStrong(string pwd)
    {
        // creates checking variable for each requirement
        bool has_lower = false;
        bool has_upper = false;
        bool has_dig = false;
        bool has_spec = false;

        // first checking password length
        if (pwd.Length < 8)
        {
            return false;
        }
        // loops through each character in the password
        foreach (char character in pwd)
        {
            // if a lowercase character is found, its checking variable is set to true
            if (Char.IsLower(character))
            {
                has_lower = true;
            }
            // if an uppercase character is found its checking variable is set to true
            else if (Char.IsUpper(character))
            {
                has_upper = true;
            }
            // if a digit character is found its checking variable is set to true
            else if (Char.IsDigit(character))
            {
                has_dig = true;
            }
            // if it is not digit or letter, it has to be a special character
            // special variable checking value is set to true
            else
            {
                has_spec = true;
            }
        }
        // checks that all checking variables are true and returns true
        if (has_lower && has_upper && has_dig && has_spec)
        {
            return true;
        }
        // returns false if all checking variable are not true
        return false;
    }

    /// <summary>
    /// Returns a list containing only unique items from the input list.
    /// </summary>
    /// <param name="list">The input list.</param>
    /// <returns>A list of unique items.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the input list is null.
    /// </exception>
    private List<T> GetUniqueItems<T>(List<T> list)
    {
        // throws an exception if the input list is null
        if (list == null)
        {
            throw new ArgumentException("This list is null");
        }
        // creates a list of results to be returned
        List<T> result = new List<T>();
        // loops through items in the lsit
        foreach (T item in list)
        {
            // if the item, is not already in results list add it
            if (!result.Contains(item))
            {
                result.Add(item);
            }
        }
        // return the list of results
        return result;

    }

     /// <summary>
    /// Calculates the average of an array of integers.
    /// </summary>
    /// <param name="numbers">The array of integers.</param>
    /// <returns>The average value.</returns>
    private double CalculateAverage(int[] numbers)
    {
        // create the variable to count total
        double total = 0;
        // loops through the array
        foreach (int num in numbers)
        {
            // add each int in array to the total
            total += num;
        }
        // return the calculated average
        return total / numbers.Length;
    }

    /// <summary>
    /// Identifies items in an array that are duplicates.
    /// </summary>
    /// <param name="array">The input array.</param>
    /// <returns>An array of items that meet the occurrence condition.</returns>
    private T[] Duplicates<T>(T[] array)
    {
        // creates a dictionary to track occurences
        Dictionary<T, int> catalog = new Dictionary<T, int>();
        // loops throught the array mapping elements to their occurrences
        foreach (T item in array)
        {
            if (catalog.ContainsKey(item))
            {
                catalog[item]++;
            }
            else
            {
                catalog[item] = 1;
            }
        }
        // creates a list to add element to
        List<T> result_list = new List<T>();
        // loops through each element in the dictionary
        foreach (KeyValuePair<T, int> item in catalog)
        {
            // adds item to the list if it has more than one occurence
            if (item.Value <= 2)
            {
                result_list.Add(item.Key);
            }
        }
        // return an array of the results
        return result_list.ToArray();

    }
}
