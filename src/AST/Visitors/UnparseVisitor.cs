using System;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using System.Text;
using AST;

namespace AST
{
    /// <summary>
    /// Visitor implementation that unparses the AST back to string representation
    /// Uses the generic visitor pattern with indentation level as parameter and string as result
    /// </summary>
    public class UnparseVisitor : IVisitor<int, string>
    {
        /// <summary>
        /// Unparses the given AST node with the specified indentation level
        /// </summary>
        /// <param name="node">The AST node to unparse</param>
        /// <param name="level">The indentation level</param>
        /// <returns>String representation of the node</returns>
        public string Unparse(ExpressionNode node, int level = 0)
        {
            return node.Accept(this, level);
        }

        /// <summary>
        /// Unparses the given statement with the specified indentation level
        /// </summary>
        /// <param name="stmt">The statement to unparse</param>
        /// <param name="level">The indentation level</param>
        /// <returns>String representation of the statement</returns>

        #region Expression Node Visit Methods

        // Unparse the left and right sub-expressions and return a string "(left + right)"
        public string Visit(PlusNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} + {right})";
        }

        // Unparse the left and right sub-expressions and return a string "(left - right)"
        public string Visit(MinusNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} - {right})";
        }

        // Unparse the left and right sub-expressions and return a string "(left * right)"
        public string Visit(TimesNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} * {right})";
        }

        // Unparse the left and right sub-expressions and return a string "(left / right)"
        public string Visit(FloatDivNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} / {right})";
        }

        // Unparse the left and right sub-expressions and return a string "(left // right)"
        public string Visit(IntDivNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} // {right})";
        }

        // Unparse the left and right sub-expressions and return a string "(left % right)"
        public string Visit(ModulusNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} % {right})";
        }

        // Unparse the left and right sub-expressions and return a string "(left ** right)"
        public string Visit(ExponentiationNode node, int level)
        {
            string left = node._left.Accept(this, level);
            string right = node._right.Accept(this, level);
            return $"({left} ** {right})";
        }

        #endregion

        public string Unparse(Statement stmt, int level = 0)
        {
            return stmt.Accept(this, level);
        }

        #region Statement Node Visit Methods


        // Unparse the literal value and return it as a string
        public string Visit(LiteralNode node, int param)
        {
            return $"{node._value}";
        }

        // Unparse the variable name and return it as a string
        public string Visit(VariableNode node, int param)
        {
            return $"{node._data}";
        }

        // Unparse the left variable and right expression, and return a string "var := expr"
        public string Visit(AssignmentStmt node, int level)
        {
            string left = node._var.Accept(this, level);
            string right = node._expr.Accept(this, level);
            return $"{left} := {right}";
        }

        // Unparse the expression and return a string "return expr"
        public string Visit(ReturnStmt node, int level)
        {

            return $"return {node._expr.Accept(this, level)}";
        }

        // Unparse each statement in the block with increased indentation and return a string "{\n statements \n indent}"
        public string Visit(BlockStmt node, int level)
        {
            string indent = new string(' ', level * 4);
            string innerIndent = new string(' ', (level + 1) * 4);
            if (node._statements.Count == 0)
            {
                return $"{indent}{{\n{indent}}}";
            }
            List<string> lines = new List<string>();
            foreach (Statement stmt in node._statements)
                lines.Add(innerIndent + stmt.Accept(this, level + 1));

            string body = string.Join("\n", lines);
            return $"{indent}{{\n{body}\n{indent}}}";
        }
        #endregion
    }
}