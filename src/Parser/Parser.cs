/**
 This file defines the core parsing helpers used to interpret a simple block-based language and
 convert its contents into AST structures that can be consumed later by semantic analysis,
 interpretation, or execution.

 Major capabilities of this file:
 - Parse an entire program into a BlockStmt
 - Parse expressions recursively
 - Parse assignment and return statements
 - Convert tokens into AST nodes
 - Create binary operator AST nodes from parsed operator tokens
 - Track variables in a symbol table during parsing
 **/
using AST;
using Tokenizer;

namespace Parser
{
    /// <summary>
    /// Provides static helper methods for parsing source text and token streams
    /// into AST nodes for a simple language.
    /// </summary>
    public static class Parser
    {
        /// <summary>
        /// Parses an entire program string into a block statement AST node.
        /// </summary>
        /// <param name="program">
        /// </param>
        /// <returns>
        /// <see cref="AST.BlockStmt"/> 
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown when the input does not begin and end with enclosed curly braces.
        /// </exception>
        public static AST.BlockStmt Parse(string program)
        {
            // create a symbol table to track variable declarations during parsing
            SymbolTable<string, object> symbolTable = new SymbolTable<string, object>();
            // split the program into individual lines for block-level parsing
            List<string> lines = new List<string>();
            foreach (string rawline in program.Split('\n'))
            {
                // clean up lines that may have extra whitespace
                string line = rawline.Trim();
                if (line.Length > 0)
                {
                    // add lines to string 
                    lines.Add(line);
                }
            }
            // ensure correct syntax of the blocks
            if (lines[0] != "{" || lines.Count == 0) throw new ParseException("Program must start with '{'.");
            // returns the parsed block statements
            return ParseBlockStmt(lines, symbolTable);

        }
        /// <summary>
        /// Parses a parenthesized expression from a token list.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns>An <see cref="AST.ExpressionNode"/> </returns>
        /// <exception cref="ParseException"></exception>
        private static AST.ExpressionNode ParseExpression(List<Token> tokens)
        {
            // throws an exception if expression is empty
            if (tokens == null || tokens.Count == 0) throw new ParseException("Expression is empty.");
            // throws exception if invalid syntax
            if (tokens[0].GetTokenType() != TokenType.LEFT_PAREN) throw new ParseException("Expression must begin with a (.");
            // throws exception if it doesn't end in right parentheses
            if (tokens[tokens.Count - 1].GetTokenType() != TokenType.RIGHT_PAREN) throw new ParseException("Expression must end with a ).");

            // parse expression without beginning and ending parenthesis
            return ParseExpressionContent(tokens.GetRange(1, tokens.Count - 2));
        }

        private static AST.ExpressionNode ParseExpressionContent(List<Tokenizer.Token> tokens)
        {
            // throw exception if the token list is null or empty
            if (tokens == null || tokens.Count == 0) throw new ParseException("The given tokens are invalid.");

            // handles if there is only one token (singular variable node)
            if (tokens.Count == 1) return HandleSingleToken(tokens[0]);

            // keeps track of inde
            int opIdx = -1;
            // for handling nested expressions, keeps track of depth of expression statements
            int depth = 0;

            // go through tokens based on indezing
            for (int i = 0; i < tokens.Count; i++)
            {
                // checks if the loop runs into a nested expression; adjusts depth as needed
                if (tokens[i].GetTokenType() == TokenType.LEFT_PAREN) depth++;
                // checks if the the loop reaches the end of an expression
                if (tokens[i].GetTokenType() == TokenType.RIGHT_PAREN) depth--;

                // if no expression beginning/ends indicated
                if (depth == 0)
                {
                    // throws and exception if an unknown token type is found
                    if (tokens[i].GetTokenType() == TokenType.UNKNOWN) throw new ParseException("Invalid operator.");
                    
                    // if an operator is found
                    if (tokens[i].GetTokenType() == TokenType.OPERATOR)
                    {
                        // More than one top-level operator means missing sub-expression parens
                        if (opIdx != -1) throw new ParseException("No valid expression found. Missing parentheses around sub-expression.");
                        opIdx = i;
                    }
                }
            }

            // instnce that no top level operator is found
            if (opIdx == -1)
            {

                // if run into a nested operator
                if (tokens[0].GetTokenType() == TokenType.LEFT_PAREN)
                {
                    // Find its matching close
                    int leftIdx = 0;
                    int rightIdx = -1;
                    for (int i = 0; i < tokens.Count; i++)
                    {
                        if (tokens[i].GetTokenType() == TokenType.LEFT_PAREN) leftIdx++;
                        if (tokens[i].GetTokenType() == TokenType.RIGHT_PAREN) leftIdx--;
                        if (leftIdx == 0)
                        {
                            rightIdx = i;
                            break;
                        }
                    }
                    // if index is not reset, meaning no parentheses found
                    if (rightIdx == -1) throw new ParseException("Missing ).");
                    // throws exception if found parentheses is not the last token
                    if (rightIdx != tokens.Count - 1) throw new ParseException("No valid expression found.");
                    // return the parsed expression 
                    return ParseExpression(tokens.GetRange(0, tokens.Count));
                }
                throw new ParseException("No valid expression found.");
            }

            // builds the left operand node for AST
            AST.ExpressionNode left;
            // creates a list of everything to the left of the operator
            List<Token> leftTokens = tokens.GetRange(0, opIdx);

            // throws an exception if there are no left operands for the AST
            if (leftTokens.Count == 0) throw new ParseException("Operator is missing a left operand.");

            // handles left as a single token if only one token found
            if (leftTokens.Count == 1)
            {
                left = HandleSingleToken(leftTokens[0]);
            }

            else
            {
                // must be a parenthesized expression if not a single node
                // throws exception if that is not the case, with correct syntax
                if (leftTokens[0].GetTokenType() != TokenType.LEFT_PAREN) throw new ParseException("Missing (.");
                if (leftTokens[leftTokens.Count - 1].GetTokenType() != TokenType.RIGHT_PAREN) throw new ParseException("Missing ).");

                // parses and sets it to the right expression
                left = ParseExpression(leftTokens);
            }

            // builds the left operand node for AST
            AST.ExpressionNode right;
            // creates a list of tokens to the right of the operator
            List<Token> rightTokens = tokens.GetRange(opIdx + 1, tokens.Count - opIdx - 1);

            // throws and exception if there is nothing to the right of the operator
            if (rightTokens.Count == 0) throw new ParseException("Operator is missing a right operand.");

            // handles it as a single token if only one is found
            if (rightTokens.Count == 1)
            {
                right = HandleSingleToken(rightTokens[0]);
            }

            else
            {
                // must be a parenthesized expression if not a single node
                // throws exception if that is not the case, with correct syntax
                if (rightTokens[0].GetTokenType() != TokenType.LEFT_PAREN) throw new ParseException("Missing (.");
                if (rightTokens[rightTokens.Count - 1].GetTokenType() != TokenType.RIGHT_PAREN) throw new ParseException("Missing ).");

                // parses the exception and sets it to the right expression
                right = ParseExpression(rightTokens);
            }

            // returns the created binary operator node with found operations, created right node, and created left node
            return CreateBinaryOperatorNode(tokens[opIdx].GetValue(), left, right);
        }

        /// <summary>
        /// Converts a single token into the corresponding AST expression node.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>An <see cref="AST.ExpressionNode"/> </returns>
        /// <exception cref="ParseException"> </exception>
        private static AST.ExpressionNode HandleSingleToken(Tokenizer.Token token)
        {
            if (token.GetTokenType() == TokenType.UNKNOWN) throw new ParseException("This is an unknown token.");

            if (token.GetTokenType() == TokenType.INTEGER) return new LiteralNode(token.GetValue());

            if (token.GetTokenType() == TokenType.FLOAT) return new LiteralNode(token.GetValue());

            if (token.GetTokenType() == TokenType.VARIABLE) return new VariableNode(token.GetValue());

            throw new ParseException($"No corresponding token type of {token.GetValue()}");
        }

        /// <summary>
        /// Creates a binary operator expression node from an operator symbol
        /// and two operand expression nodes.
        /// </summary>
        /// <returns>An AST node representing the binary operation.</returns>
        /// <exception cref="ParseException"></exception>
        private static AST.ExpressionNode CreateBinaryOperatorNode(string op, ExpressionNode l, ExpressionNode r)
        {
            // maps a recognized operator string to its AST node type
            if (op == "+") return new PlusNode(l, r);
            if (op == "-") return new MinusNode(l, r);
            if (op == "*") return new TimesNode(l, r);
            if (op == "/") return new FloatDivNode(l, r);
            if (op == "//") return new IntDivNode(l, r);
            if (op == "%") return new ModulusNode(l, r);
            if (op == "**") return new ExponentiationNode(l, r);
            else { throw new ParseException($"Invalid operator: {op}"); }

        }

        /// <summary>
        /// Parses a variable name into a variable AST node.
        /// </summary>
        /// <param name="var">The variable name.</param>
        /// <returns><see cref="AST.VariableNode"/> </returns>
        /// <exception cref="ParseException"></exception>
        private static AST.VariableNode ParseVariableNode(string var)
        {
            // throws an exception if variable is invalid, otherwise it returns a new var node
            if (var == null) throw new ParseException("Invalid variable given.");
            return new VariableNode(var);
        }

        /// <summary>
        /// Parses an assignment statement from a token list and records the variable
        /// in the symbol table.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="symbolTable"></param>
        /// <returns><see cref="AST.AssignmentStmt"/></returns>
        /// <exception cref="ParseException"></exception>
        private static AST.AssignmentStmt ParseAssignmentStmt(List<Tokenizer.Token> tokens, SymbolTable<string, object> symbolTable)
        {
            // variable name is invalid, throws exception
            if (tokens[0].GetTokenType() != TokenType.VARIABLE) throw new ParseException("Invalid variable name.");
            // throws an exception if the assignment character itself is invalid
            if (tokens.Count < 3 || tokens[1].GetTokenType() != TokenType.ASSIGNMENT) throw new ParseException("Invalid token: Expected assignment operator ':=' after variable.");
            // throws exception if there is nothing after the assignment character
            if (tokens.Count <= 2) throw new ParseException("Assignment statement is missing an expression after ':='.");

            // save the variable in the symbol table with an initial null value, add it to symbolTable
            KeyValuePair<string, object> item = new KeyValuePair<string, object>(tokens[0].GetValue(), null);
            symbolTable.Add(item);

            // parse the right-hand-side expression
            List<Token> exprTokens = tokens.GetRange(2, tokens.Count - 2);
            AST.ExpressionNode expr = ParseExpression(exprTokens);

            // build the assignment statement node
            AST.VariableNode varNode = ParseVariableNode(tokens[0].GetValue());
            return new AssignmentStmt(varNode, expr);

        }

        /// <summary>
        /// Parses a return statement from a token list.
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns><see cref="AST.ReturnStmt"/></returns>
        /// <exception cref="ParseException"></exception>
        private static AST.ReturnStmt ParseReturnStatement(List<Tokenizer.Token> tokens)
        {
            // throws and exception if it doesn't begin with 'return'
            if (tokens[0].GetTokenType() != TokenType.RETURN) throw new ParseException("Return statement mus begin with 'return'.");
            // throws exception if doesn't state what to return
            if (tokens.Count == 1) throw new ParseException("Return statement is missing expression after 'return'.");

            // everything after the return keyword is treated as the return expression
            List<Token> exprTokens = tokens.GetRange(1, tokens.Count - 1);
            AST.ExpressionNode expr = ParseExpression(exprTokens);
            return new ReturnStmt(expr);

        }

        /// <summary>
        /// Parses a generic statement by dispatching to the appropriate specialized parser
        /// based on the first token.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="symbolTable"></param>
        /// <returns><see cref="AST.Statement"/></returns>
        /// <exception cref="ParseException"></exception>
        private static AST.Statement ParseStatement(List<Tokenizer.Token> tokens, SymbolTable<string, object> symbolTable)
        {
            if (tokens.Count == 0) throw new ParseException("No tokens found.");
            if (tokens[0].GetTokenType() == TokenType.RETURN) return ParseReturnStatement(tokens);
            if (tokens[0].GetTokenType() == TokenType.VARIABLE) return ParseAssignmentStmt(tokens, symbolTable);
            throw new ParseException($"Unknown beginning value in tokens of {tokens[0].GetValue()}");
        }

        /// <summary>
        /// Parses a list of source lines into statements or expressions and appends them
        /// to the provided block.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="block"></param>
        /// <param name="symboltable"></param>
        /// <exception cref="ParseException"></exception>
        private static void ParseStmtList(List<string> lines, SymbolTable<string, object> symbolTable, BlockStmt block)
        {
            // throws exception if nother to parse
            if (lines.Count == 0) throw new ParseException("No lines to parse.");

            // tokenizes elements within the lines
            TokenizerImpl tokenizer = new TokenizerImpl();
            List<Token> lineTokens = tokenizer.Tokenize(lines[0]);

            // checks if the element is the end of the block
            if (lineTokens[0].GetTokenType() == TokenType.RIGHT_CURLY)
            {
                // throws an exception if not singular curly brace on closing line
                if (lineTokens.Count > 1) throw new ParseException("Expected 1 token on closing line.");

            }

            // checks if there is another block
            else if (lineTokens[0].GetTokenType() == TokenType.LEFT_CURLY)
            {
                // calls the parsing of that block statement
                BlockStmt otherblock = ParseBlockStmt(lines, new SymbolTable<string, object>(symbolTable));
                // adds it to the parent block statement
                block.AddState(otherblock);
                // parses the next statement
                ParseStmtList(lines, symbolTable, block);
            }

            else
            {
                // checks if there is an invalid token and throws exception
                if (lineTokens[0].GetTokenType() != TokenType.RETURN && lineTokens[0].GetTokenType() != TokenType.VARIABLE)
                    throw new ParseException("Invalid token.");

                // removes the curly brace
                lines.RemoveAt(0);
                // parses the statement and adds it to the block
                block.AddState(ParseStatement(lineTokens, symbolTable));
                // continues the parsing
                ParseStmtList(lines, symbolTable, block);
            }

        }

        /// <summary>
        /// Initiates the parsing of the block statements within the given
        /// lines from the beginning parser.
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="symboltable"></param>
        /// /// <param name="block"></param>
        /// <exception cref="ParseException"></exception>
        public static AST.BlockStmt ParseBlockStmt(List<string> lines, SymbolTable<string, object> symboltable)
        {
            // throws an exception if the block is invalid
            if (lines == null || lines.Count == 0) throw new ParseException("Invalid block.");
            // tokenizes the first line of the block
            TokenizerImpl tokenizer = new TokenizerImpl();
            List<Token> tokens = tokenizer.Tokenize(lines[0]);

            // throws an exception if the block does now begin with '{'
            if (tokens == null || tokens.Count == 0 || tokens[0].GetTokenType() != TokenType.LEFT_CURLY) throw new ParseException("Block must begin with '{'.");
            // throws an exception if there is more than one token on opening line of block ('{')
            if (tokens.Count > 1) throw new ParseException("Expected 1 token on opening line.");

            // removes beginning parentheses of the block
            lines.RemoveAt(0);

            // create blocks statement of singular statement with symbol table passed in
            BlockStmt blockStmt = new BlockStmt(symboltable);
            // calls parsing statement
            ParseStmtList(lines, symboltable, blockStmt);

            //if it reaches end of the block, consumes ending parenthesis and returns the block;
            lines.RemoveAt(0);
            return blockStmt;

        }

    }

    /// <summary>
    /// creation of ParseException.
    /// </summary>
    [Serializable]
    internal class ParseException : Exception
    {
        public ParseException()
        {
        }

        public ParseException(string? message) : base(message)
        {
        }

        public ParseException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
