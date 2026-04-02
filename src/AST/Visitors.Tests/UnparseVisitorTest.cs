using Xunit;
using AST;
using AST.Visitors;

namespace AST.Visitors.Tests
{
    /// <summary>
    /// Direct visitor tests for UnparseVisitor.
    /// AST nodes are constructed manually using the actual signatures from AST.cs,
    /// then visited to verify the unparsed string output.
    ///
    /// Key constructor notes from the real AST:
    ///   - BlockStmt(SymbolTable<string,object>)  — then AddState(stmt) to populate
    ///   - AssignmentStmt(VariableNode, ExpressionNode)  — NOT (string, ExpressionNode)
    ///   - ExponentiationNode._op == "**"  (not "^")
    /// </summary>
    public class UnparseVisitorTest
    {
        private readonly UnparseVisitor _visitor;

        public UnparseVisitorTest()
        {
            _visitor = new UnparseVisitor();
        }

        // -----------------------------------------------------------------------
        // Shared helpers
        // -----------------------------------------------------------------------

        /// <summary>Returns a fresh empty BlockStmt backed by a new SymbolTable.</summary>
        private static BlockStmt EmptyBlock()
            => new BlockStmt(new SymbolTable<string, object>());

        /// <summary>Creates a BlockStmt and populates it with the supplied statements.</summary>
        private static BlockStmt BlockOf(params Statement[] stmts)
        {
            var block = EmptyBlock();
            foreach (var s in stmts)
                block.AddState(s);
            return block;
        }

        /// <summary>
        /// Convenience wrapper: AssignmentStmt requires a VariableNode as its first
        /// argument, not a plain string.
        /// </summary>
        private static AssignmentStmt Assign(string name, ExpressionNode expr)
            => new AssignmentStmt(new VariableNode(name), expr);

        // -----------------------------------------------------------------------
        // LiteralNode
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData(42,   "42")]
        [InlineData(0,    "0")]
        [InlineData(3.14, "3.14")]
        public void Unparse_Literal_ReturnsValueString(object value, string expected)
        {
            var node = new LiteralNode(value);
            string result = node.Accept(_visitor, 0);
            Assert.Equal(expected, result);
        }

        // -----------------------------------------------------------------------
        // VariableNode
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("result")]
        public void Unparse_Variable_ReturnsName(string name)
        {
            var node = new VariableNode(name);
            string result = node.Accept(_visitor, 0);
            Assert.Equal(name, result);
        }

        // -----------------------------------------------------------------------
        // Binary expression nodes — each should parenthesize its operands
        // -----------------------------------------------------------------------

        [Fact]
        public void Unparse_Plus_ProducesParenthesizedExpression()
        {
            var node = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            Assert.Equal("(1 + 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_Minus_ProducesParenthesizedExpression()
        {
            var node = new MinusNode(new LiteralNode(10), new LiteralNode(3));
            Assert.Equal("(10 - 3)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_Times_ProducesParenthesizedExpression()
        {
            var node = new TimesNode(new LiteralNode(4), new LiteralNode(5));
            Assert.Equal("(4 * 5)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_FloatDiv_ProducesParenthesizedExpression()
        {
            var node = new FloatDivNode(new LiteralNode(7), new LiteralNode(2));
            Assert.Equal("(7 / 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_IntDiv_ProducesParenthesizedExpression()
        {
            var node = new IntDivNode(new LiteralNode(7), new LiteralNode(2));
            Assert.Equal("(7 // 2)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_Modulus_ProducesParenthesizedExpression()
        {
            var node = new ModulusNode(new LiteralNode(10), new LiteralNode(3));
            Assert.Equal("(10 % 3)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_Exponentiation_UsesTwoStarOperator()
        {
            // ExponentiationNode._op is "**" in AST.cs — not "^"
            var node = new ExponentiationNode(new LiteralNode(2), new LiteralNode(8));
            Assert.Equal("(2 ** 8)", node.Accept(_visitor, 0));
        }

        // -----------------------------------------------------------------------
        // Nested expressions
        // -----------------------------------------------------------------------

        [Fact]
        public void Unparse_NestedArithmetic_ProducesCorrectParens()
        {
            // (1 + 2) * 3  =>  "((1 + 2) * 3)"
            var node = new TimesNode(
                new PlusNode(new LiteralNode(1), new LiteralNode(2)),
                new LiteralNode(3));
            Assert.Equal("((1 + 2) * 3)", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_DeeplyNestedExpression_ProducesCorrectParens()
        {
            // (x + 1) * (y - 2)  =>  "((x + 1) * (y - 2))"
            var node = new TimesNode(
                new PlusNode(new VariableNode("x"), new LiteralNode(1)),
                new MinusNode(new VariableNode("y"), new LiteralNode(2)));
            Assert.Equal("((x + 1) * (y - 2))", node.Accept(_visitor, 0));
        }

        [Fact]
        public void Unparse_ExpressionWithVariable_IncludesVariableName()
        {
            var node = new PlusNode(new VariableNode("x"), new LiteralNode(5));
            Assert.Equal("(x + 5)", node.Accept(_visitor, 0));
        }

        // -----------------------------------------------------------------------
        // AssignmentStmt — first arg is VariableNode, not a plain string
        // -----------------------------------------------------------------------

        [Fact]
        public void Unparse_Assignment_ProducesCorrectFormat()
        {
            string result = Assign("x", new LiteralNode(42)).Accept(_visitor, 0);
            Assert.Equal("x := 42", result);
        }

        [Fact]
        public void Unparse_Assignment_WithExpression_ProducesCorrectFormat()
        {
            var expr = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            string result = Assign("total", expr).Accept(_visitor, 0);
            Assert.Equal("total := (1 + 2)", result);
        }

        [Theory]
        [InlineData("a", 10, "a := 10")]
        [InlineData("b",  0, "b := 0")]
        public void Unparse_AssignmentVariants_ProduceExpectedText(
            string name, int value, string expected)
        {
            string result = Assign(name, new LiteralNode(value)).Accept(_visitor, 0);
            Assert.Equal(expected, result);
        }

        // -----------------------------------------------------------------------
        // ReturnStmt
        // -----------------------------------------------------------------------

        [Fact]
        public void Unparse_Return_ProducesCorrectFormat()
        {
            string result = new ReturnStmt(new LiteralNode(0)).Accept(_visitor, 0);
            Assert.Equal("return 0", result);
        }

        [Fact]
        public void Unparse_Return_WithComplexExpression_ProducesCorrectFormat()
        {
            var expr = new TimesNode(new VariableNode("x"), new LiteralNode(2));
            string result = new ReturnStmt(expr).Accept(_visitor, 0);
            Assert.Equal("return (x * 2)", result);
        }

        // -----------------------------------------------------------------------
        // BlockStmt — SymbolTable + AddState construction, indentation
        // -----------------------------------------------------------------------

        [Fact]
        public void Unparse_EmptyBlock_ContainsBraces()
        {
            string result = EmptyBlock().Accept(_visitor, 0);
            Assert.Contains("{", result);
            Assert.Contains("}", result);
        }

        [Fact]
        public void Unparse_BlockWithSingleAssignment_ContainsStatement()
        {
            string result = BlockOf(Assign("x", new LiteralNode(1))).Accept(_visitor, 0);
            Assert.Contains("x := 1", result);
        }

        [Fact]
        public void Unparse_BlockWithMultipleStatements_ContainsAllStatements()
        {
            var block = BlockOf(
                Assign("x", new LiteralNode(1)),
                Assign("y", new LiteralNode(2)),
                new ReturnStmt(new PlusNode(new VariableNode("x"), new VariableNode("y")))
            );
            string result = block.Accept(_visitor, 0);

            Assert.Contains("x := 1",         result);
            Assert.Contains("y := 2",         result);
            Assert.Contains("return (x + y)", result);
        }

        [Fact]
        public void Unparse_Block_IndentsStatementsInsideBraces()
        {
            string result = BlockOf(new ReturnStmt(new LiteralNode(1))).Accept(_visitor, 0);

            // The "return 1" line should have leading whitespace
            string returnLine = result.Split('\n').First(l => l.TrimStart().StartsWith("return"));
            Assert.True(returnLine.StartsWith(" "),
                "Statements inside a block should be indented.");
        }

        [Fact]
        public void Unparse_NestedBlock_InnerStatementsMoreIndented()
        {
            var inner = BlockOf(new ReturnStmt(new LiteralNode(99)));
            var outer = BlockOf(inner);
            string result = outer.Accept(_visitor, 0);

            string returnLine = result.Split('\n').First(l => l.TrimStart().StartsWith("return"));
            int leadingSpaces = returnLine.Length - returnLine.TrimStart().Length;
            // Two levels of indentation at 4 spaces each = 8 spaces minimum
            Assert.True(leadingSpaces >= 8,
                $"Expected >= 8 leading spaces at depth 2; got {leadingSpaces}.");
        }

        [Fact]
        public void Unparse_IndentationParam_AffectsLeadingWhitespace()
        {
            var block = BlockOf(Assign("x", new LiteralNode(5)));
            string atLevel0 = block.Accept(_visitor, 0);
            string atLevel1 = block.Accept(_visitor, 1);

            Assert.True(atLevel1.Length > atLevel0.Length,
                "Level-1 output should be strictly longer than level-0 output.");
        }
    }
}