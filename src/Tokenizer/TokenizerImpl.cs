
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;

namespace Tokenizer
{
    public class TokenizerImpl
    {
        public List<Token> Tokenize(string source)
        {

            List<Token> tokenList = new List<Token>();

            // scan character by character, uses an indexing loop
            int idx = 0;
            while (idx < source.Length)
            {
                // sets checked character
                char character = source[idx];

                // ignore whitespace, skips past it as a character
                if (char.IsWhiteSpace(character))
                {
                    idx++;
                }

                // instance if the character indicates a keyword/variable
                else if (char.IsLetter(character))
                {
                    // sets next index and token to add to the list
                    var (currToken, nextIdx) = HandleKeyword(idx, source);
                    // adds the token to list and goes to next index
                    tokenList.Add(currToken);
                    idx = nextIdx;
                }

                // checks if the next character is a number (int/float)
                else if (char.IsDigit(character))
                {
                    // creates token and gets next valid index
                    var (currToken, nextIdx) = HandleNumber(idx, source);
                    // adds to the list and updates index
                    tokenList.Add(currToken);
                    idx = nextIdx;
                }

                // checks for indication of an assignment character
                else if (character == ':')
                {
                    // creates the token and adds it to the list
                    Token currToken = HandleAssignment(idx, source);
                    tokenList.Add(currToken);
                    // updates the index by 2 (unique to assignment character)
                    idx += 2;
                }

                // checks if indication of integer or float division
                else if (character == '/')
                {
                    // creates the token and sets the next index
                    var (currToken, nextIdx) = HandleDivision(idx, source);
                    // adds it to the list and updates index
                    tokenList.Add(currToken);
                    idx = nextIdx;
                }

                // checks for any single character operators
                else if (character == '+' || character == '-' || character == '%' || character == '^')
                {
                    // adds the token with single character and opertor type to the list
                    tokenList.Add(new Token(character.ToString(), TokenType.OPERATOR));
                    // updates the inddex by 1
                    idx++;

                }

                else if (character == '*')
                {
                    if (idx + 1 < source.Length && source[idx + 1] == '*')
                    {
                        tokenList.Add(new Token("**", TokenType.OPERATOR));
                        idx += 2;
                    }
                    else
                    {
                        tokenList.Add(new Token("*", TokenType.OPERATOR));
                        idx++;
                    }
                }

                // checks for unique characters 
                else if (character == '(')
                {
                    // adds the token to list with corresponding type
                    tokenList.Add(new Token(character.ToString(), TokenType.LEFT_PAREN));
                    // updates index by 1
                    idx++;
                }

                else if (character == ')')
                {
                    // creates and adds token to the list
                    tokenList.Add(new Token(character.ToString(), TokenType.RIGHT_PAREN));
                    // updates index by 1
                    idx++;
                }

                else if (character == '{')
                {
                    // creates and adds token to the list
                    tokenList.Add(new Token(character.ToString(), TokenType.LEFT_CURLY));
                    // updates index by 1
                    idx++;
                }

                else if (character == '}')
                {
                    // creates and adds token to the list
                    tokenList.Add(new Token(character.ToString(), TokenType.RIGHT_CURLY));
                    // updates index by 1
                    idx++;
                }

                else
                {
                    //tokenList.Add(new Token(character.ToString(), TokenType.UNKNOWN));
                    throw new ArgumentException("Invalid character.");
                }

                // anything else is seen as an invalid character and throws an exception
                // throw new ArgumentException($"Unexpected character {character} at {idx}.");
            }

            // returns compiled token list
            return tokenList;
        }

        // handles "return" keyword or a variable 
        private (Token, int) HandleKeyword(int idx, string source)
        {
            // creates variable of compiled keyword
            string check = "";
            // loops through the characters until end or keyword or vairable is indicated
            while (idx < source.Length && char.IsLetter(source[idx]))
            {
                // adds the characters to variable, updates index by 1
                check += source[idx];
                idx++;
            }

            // if it is the "return" keyword, return that as the created token, along with updated index
            if (check == "return") return (new Token(check, TokenType.RETURN), idx);

            // variable must be lowercase alphabetic
            foreach (char character in check)
            {
                // throws exception if doesn't meet the requirements
                if (!char.IsLetter(character) || !char.IsLower(character)) throw new ArgumentException("Invalid variable");
            }
            // returns the valid variable as a created token
            return (new Token(check, TokenType.VARIABLE), idx);
        }

        // handles numbers that are either integers or floast
        private (Token, int) HandleNumber(int idx, string source)
        {
            // compiles a string of the numbers in sequence 
            string num = "";

            // loops through the digits
            while (idx < source.Length && char.IsDigit(source[idx]))
            {
                // adds them to the varible and updates index
                num += source[idx];
                idx++;
            }

            // checks of there is a decimal point indicating a float
            if (idx < source.Length && source[idx] == '.')
            {
                // adds the decimal into the number and updates index
                num += '.';
                idx++;

                // checks that integer has at least one digit after decimal
                // throws and exception if its invalid
                if (idx >= source.Length || !char.IsDigit(source[idx])) throw new ArgumentException("Invalid float");

                // loops through what is after the decimal 
                while (idx < source.Length && char.IsDigit(source[idx]))
                {
                    // adds to the string and updates the index
                    num += source[idx];
                    idx++;
                }

                // returns finalized created floast token and updated index
                return (new Token(num, TokenType.FLOAT), idx);
            }
            // returns integer token and index
            return (new Token(num, TokenType.INTEGER), idx);
        }

        // handles assignment variable only
        private Token HandleAssignment(int idx, string source)
        {
            // checks if colon is at the end of the string already
            if (idx + 1 >= source.Length) throw new ArgumentException("not an assignment operator");
            // checks that after the colon is "="
            if (source[idx + 1] == '=')
            {
                // updates index to go past the full assignment strings
                idx++;
                // returns the found assignment token
                return new Token(":=", TokenType.ASSIGNMENT);
            }
            // if there is no "=" after the colon, throws and exception
            throw new ArgumentException("Invalid assignment operator");
        }

        // handles integer and float division
        private (Token, int) HandleDivision(int idx, string source)
        {
            // checks if indicates integer division
            if (idx + 1 < source.Length && source[idx + 1] == '/')
            {
                // update track index to include second backslash
                // returns created token and updated index
                return (new Token("//", TokenType.OPERATOR), idx + 2);
            }

            // returns token of integer division
            return (new Token("/", TokenType.OPERATOR), idx + 1);
        }
    }
    
}