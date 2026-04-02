using System;
using Xunit;
using AST;
using Parser;

namespace AST.Tests
{
    public class EvaluateVisitorTests
    {
        private readonly EvaluateVisitor _evaluator;

        public EvaluateVisitorTests()
        {
            _evaluator = new EvaluateVisitor();
        }

        private static SymbolTable<string, object> NewTable()
        {
            return new SymbolTable<string, object>();
        }

        private object Run(string program)
        {
            BlockStmt ast = Parser.Parser.Parse(program);
            return _evaluator.Evaluate(ast);
        }

        // -------------------------------------------------
        // Direct visitor tests
        // -------------------------------------------------

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(-7)]
        public void Visit_LiteralNode_ReturnsStoredValue(int value)
        {
            LiteralNode node = new LiteralNode(value);
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(value, Convert.ToInt32(result));
        }

        [Fact]
        public void Visit_VariableNode_ReturnsStoredValue()
        {
            SymbolTable<string, object> table = NewTable();
            table["x"] = 12;

            VariableNode node = new VariableNode("x");
            object result = node.Accept(_evaluator, table);

            Assert.Equal(12, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(3, 4, 7)]
        [InlineData(0, 0, 0)]
        [InlineData(-1, 1, 0)]
        public void Visit_PlusNode_ReturnsCorrectSum(int left, int right, int expected)
        {
            PlusNode node = new PlusNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(10, 3, 7)]
        [InlineData(5, 5, 0)]
        [InlineData(0, 5, -5)]
        public void Visit_MinusNode_ReturnsCorrectDifference(int left, int right, int expected)
        {
            MinusNode node = new MinusNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(3, 4, 12)]
        [InlineData(0, 5, 0)]
        [InlineData(-2, 3, -6)]
        public void Visit_TimesNode_ReturnsCorrectProduct(int left, int right, int expected)
        {
            TimesNode node = new TimesNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(7, 2, 3.5f)]
        [InlineData(9, 3, 3.0f)]
        [InlineData(1, 4, 0.25f)]
        public void Visit_FloatDivNode_ReturnsCorrectQuotient(int left, int right, float expected)
        {
            FloatDivNode node = new FloatDivNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToSingle(result));
        }

        [Theory]
        [InlineData(7, 2, 3)]
        [InlineData(10, 3, 3)]
        [InlineData(6, 3, 2)]
        public void Visit_IntDivNode_ReturnsCorrectQuotient(int left, int right, int expected)
        {
            IntDivNode node = new IntDivNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(10, 3, 1)]
        [InlineData(9, 3, 0)]
        [InlineData(7, 4, 3)]
        public void Visit_ModulusNode_ReturnsCorrectRemainder(int left, int right, int expected)
        {
            ModulusNode node = new ModulusNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(2, 4, 16)]
        [InlineData(3, 3, 27)]
        [InlineData(5, 0, 1)]
        public void Visit_ExponentiationNode_ReturnsCorrectPower(int left, int right, double expected)
        {
            ExponentiationNode node = new ExponentiationNode(new LiteralNode(left), new LiteralNode(right));
            object result = node.Accept(_evaluator, NewTable());

            Assert.Equal(expected, Convert.ToDouble(result));
        }

        [Fact]
        public void Visit_FloatDivNode_ByZero_ThrowsException()
        {
            FloatDivNode node = new FloatDivNode(new LiteralNode(5), new LiteralNode(0));
            Assert.ThrowsAny<Exception>(() => node.Accept(_evaluator, NewTable()));
        }

        [Fact]
        public void Visit_IntDivNode_ByZero_ThrowsException()
        {
            IntDivNode node = new IntDivNode(new LiteralNode(10), new LiteralNode(0));
            Assert.ThrowsAny<Exception>(() => node.Accept(_evaluator, NewTable()));
        }

        [Fact]
        public void Visit_ModulusNode_ByZero_ThrowsException()
        {
            ModulusNode node = new ModulusNode(new LiteralNode(10), new LiteralNode(0));
            Assert.ThrowsAny<Exception>(() => node.Accept(_evaluator, NewTable()));
        }

        // -------------------------------------------------
        // Integration tests
        // -------------------------------------------------

        [Fact]
        public void Evaluate_ReturnLiteral_ReturnsLiteral()
        {
            object result = Run(@"{
                return 7
            }");

            Assert.Equal(7, Convert.ToInt32(result));
        }

        [Fact]
        public void Evaluate_AssignmentThenReturnVariable_ReturnsCorrectValue()
        {
            object result = Run(@"{
                x := 42
                return x
            }");

            Assert.Equal(42, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData("{\nreturn (3 + 4)\n}", 7)]
        [InlineData("{\nreturn (10 - 6)\n}", 4)]
        [InlineData("{\nreturn (3 * 5)\n}", 15)]
        [InlineData("{\nreturn (9 // 2)\n}", 4)]
        [InlineData("{\nreturn (10 % 3)\n}", 1)]
        public void Evaluate_ArithmeticPrograms_ReturnCorrectIntegerValue(string program, int expected)
        {
            object result = Run(program);
            Assert.Equal(expected, Convert.ToInt32(result));
        }

        [Fact]
        public void Evaluate_FloatDivisionProgram_ReturnsCorrectValue()
        {
            object result = Run(@"{
                return (7 / 2)
            }");

            Assert.Equal(3.5f, Convert.ToSingle(result));
        }

        [Fact]
        public void Evaluate_ExponentiationProgram_ReturnsCorrectValue()
        {
            object result = Run(@"{
                return (2 ** 4)
            }");

            Assert.Equal(16, Convert.ToDouble(result));
        }

        [Fact]
        public void Evaluate_ChainedAssignments_ReturnsCorrectValue()
        {
            object result = Run(@"{
                a := 10
                b := 5
                c := (a + b)
                return c
            }");

            Assert.Equal(15, Convert.ToInt32(result));
        }

        [Theory]
        [InlineData(@"{ return (10 // 0) }")]
        [InlineData(@"{ return (5 / 0) }")]
        [InlineData(@"{ return (8 % 0) }")]
        public void Evaluate_DivisionByZeroPrograms_ThrowException(string program)
        {
            Assert.ThrowsAny<Exception>(() => Run(program));
        }

        [Fact]
        public void Evaluate_ReturnStopsExecution()
        {
            object result = Run(@"{
                return 1
                return 999
            }");

            Assert.Equal(1, Convert.ToInt32(result));
        }

        [Fact]
        public void Evaluate_NoExplicitReturn_ReturnsLastStatementValue()
        {
            object result = Run(@"{
                a := 5
                b := (a * 2)
            }");

            Assert.Equal(10, Convert.ToInt32(result));
        }
    }
}
