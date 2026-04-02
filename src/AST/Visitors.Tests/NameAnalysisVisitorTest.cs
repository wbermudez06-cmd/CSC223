using Xunit;
using AST;
using AST.Visitors;
using Parser;

namespace AST.Visitors.Tests
{
    public class NameAnalysisVisitorTest
    {
        private readonly NameAnalysisVisitor _analyzer;

        public NameAnalysisVisitorTest()
        {
            _analyzer = new NameAnalysisVisitor();
        }

        // -----------------------------------------------------------------------
        // Shared helpers
        // -----------------------------------------------------------------------

        private static SymbolTable<string, object> NewTable()
            => new SymbolTable<string, object>();

        // Use Add(KeyValuePair) since that is the actual SymbolTable API
        private static SymbolTable<string, object> TableWith(params string[] names)
        {
            var table = NewTable();
            foreach (var name in names)
                table.Add(new KeyValuePair<string, object>(name, null));
            return table;
        }

        private static BlockStmt EmptyBlock()
            => new BlockStmt(NewTable());

        private static BlockStmt BlockOf(params Statement[] stmts)
        {
            var block = EmptyBlock();
            foreach (var s in stmts)
                block.AddState(s);
            return block;
        }

        private static AssignmentStmt Assign(string name, ExpressionNode expr)
            => new AssignmentStmt(new VariableNode(name), expr);

        private static Tuple<SymbolTable<string, object>, Statement> Param(
            SymbolTable<string, object> table, Statement ctx)
            => Tuple.Create(table, ctx);

        private static Statement DummyCtx() => new ReturnStmt(new LiteralNode(0));

        // ===================================================================
        // PART 1 – Direct visitor tests
        // ===================================================================

        // -----------------------------------------------------------------------
        // LiteralNode — always valid
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(42)]
        [InlineData(-3.14)]
        public void Visit_Literal_ReturnsTrue(double value)
        {
            bool result = new LiteralNode(value)
                .Accept(_analyzer, Param(NewTable(), DummyCtx()));
            Assert.True(result);
        }

        // -----------------------------------------------------------------------
        // VariableNode — defined vs. undefined
        // -----------------------------------------------------------------------

        [Fact]
        public void Visit_DefinedVariable_ReturnsTrue()
        {
            // Use Add(KeyValuePair) — SymbolTable has no Define method
            var table = TableWith("x");
            bool result = new VariableNode("x").Accept(_analyzer, Param(table, DummyCtx()));
            Assert.True(result);
        }

        [Fact]
        public void Visit_UndefinedVariable_ReturnsFalse()
        {
            bool result = new VariableNode("ghost").Accept(_analyzer, Param(NewTable(), DummyCtx()));
            Assert.False(result);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("total")]
        [InlineData("myVar")]
        public void Visit_MultipleDistinctUndefinedVariables_AllReturnFalse(string name)
        {
            bool result = new VariableNode(name).Accept(_analyzer, Param(NewTable(), DummyCtx()));
            Assert.False(result);
        }

        // -----------------------------------------------------------------------
        // Binary expression nodes
        // -----------------------------------------------------------------------

        [Fact]
        public void Visit_PlusWithBothDefined_ReturnsTrue()
        {
            var table = TableWith("a", "b");
            var node = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            Assert.True(node.Accept(_analyzer, Param(table, DummyCtx())));
        }

        [Fact]
        public void Visit_PlusWithLeftUndefined_ReturnsFalse()
        {
            var table = TableWith("b");  // only right side defined
            var node = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            Assert.False(node.Accept(_analyzer, Param(table, DummyCtx())));
        }

        [Fact]
        public void Visit_PlusWithRightUndefined_ReturnsFalse()
        {
            var table = TableWith("a");  // only left side defined
            var node = new PlusNode(new VariableNode("a"), new VariableNode("b"));
            Assert.False(node.Accept(_analyzer, Param(table, DummyCtx())));
        }

        [Fact]
        public void Visit_PlusWithBothUndefined_ReturnsFalse()
        {
            var node = new PlusNode(new VariableNode("x"), new VariableNode("y"));
            Assert.False(node.Accept(_analyzer, Param(NewTable(), DummyCtx())));
        }

        [Theory]
        [InlineData("minus")]
        [InlineData("times")]
        [InlineData("floatDiv")]
        [InlineData("intDiv")]
        [InlineData("modulus")]
        [InlineData("exponentiation")]
        public void Visit_BinaryOpWithUndefinedRightOperand_ReturnsFalse(string op)
        {
            var table = TableWith("lhs");  // only left defined

            ExpressionNode node = op switch
            {
                "minus"          => new MinusNode(new VariableNode("lhs"), new VariableNode("rhs")),
                "times"          => new TimesNode(new VariableNode("lhs"), new VariableNode("rhs")),
                "floatDiv"       => new FloatDivNode(new VariableNode("lhs"), new VariableNode("rhs")),
                "intDiv"         => new IntDivNode(new VariableNode("lhs"), new VariableNode("rhs")),
                "modulus"        => new ModulusNode(new VariableNode("lhs"), new VariableNode("rhs")),
                "exponentiation" => new ExponentiationNode(new VariableNode("lhs"), new VariableNode("rhs")),
                _ => throw new ArgumentException(op)
            };

            Assert.False(node.Accept(_analyzer, Param(table, DummyCtx())));
        }

        // -----------------------------------------------------------------------
        // AssignmentStmt
        // -----------------------------------------------------------------------

        [Fact]
        public void Visit_AssignmentWithLiteralRhs_ReturnsTrue()
        {
            var table = NewTable();
            var stmt  = Assign("x", new LiteralNode(5));
            Assert.True(stmt.Accept(_analyzer, Param(table, stmt)));
        }

        [Fact]
        public void Visit_AssignmentWithUndefinedRhs_ReturnsFalse()
        {
            var table = NewTable();
            var stmt  = Assign("x", new VariableNode("y"));  // "y" not defined
            Assert.False(stmt.Accept(_analyzer, Param(table, stmt)));
        }

        [Fact]
        public void Visit_AssignmentDefinesVariable_CanBeUsedAfterward()
        {
            var table  = NewTable();
            var assign = Assign("x", new LiteralNode(1));
            assign.Accept(_analyzer, Param(table, assign));

            // "x" should now be defined in the table
            bool result = new VariableNode("x").Accept(_analyzer, Param(table, DummyCtx()));
            Assert.True(result);
        }

        // -----------------------------------------------------------------------
        // ReturnStmt
        // -----------------------------------------------------------------------

        [Fact]
        public void Visit_ReturnWithLiteral_ReturnsTrue()
        {
            var stmt = new ReturnStmt(new LiteralNode(0));
            Assert.True(stmt.Accept(_analyzer, Param(NewTable(), stmt)));
        }

        [Fact]
        public void Visit_ReturnWithUndefinedVariable_ReturnsFalse()
        {
            var stmt = new ReturnStmt(new VariableNode("undef"));
            Assert.False(stmt.Accept(_analyzer, Param(NewTable(), stmt)));
        }

        // -----------------------------------------------------------------------
        // BlockStmt
        // -----------------------------------------------------------------------

        [Fact]
        public void Visit_BlockWithAllDefinedVariables_ReturnsTrue()
        {
            var block = BlockOf(
                Assign("a", new LiteralNode(1)),
                Assign("b", new LiteralNode(2)),
                new ReturnStmt(new PlusNode(new VariableNode("a"), new VariableNode("b")))
            );
            Assert.True(block.Accept(_analyzer, Param(NewTable(), block)));
        }

        [Fact]
        public void Visit_BlockWithUndefinedReference_ReturnsFalse()
        {
            var block = BlockOf(new ReturnStmt(new VariableNode("ghost")));
            Assert.False(block.Accept(_analyzer, Param(NewTable(), block)));
        }

        [Fact]
        public void Visit_Block_ContinuesAnalysisAfterFirstError()
        {
            // Both RHS variables are undefined — visitor must catch both
            var block = BlockOf(
                Assign("x", new VariableNode("undef1")),
                Assign("y", new VariableNode("undef2"))
            );
            Assert.False(block.Accept(_analyzer, Param(NewTable(), block)));
        }

        [Fact]
        public void Visit_NestedBlock_InnerScopeCanAccessOuterVariable()
        {
            var table = TableWith("outer");
            // Give the inner block the same table so ContainsKey finds "outer"
            var inner = new BlockStmt(table);
            inner.AddState(new ReturnStmt(new VariableNode("outer")));
            var outerTable = TableWith("outer");
            var outer = new BlockStmt(outerTable);
            outer.AddState(inner);
            Assert.True(outer.Accept(_analyzer, Param(outerTable, outer)));
}

        // ===================================================================
        // PART 2 – Integration tests (source string → Parse → Analyze)
        // Each statement must be on its own line — the parser splits on \n
        // ===================================================================

        private bool Analyze(string program)
        {
            BlockStmt ast = Parser.Parser.Parse(program);
            return _analyzer.Analyze(ast);
        }

        [Fact]
        public void Integration_AllVariablesDefined_ReturnsTrue()
        {
            string program =
                "{\n" +
                "x := 10\n" +
                "y := 20\n" +
                "return (x + y)\n" +
                "}";
            Assert.True(Analyze(program));
        }

        [Fact]
        public void Integration_UndefinedVariableInReturn_ReturnsFalse()
        {
            string program = "{\nreturn z\n}";
            Assert.False(Analyze(program));
        }

        [Fact]
        public void Integration_UseBeforeDefinition_ReturnsFalse()
        {
            string program =
            "{\n" +
            "y := (x + 1)\n" +
            "}";
        // x is never declared anywhere in this program so it won't be in the table
        Assert.False(Analyze(program));
        }

        // Each InlineData program has every statement on its own line
        [Theory]
        [InlineData("{\nreturn (p + q)\n}", false)]
        [InlineData("{\nx := 3\nreturn (x ** 2)\n}", true)]
        public void Integration_VariousPrograms_ReturnsExpected(string program, bool expected)
        {
            Assert.Equal(expected, Analyze(program));
        }

        [Fact]
        public void Integration_MultipleUndefinedVariables_AllCaughtInOnePass()
        {
            string program =
                "{\n" +
                "a := (p + 1)\n" +
                "b := (q + 2)\n" +
                "return r\n" +
                "}";
            Assert.False(Analyze(program));
        }

        [Fact]
        public void Integration_NestedBlock_OuterVariableAccessible_ReturnsTrue()
        {
            string program =
                "{\n" +
                "x := 10\n" +
                "{\n" +
                "y := (x + 5)\n" +
                "return y\n" +
                "}\n" +
                "}";
            Assert.True(Analyze(program));
        }

        [Fact]
        public void Integration_NestedBlock_InnerVariableNotVisibleOutside_ReturnsFalse()
        {
            string program =
                "{\n" +
                "{\n" +
                "inner := 99\n" +
                "}\n" +
                "return inner\n" +
                "}";
            Assert.False(Analyze(program));
        }

        [Fact]
        public void Integration_ReturnWithPartiallyUndefinedExpression_ReturnsFalse()
        {
            string program =
                "{\n" +
                "x := 5\n" +
                "return (x + missing)\n" +
                "}";
            Assert.False(Analyze(program));
        }

        // [Fact]
        // public void Integration_ErrorsCollectionPopulated_WhenUndefinedVariableFound()
        // {
        //     BlockStmt ast = Parser.Parser.Parse("{\n$\n}");
        //     bool valid = _analyzer.Analyze(ast);

        //     Assert.False(valid);
        //     Assert.NotEmpty(_analyzer.Errors);
        //     Assert.Contains(_analyzer.Errors, msg => msg.Contains("Invalid character"));
        // }

        [Fact]
        public void Integration_MultipleErrors_AllRecordedInErrors()
        {
            string program =
                "{\n" +
                "a := (p + 1)\n" +
                "b := (q - 2)\n" +
                "return r\n" +
                "}";
            BlockStmt ast = Parser.Parser.Parse(program);
            _analyzer.Analyze(ast);
            Assert.True(_analyzer.Errors.Count >= 3,
                $"Expected >= 3 errors; got {_analyzer.Errors.Count}.");
        }

        [Fact]
        public void Integration_EmptyBlock_ReturnsTrue()
        {
            Assert.True(Analyze("{\n}"));
        }
    }
}