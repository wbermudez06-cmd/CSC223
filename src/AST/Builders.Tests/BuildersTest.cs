using Xunit;
using System;
using System.IO;
using System.Collections.Generic;
using DEC.AST;
using AST;

namespace DEC.AST.Tests
{
    // =========================================================
    // DEFAULT BUILDER TESTS
    // =========================================================
    public class DefaultBuilderTests
    {
        private readonly DefaultBuilder _builder = new DefaultBuilder();

        // --- Literal Node ---

        [Theory]
        [InlineData(0)]
        [InlineData(42)]
        [InlineData(-7)]
        public void BuildLiteralNode_ReturnsLiteralNodeWithCorrectValue(int value)
        {
            var node = _builder.CreateLiteralNode(value);
            Assert.IsType<LiteralNode>(node);
            Assert.Equal(value, ((LiteralNode)node)._value);
        }

        // --- Variable Node ---

        [Theory]
        [InlineData("x")]
        [InlineData("myVar")]
        [InlineData("counter")]
        public void BuildVariableNode_ReturnsVariableNodeWithCorrectName(string name)
        {
            var node = _builder.CreateVariableNode(name);
            Assert.IsType<VariableNode>(node);
            Assert.Equal(name, ((VariableNode)node)._data);
        }

        // --- Binary Operator Nodes ---

        [Fact]
        public void BuildPlusNode_ReturnsPlusNode()
        {
            var node = _builder.CreatePlusNode(new LiteralNode(1), new LiteralNode(2));
            Assert.IsType<PlusNode>(node);
        }

        [Fact]
        public void BuildMinusNode_ReturnsMinusNode()
        {
            var node = _builder.CreateMinusNode(new LiteralNode(5), new LiteralNode(3));
            Assert.IsType<MinusNode>(node);
        }

        [Fact]
        public void BuildTimesNode_ReturnsTimesNode()
        {
            var node = _builder.CreateTimesNode(new LiteralNode(2), new LiteralNode(4));
            Assert.IsType<TimesNode>(node);
        }

        [Fact]
        public void BuildFloatDivNode_ReturnsFloatDivNode()
        {
            var node = _builder.CreateFloatDivNode(new LiteralNode(10), new LiteralNode(3));
            Assert.IsType<FloatDivNode>(node);
        }

        [Fact]
        public void BuildIntDivNode_ReturnsIntDivNode()
        {
            var node = _builder.CreateIntDivNode(new LiteralNode(10), new LiteralNode(3));
            Assert.IsType<IntDivNode>(node);
        }

        [Fact]
        public void BuildModulusNode_ReturnsModulusNode()
        {
            var node = _builder.CreateModulusNode(new LiteralNode(10), new LiteralNode(3));
            Assert.IsType<ModulusNode>(node);
        }

        [Fact]
        public void BuildExponentiationNode_ReturnsExponentiationNode()
        {
            var node = _builder.CreateExponentiationNode(new LiteralNode(2), new LiteralNode(8));
            Assert.IsType<ExponentiationNode>(node);
        }

        // Verify children are stored correctly for a binary node
        [Theory]
        [InlineData(1, 2)]
        [InlineData(0, 99)]
        [InlineData(-3, 3)]
        public void BuildPlusNode_StoresCorrectChildren(int leftVal, int rightVal)
        {
            var left = new LiteralNode(leftVal);
            var right = new LiteralNode(rightVal);
            var node = (PlusNode)_builder.CreatePlusNode(left, right);
            Assert.Equal(left, node._left);
            Assert.Equal(right, node._right);
        }

        // --- Statement Nodes ---

        [Fact]
        public void BuildAssignmentStmt_ReturnsAssignmentStmt()
        {
            var stmt = _builder.CreateAssignmentStmt(new VariableNode("x"), new LiteralNode(1));
            Assert.IsType<AssignmentStmt>(stmt);
        }

        [Fact]
        public void BuildAssignmentStmt_StoresCorrectChildren()
        {
            var varNode = new VariableNode("a");
            var litNode = new LiteralNode(5);
            var stmt = (AssignmentStmt)_builder.CreateAssignmentStmt(varNode, litNode);
            Assert.Equal(varNode, stmt._var);
            Assert.Equal(litNode, stmt._expr);
        }

        [Fact]
        public void BuildReturnStmt_ReturnsReturnStmt()
        {
            var stmt = _builder.CreateReturnStmt(new LiteralNode(0));
            Assert.IsType<ReturnStmt>(stmt);
        }

        [Fact]
        public void BuildReturnStmt_StoresCorrectExpression()
        {
            var expr = new PlusNode(new LiteralNode(1), new LiteralNode(2));
            var stmt = (ReturnStmt)_builder.CreateReturnStmt(expr);
            Assert.Equal(expr, stmt._expr);
        }

        [Fact]
        public void BuildBlockStmt_ReturnsBlockStmt()
        {
            var block = _builder.CreateBlockStmt(new SymbolTable<string, object>());
            Assert.IsType<BlockStmt>(block);
        }

    }

    // =========================================================
    // NULL BUILDER TESTS
    // =========================================================
    public class NullBuilderTests
    {
        private readonly NullBuilder _builder = new NullBuilder();

        [Fact]
        public void BuildLiteralNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateLiteralNode(5));
        }

        [Fact]
        public void BuildVariableNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateVariableNode("x"));
        }

        [Fact]
        public void BuildPlusNode_ReturnsNull()
        {
            Assert.Null(_builder.CreatePlusNode(null, null));
        }

        [Fact]
        public void BuildMinusNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateMinusNode(null, null));
        }

        [Fact]
        public void BuildTimesNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateTimesNode(null, null));
        }

        [Fact]
        public void BuildFloatDivNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateFloatDivNode(null, null));
        }

        [Fact]
        public void BuildIntDivNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateIntDivNode(null, null));
        }

        [Fact]
        public void BuildModulusNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateModulusNode(null, null));
        }

        [Fact]
        public void BuildExponentiationNode_ReturnsNull()
        {
            Assert.Null(_builder.CreateExponentiationNode(null, null));
        }

        [Fact]
        public void BuildAssignmentStmt_ReturnsNull()
        {
            Assert.Null(_builder.CreateAssignmentStmt(null, null));
        }

        [Fact]
        public void BuildReturnStmt_ReturnsNull()
        {
            Assert.Null(_builder.CreateReturnStmt(null));
        }

        [Fact]
        public void BuildBlockStmt_ReturnsNull()
        {
            Assert.Null(_builder.CreateBlockStmt(null));
        }
    }


    // =========================================================
    // DEBUG BUILDER TESTS
    // =========================================================
    public class DebugBuilderTests
    {
        private readonly DebugBuilder _builder = new DebugBuilder();

        // Helper to capture console output during an action
        private string CaptureConsoleOutput(Action action)
        {
            var sw = new StringWriter();
            var original = Console.Out;
            Console.SetOut(sw);
            action();
            Console.SetOut(original);
            return sw.ToString();
        }

        // --- Returns correct types (inherits DefaultBuilder behavior) ---

        [Fact]
        public void BuildLiteralNode_ReturnsLiteralNode()
        {
            var node = _builder.CreateLiteralNode(3);
            Assert.IsType<LiteralNode>(node);
        }

        [Fact]
        public void BuildVariableNode_ReturnsVariableNode()
        {
            var node = _builder.CreateVariableNode("y");
            Assert.IsType<VariableNode>(node);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(10, 20)]
        public void BuildPlusNode_ReturnsCorrectNodeWithChildren(int left, int right)
        {
            var node = (PlusNode)_builder.CreatePlusNode(new LiteralNode(left), new LiteralNode(right));
            Assert.IsType<PlusNode>(node);
            Assert.Equal(left, ((LiteralNode)node._left)._value);
            Assert.Equal(right, ((LiteralNode)node._right)._value);
        }

        [Fact]
        public void BuildAssignmentStmt_ReturnsAssignmentStmt()
        {
            var stmt = _builder.CreateAssignmentStmt(new VariableNode("x"), new LiteralNode(1));
            Assert.IsType<AssignmentStmt>(stmt);
        }

        [Fact]
        public void BuildReturnStmt_ReturnsReturnStmt()
        {
            var stmt = _builder.CreateReturnStmt(new LiteralNode(0));
            Assert.IsType<ReturnStmt>(stmt);
        }

        [Fact]
        public void BuildBlockStmt_ReturnsBlockStmt()
        {
            var block = _builder.CreateBlockStmt(new SymbolTable<string, object>());
            Assert.IsType<BlockStmt>(block);
        }

        // --- Console output is written ---

        [Fact]
        public void BuildLiteralNode_WritesOutputToConsole()
        {
            var output = CaptureConsoleOutput(() => _builder.CreateLiteralNode(7));
            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        [Fact]
        public void BuildVariableNode_WritesOutputToConsole()
        {
            var output = CaptureConsoleOutput(() => _builder.CreateVariableNode("z"));
            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        [Fact]
        public void BuildPlusNode_WritesOutputToConsole()
        {
            var output = CaptureConsoleOutput(() =>
                _builder.CreatePlusNode(new LiteralNode(1), new LiteralNode(2)));
            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        [Fact]
        public void BuildAssignmentStmt_WritesOutputToConsole()
        {
            var output = CaptureConsoleOutput(() =>
                _builder.CreateAssignmentStmt(new VariableNode("a"), new LiteralNode(1)));
            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        [Fact]
        public void BuildReturnStmt_WritesOutputToConsole()
        {
            var output = CaptureConsoleOutput(() => _builder.CreateReturnStmt(new LiteralNode(0)));
            Assert.False(string.IsNullOrWhiteSpace(output));
        }

        [Fact]
        public void BuildBlockStmt_WritesOutputToConsole()
        {
            var output = CaptureConsoleOutput(() =>
                _builder.CreateBlockStmt(new SymbolTable<string, object>()));
            Assert.False(string.IsNullOrWhiteSpace(output));
        }
    }

}
