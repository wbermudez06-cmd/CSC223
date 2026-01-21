using Microsoft.VisualBasic;

namespace Xunit.Sdk;

public class generalUtils
{

    /// <summary>
    /// Checks if an array contains a specific item using
    /// equality comparison: EqualityComparer<T>.
    /// </summary>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool Contains<T>(T[] array, T item)
    {
        int index = 0;
        while (index < array.Length)
        {
            // Returns true if the item in the array is equal to the specified item
            if (EqualityComparer<T>.Default.Equals(array[index], item))
                return true;
            index += 1;
        }
        return false;
    }

    /// <summary>
    /// Returns a string with spaces for the specified
    /// indentation level: 4 spaces per level.
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public string GetIndentation(int level)
    {
        // Second parameter is number of times to repeat
        return new string(' ', level * 4);
    }

    /// <summary>
    /// Checks if the given string contains only lowercase letters.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsValidVariable(string name)
    {
        foreach (char c in name)
        {
            // Return false if any character is NOT lowercase
            if (!char.IsLower(c))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Checks if a string is a valid operator (+, -, *, /, //, %, **).
    /// </summary>
    /// <param name="op"></param>
    /// <returns></returns>
    public bool IsValidOperator(string op)
    {
        string[] validOperatorList = { "+", "-", "*", "/", "//", "%", "**" };
        // Run through operators in validOperatorList and return true if
        // one patches the given operator
        foreach (string validOperator in validOperatorList)
        {
            if (op == validOperator)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Counts how many times a given character appears in a string.
    /// </summary>
    /// <param name="s"></param>
    /// <param name="c"></param>
    public int CountOccurrances(string s, char c)
    {
        int count = 0;
        foreach (char currentChar in s)
        {
            if (currentChar == c)
                count += 1;
        }
        return count;
    }

    /// <summary>
    /// Converts space-separated words to camelCase
    /// format. That is, the first character is lower case,
    /// but all subsequent spaces are removed and the next
    /// letter is capitalized.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public string ToCamelCase(string s)
    {
        // Empty string case
        if (s.Length == 0)
            return "";
        
        string[] wordList = s.Split(' ');
        bool isFirstWord = true;
        string camelCaseString = "";

        foreach (string word in wordList)
        {
            camelCaseString += word.ToLower();
            if (isFirstWord)
            {
                // Ignore capitalization for first word
                isFirstWord = false;
            }
            else
            {
                camelCaseString += char.ToUpper(word[0]);
                // Substring takes all of the string starting from the indicated index
                camelCaseString += word.Substring(1).ToLower();
            }
        }
        return camelCaseString;
    }

    /// <summary>
    /// Checks if a password meets strength requirements.
    /// A strong password must be at least 8 characters
    /// long and contain at least one lowercase letter, one
    /// uppercase letter, one digit, and one special
    /// character (any non-alphanumeric character).
    /// </summary>
    /// <param name="pwd"></param>
    /// <returns></returns>
            public bool IsPasswordStrong(string pwd)
    {
        if (pwd.Length < 8)
            return false;

        bool hasLower = false;
        bool hasUpper = false;
        bool hasDigit = false;
        bool hasSpecial = false;

        foreach (char c in pwd)
        {
            if (char.IsLower(c))
                hasLower = true;
            else if (char.IsUpper(c))
                hasUpper = true;
            else if (char.IsDigit(c))
                hasDigit = true;
            else
                hasSpecial = true;
        }
        return hasLower && hasUpper && hasDigit && hasSpecial;
    }

    /// <summary>
    /// Returns another list with only unique items from
    /// the given list; the given list should not be
    /// modified. Throws ArgumentException if list is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public List<T> GetUniqueItems<T>(List<T> list)
    {
        if (list == null)
            throw new ArgumentException("List is null!");

        List<T> uniqueItems = new List<T>();

        foreach (T item in list)
        {
            // If newly created list does not contain the item, add it
            if (!uniqueItems.Contains(item))
                uniqueItems.Add(item);
        }
        return uniqueItems;
    }

    /// <summary>
    /// Calculates the average value of an array of
    /// integers. Throws ArgumentException if array
    /// is null or empty.
    /// </summary>
    /// <param name="numbers"></param>
    /// <returns></returns>
    public double CalculateAverage(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
            throw new ArgumentException("Array is null or empty!");

        int sum = 0;

        // Add all numbers then divide by length (average)
        foreach (int num in numbers)
        {
            sum += num;
        }
        return (double)sum / numbers.Length;
    }

    /// <summary>
    /// Returns the set of all items from an array that are
    /// duplicated more than one time. It is reasonable to
    /// use a dictionary in an implementation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <returns></returns>
    public T[] Duplicates<T>(T[] array)
    {
        Dictionary<T, int> itemCounts = new Dictionary<T, int>();

        foreach (T item in array)
        {
            // If dictionary contains key, increase count. If not, add it with
            // a count of 1
            if (itemCounts.ContainsKey(item))
                itemCounts[item] += 1;
            else
                itemCounts[item] = 1;
        }

        List<T> duplicates = new List<T>();

        // For each item in the dictionary, if its count is greater than 1,
        // add it to the duplicates list to return
        foreach (var pair in itemCounts)
        {
            if (pair.Value > 1)
                duplicates.Add(pair.Key);
        }
        return duplicates.ToArray();
    }

}