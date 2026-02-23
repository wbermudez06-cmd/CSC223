
// TokenizerImpl.cs
// ===========================================================================
// This file contains the main tokenizer implementation that converts source
// code strings into sequences of tokens. The tokenizer performs lexical
// analysis, recognizing variables, numbers, operators, and delimiters.
// ===========================================================================

using System;
using System.Collections.Generic;

namespace Tokenizer
{
    /// <summary>
    /// Implements a lexical analyzer (tokenizer) that converts source code
    /// into a sequence of tokens. The tokenizer recognizes variables, keywords,
    /// numbers (integers and floats), operators, and delimiters.
    /// </summary>
    public class TokenizerImpl
    {
        /// <summary>
        /// Tokenizes the given source code string into a list of tokens.
        /// The method iterates through each character, identifying token boundaries
        /// and classifying each token appropriately.
        /// </summary>
        /// <param name="sourcecode">The source code string to tokenize</param>
        /// <returns>A list of Token objects representing the lexical units</returns>
        /// <exception cref="ArgumentException">Thrown when an unexpected character is encountered</exception>
        public List<Token> Tokenize(string sourcecode)
        {

            List<Token> tokens = new List<Token>();
            int i = 0;

            // Main tokenization loop - process each character
            while (i < sourcecode.Length)
            {

                char c = sourcecode[i];

                // Check for variable names or keywords (start with letter or underscore)
                if (char.IsLetter(c) || c == '_')
                {
                    //Variable or keyword
                    Token t = HandleVariable(sourcecode, ref i);
                    tokens.Add(t);
                }

                // Check for numeric literals (integers or floats)
                else if (char.IsDigit(c))
                {
                    //Integer or float
                    Token t = HandleNumber(sourcecode, ref i);
                    tokens.Add(t);
                }

                // Check for assignment operator (:=)
                else if (c == ':')
                {
                    //Assignment
                    Token t = HandleAssignment(sourcecode, ref i);
                    tokens.Add(t);
                }

                // Check for multiplication (*) or exponentiation (**)
                else if (c == '*')
                {
                    //Multiplication or exponentiation
                    Token t = HandleMultiplication(sourcecode, ref i);
                    tokens.Add(t);
                }

                // Check for float division (/) or integer division (//)
                else if (c == '/')
                {
                    //Float/integer division
                    Token t = HandleDivision(sourcecode, ref i);
                    tokens.Add(t);
                }

                // Single-character addition operator
                else if (c == '+')
                {
                    //Addition
                    tokens.Add(new Token(TokenConstants.PLUS, TokenType.OPERATOR));
                    i++;
                }

                // Single-character subtraction operator
                else if (c == '-')
                {
                    //Subtraction
                    tokens.Add(new Token(TokenConstants.MINUS, TokenType.OPERATOR));
                    i++;
                }

                // Single-character modulus operator
                else if (c == '%')
                {
                    //Modulus
                    tokens.Add(new Token(TokenConstants.MODULUS, TokenType.OPERATOR));
                    i++;
                }

                // Left parenthesis delimiter
                else if (c == '(')
                {
                    //Left parenthesis
                    tokens.Add(new Token(TokenConstants.LEFT_PAREN, TokenType.LEFT_PAREN));
                    i++;
                }

                // Right parenthesis delimiter
                else if (c == ')')
                {
                    //Right parenthesis
                    tokens.Add(new Token(TokenConstants.RIGHT_PAREN, TokenType.RIGHT_PAREN));
                    i++;
                }

                // Left curly brace delimiter
                else if (c == '{')
                {
                    //Left curly brace
                    tokens.Add(new Token(TokenConstants.LEFT_CURLY, TokenType.LEFT_CURLY));
                    i++;
                }

                // Right curly brace delimiter
                else if (c == '}')
                {
                    //Right curly brace
                    tokens.Add(new Token(TokenConstants.RIGHT_CURLY, TokenType.RIGHT_CURLY));
                    i++;
                }

                // Skip whitespace characters (spaces, tabs, newlines)
                else if (char.IsWhiteSpace(c))
                {
                    i++;
                }

                // Unrecognized character - throw an error
                else
                {
                    //If all else fails, throw error
                    throw new ArgumentException("Unexpected character!");
                }
            }

            return tokens;
        }

        /// <summary>
        /// Processes a variable name or keyword starting at the current position.
        /// Variables can contain letters, digits, and underscores but must start
        /// with a letter or underscore. Recognizes "return" as a keyword.
        /// </summary>
        /// <param name="source">The source code string</param>
        /// <param name="i">Reference to current position index (will be updated)</param>
        /// <returns>A Token representing the variable or keyword</returns>
        private Token HandleVariable(string source, ref int i)
        {
            int start = i;
            // Consume all letters, digits, and underscores
            while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_'))
                i++;

            string word = source.Substring(start, i - start);

            // Check if this is the "return" keyword
            if (word == "return")
                return new Token(word, TokenType.RETURN);

            // Otherwise, it's a variable name
            return new Token(word, TokenType.VARIABLE);
        }

        /// <summary>
        /// Processes a numeric literal (integer or float) starting at the current position.
        /// Handles decimal points to distinguish between integers and floats.
        /// </summary>
        /// <param name="source">The source code string</param>
        /// <param name="i">Reference to current position index (will be updated)</param>
        /// <returns>A Token representing the number (INTEGER or FLOAT type)</returns>
        /// <exception cref="ArgumentException">Thrown if multiple decimal points are found</exception>
        private Token HandleNumber(string source, ref int i)
        {
            int start = i;
            bool isFloat = false;

            // Consume all digits and decimal points
            while (i < source.Length && (char.IsDigit(source[i]) || source[i] == '.'))
            {
                
                if (source[i] == '.')
                {
                    // Error if we encounter a second decimal point
                    if (isFloat)
                        throw new ArgumentException("Unexpected second decimal point found in umber!");
                    isFloat = true;
                }
                i++;
            }

            string numberToReturn = source.Substring(start, i - start);

            // Determine token type based on presence of decimal point
            //If/else syntax (If isFloat then TokenType.FLOAT else TokenType.INTEGER)
            TokenType type = isFloat ? TokenType.FLOAT : TokenType.INTEGER;

            return new Token(numberToReturn, type);
        }

        /// <summary>
        /// Processes an assignment operator (:=) starting at the current position.
        /// The assignment operator must be exactly ":=" - a lone colon is invalid.
        /// </summary>
        /// <param name="source">The source code string</param>
        /// <param name="i">Reference to current position index (will be updated)</param>
        /// <returns>A Token representing the assignment operator</returns>
        /// <exception cref="ArgumentException">Thrown if ':' is not followed by '='</exception>
        private Token HandleAssignment(string source, ref int i)
        {
            // Verify that ':' is followed by '=' to form ':='
            if (i + 1 >= source.Length || source[i + 1] != '=')
                throw new ArgumentException("Not an assignment operator (Expected := but got :");

            i += 2; //Takes ":="
            return new Token(TokenConstants.ASSIGNMENT, TokenType.ASSIGNMENT);
        }

        /// <summary>
        /// Processes multiplication (*) or exponentiation (**) operator.
        /// Checks if a single asterisk is followed by another to form exponentiation.
        /// </summary>
        /// <param name="source">The source code string</param>
        /// <param name="i">Reference to current position index (will be updated)</param>
        /// <returns>A Token representing multiplication or exponentiation</returns>
        private Token HandleMultiplication(string source, ref int i)
        {
            // Check for exponentiation operator (**)
            if (i + 1 < source.Length && source[i + 1] == '*')
            {
                i += 2; //Takes "**"
                return new Token(TokenConstants.EXPONENTIATION, TokenType.OPERATOR);
            }

            // Single asterisk is multiplication
            i++; //Takes "*"
            return new Token(TokenConstants.TIMES, TokenType.OPERATOR);
        }

        /// <summary>
        /// Processes float division (/) or integer division (//) operator.
        /// Checks if a single slash is followed by another to form integer division.
        /// </summary>
        /// <param name="source">The source code string</param>
        /// <param name="i">Reference to current position index (will be updated)</param>
        /// <returns>A Token representing float or integer division</returns>
        private Token HandleDivision(string source, ref int i)
        {
            // Check for integer division operator (//)
            if (i + 1 < source.Length && source[i + 1] == '/')
            {
                i += 2; //Takes "//"
                return new Token(TokenConstants.INTEGER_DIVISION, TokenType.OPERATOR);
            }

            // Single slash is float division
            i++; //Takes "/"
            return new Token(TokenConstants.FLOAT_DIVISION, TokenType.OPERATOR);
        }
    }
}