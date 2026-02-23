
// Token.cs
// ===========================================================================
// This file defines the core token structures used by the tokenizer.
// It includes the TokenType enumeration, TokenConstants for operators and
// symbols, and the Token class that represents individual lexical units.
// ===========================================================================

using System;

namespace Tokenizer
{
    /// <summary>
    /// Enumeration defining all possible token types recognized by the tokenizer.
    /// Each type represents a distinct category of lexical element.
    /// </summary>
    public enum TokenType
    {
        VARIABLE,     // 0 - Identifiers and variable names
        RETURN,       // 1 - The 'return' keyword
        INTEGER,      // 2 - Whole number literals (e.g., 42, 123)
        FLOAT,        // 3 - Floating-point number literals (e.g., 3.14, 0.5)
        OPERATOR,     // 4 - Arithmetic operators (+, -, *, /, //, %, **)
        ASSIGNMENT,   // 5 - Assignment operator (:=)
        LEFT_PAREN,   // 6 - Opening parenthesis (
        RIGHT_PAREN,  // 7 - Closing parenthesis )
        LEFT_CURLY,   // 8 - Opening curly brace {
        RIGHT_CURLY,  // 9 - Closing curly brace }
    }

    /// <summary>
    /// Static class containing string constants for all recognized operators
    /// and symbols in the language. These constants are used for token matching
    /// and comparison throughout the tokenization process.
    /// </summary>
    static class TokenConstants
    {
        // Arithmetic operators
        public static string PLUS = "+";               // Addition operator
        public static string MINUS = "-";              // Subtraction operator
        public static string TIMES = "*";              // Multiplication operator
        public static string FLOAT_DIVISION = "/";     // Floating-point division operator
        public static string INTEGER_DIVISION = "//";  // Integer division operator
        public static string MODULUS = "%";            // Modulus/remainder operator
        public static string EXPONENTIATION = "**";    // Exponentiation/power operator
        
        // Delimiters
        public static string LEFT_PAREN = "(";         // Opening parenthesis
        public static string RIGHT_PAREN = ")";        // Closing parenthesis
        public static string LEFT_CURLY = "{";         // Opening curly brace
        public static string RIGHT_CURLY = "}";        // Closing curly brace
        
        // Other symbols
        public static string ASSIGNMENT = ":=";        // Assignment operator
        public static string DECIMAL_POINT = ".";      // Decimal point for floats
    }

    /// <summary>
    /// Represents a single lexical token produced by the tokenizer.
    /// Each token contains the actual text (token) and its type classification.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The actual string value of the token (e.g., "42", "variable", "+")
        /// </summary>
        public string token;
        
        /// <summary>
        /// The classification of this token (e.g., INTEGER, VARIABLE, OPERATOR)
        /// </summary>
        public TokenType type;

        /// <summary>
        /// Constructs a new Token with the specified value and type.
        /// </summary>
        /// <param name="token">The string value of the token</param>
        /// <param name="type">The TokenType classification</param>
        public Token(string token, TokenType type)
        {
            this.token = token;
            this.type = type;
        }

        /// <summary>
        /// Returns a string representation of this token for debugging purposes.
        /// </summary>
        /// <returns>A formatted string showing the token value and type</returns>
        public override string ToString()
        {
            return $"Token: {token}, Type: {type}";
        }

        /// <summary>
        /// Determines whether this token is equal to another object.
        /// Two tokens are equal if they have the same token string and type.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True if the tokens are equal, false otherwise</returns>
        public override bool Equals(object? obj)
        {
            if (obj is Token toEqual)
                return (token == toEqual.token && type == toEqual.type);

            //Extra false in case obj is not a token
            return false;
        }
    }
}