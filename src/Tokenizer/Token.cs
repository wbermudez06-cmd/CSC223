using System;
using System.Reflection.Metadata;

namespace Tokenizer
{
    // enumeration table for token types
    public enum TokenType
    {
        VARIABLE, RETURN,
        INTEGER, FLOAT,
        OPERATOR, ASSIGNMENT,
        LEFT_PAREN, RIGHT_PAREN, LEFT_CURLY, RIGHT_CURLY,
        UNKNOWN
    }
    // implement TokenType class

    public static class TokenConstants
    {
        // selection of token constants
        public static string PLUS = "+";

        public static string LEFT_PAREN = "(";

        public static string RIGHT_PAREN = ")";

        public static string LEFT_CURLY = "{";

        public static string RIGHT_CURLEY = "}";

        public static string ASSIGNMENT = ":=";

        public static string DECIMAL_POINT = ".";
    }

    // creation of a token 
    public class Token
    {
        // two properties of the token
        private string Value;
        private TokenType Type;
        // MIGHT BE BAD TO MAKE THESE PUBLIC!!!

        public Token(string val, TokenType type)
        {
            Value = val;
            Type = type;
        }

        // Standard methods
        // meaningful string
        public string ToString()
        {
            return $"{Type},{Value}";
        }

        // Equality comparison method
        public bool Equals(Token? otherToken)
        {
            if (otherToken is null) return false;
            return Value == otherToken.Value && Type == otherToken.Type;
        }

        public string GetValue() { return Value; }
        public TokenType GetTokenType() { return Type; }
    }
}
