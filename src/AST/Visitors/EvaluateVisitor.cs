using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Serialization;
using AST;

namespace AST
{
    /// <summary>
    /// Exception thrown when an evaluation error occurs
    /// </summary>
    public class EvaluationException : Exception
    {
        public EvaluationException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// Visitor that evaluates an AST, executing the program and returning the final value
    /// Uses symbol tables to store variable values during execution
    /// </summary>
    public class EvaluateVisitor : IVisitor<SymbolTable<string, object>, object>
    {
        // Flag to indicate if a return statement has been encountered
        private bool _returnEncountered;

        // Value from the return statement
        private object _returnValue;

        /// <summary>
        /// Initializes a new instance of the EvaluateVisitor class
        /// </summary>
        public EvaluateVisitor()
        {
            _returnEncountered = false;
            _returnValue = null;
        }

        /// <summary>
        /// Evaluates the given AST and returns the result
        /// </summary>
        /// <param name="ast">The AST to evaluate</param>
        /// <returns>The result of the evaluation (typically from a return statement)</returns>
        public object Evaluate(Statement ast)
        {
            _returnEncountered = false;
            _returnValue = null;

            // Execute the AST with a null initial scope
            // (the BlockStmt will use its own symbol table)
            return(ast.Accept(this, null));

            //return _returnValue;
        }

        // return the corresponding value of the variable in the symboltable 
        // TODO

        public object GetVariableValue(string name, SymbolTable<string, object> symbolTable)
        {
            if (symbolTable == null) throw new EvaluationException("Symbol Table is empty.");
            if (symbolTable.TryGetValue(name, out object value)) return value;

            throw new EvaluationException("Undefined variable.");
        }
        #region Expression Node Visit Methods

        public object Visit(PlusNode node, SymbolTable<string, object> symbolTable)
        {
            // make this a loop/recursive to account for multiple expressions
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);

            return Convert.ToDouble(left) + Convert.ToDouble(right);

        }

        public object Visit(MinusNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);

            return Convert.ToInt32(left) - Convert.ToInt32(right);
        }

        public object Visit(TimesNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);

            return Convert.ToDouble(left) * Convert.ToDouble(right);
        }

        public object Visit(FloatDivNode node, SymbolTable<string, object> symbolTable)
        {
            if (node._left.ToString() == "0" || node._right.ToString() == "0") throw new EvaluationException("Cannot divide by 0.");
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);
            if (right.ToString() == "0") throw new EvaluationException("Cannot do a division by 0.");
            //if (left == "0" || right == "0") throw new EvaluateException("Cannot divide by 0.");

            return (float)Convert.ToDouble(left) / Convert.ToDouble(right);
        }

        public object Visit(IntDivNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);
            if (right.ToString() == "0") throw new EvaluationException("Cannot do a division by 0.");

            return Convert.ToInt32(left) / Convert.ToInt32(right);
        }

        public object Visit(ModulusNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);
            if (right.ToString() == "0") throw new EvaluationException("Cannot do a division by 0.");
            return Convert.ToInt32(left) % Convert.ToInt32(right);
        }

        public object Visit(ExponentiationNode node, SymbolTable<string, object> symbolTable)
        {
            object left = node._left.Accept(this, symbolTable);
            object right = node._right.Accept(this, symbolTable);

            return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
        }

        // TODO

        public Object Visit(LiteralNode node, SymbolTable<string, object> symbolTable)
        {
            return node._value;
        }
        public object Visit(VariableNode node, SymbolTable<string, object> symbolTable)
        {
            if (symbolTable.TryGetValue(node._data, out object value)) return value;

            throw new EvaluationException("Variable not found.");
            // Variables return their value from the symbol table
        }

        #endregion

        #region Statement Node Visit Methods

        public object Visit(AssignmentStmt node, SymbolTable<string, object> symbolTable)
        {
            if (_returnEncountered) return _returnValue;

            object value = node._expr.Accept(this, symbolTable);
            symbolTable[node._var._data] = value;
            return value;

        }

        public object Visit(ReturnStmt node, SymbolTable<string, object> symbolTable)
        {
            if (_returnEncountered) return _returnValue;
            _returnValue = node._expr.Accept(this, symbolTable);
            _returnEncountered = true;
            return _returnValue;
        }
        // TODO

        public object Visit(BlockStmt node, SymbolTable<string, object> symbolTable)
        {
            // Use this block's symbol table, which is already linked to its parent
            SymbolTable<string, object> currentScope = node.SymbolTable;

            object lastValue = null;
            foreach (Statement stmt in node._statements)
            {
                lastValue = stmt.Accept(this, currentScope);
                
            }
            if (_returnEncountered) return _returnValue;
            return lastValue;
        }
        
        

        #endregion
    }
}