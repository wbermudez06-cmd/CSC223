using System;
using System.Collections.Generic;

namespace AST
{
    /// <summary>
    /// NullBuilder that does not create any objects; useful for assessing parsing problems
    /// while avoiding object creation
    /// </summary>
    public class DebugBuilder : DefaultBuilder
    {
        // Override all creation methods to return null
        public override PlusNode CreatePlusNode(ExpressionNode left, ExpressionNode right)
        {
            PlusNode plusNode = base.CreatePlusNode(left, right);
            Console.WriteLine("Creating a plus node");
            return plusNode;
        }

        public override MinusNode CreateMinusNode(ExpressionNode left, ExpressionNode right)
        {
            MinusNode minusNode = base.CreateMinusNode(left, right);
            Console.WriteLine("Creating a minus node");
            return minusNode;
        }

        public override TimesNode CreateTimesNode(ExpressionNode left, ExpressionNode right)
        {
            TimesNode timesNode = base.CreateTimesNode(left, right);
            Console.WriteLine("Creating a times node");
            return timesNode;
        }

        public override FloatDivNode CreateFloatDivNode(ExpressionNode left, ExpressionNode right)
        {
            FloatDivNode floatdivNode = base.CreateFloatDivNode(left, right);
            Console.WriteLine("Creating a float division node");
            return floatdivNode;
        }

        public override IntDivNode CreateIntDivNode(ExpressionNode left, ExpressionNode right)
        {
            IntDivNode intdivNode = base.CreateIntDivNode(left, right);
            Console.WriteLine("Creating an integer division node");
            return intdivNode;
        }

        public override ModulusNode CreateModulusNode(ExpressionNode left, ExpressionNode right)
        {
            ModulusNode modulusNode = base.CreateModulusNode(left, right);
            Console.WriteLine("Creating a modulus node");
            return modulusNode;
        }

        public override ExponentiationNode CreateExponentiationNode(ExpressionNode left, ExpressionNode right)
        {
            ExponentiationNode expoNode = base.CreateExponentiationNode(left, right);
            Console.WriteLine("Creating an exponentiation node");
            return expoNode;
        }

        public override LiteralNode CreateLiteralNode(object value)
        {
            LiteralNode litNode = base.CreateLiteralNode(value);
            Console.WriteLine("Creating a literal node");
            return litNode;
        }

        public override VariableNode CreateVariableNode(string name)
        {
            VariableNode varNode = base.CreateVariableNode(name);
            Console.WriteLine("Creating a variable node");
            return varNode;
        }

        public override AssignmentStmt CreateAssignmentStmt(VariableNode variable, ExpressionNode expression)
        {
            AssignmentStmt asignStmtNode = base.CreateAssignmentStmt(variable, expression);
            Console.WriteLine("Creating an assignment statement node");
            return asignStmtNode;
        }

        public override ReturnStmt CreateReturnStmt(ExpressionNode expression)
        {
            ReturnStmt returnStmtNode = base.CreateReturnStmt(expression);
            Console.WriteLine("Creating a return statement node");
            return returnStmtNode;
        }

        public override BlockStmt CreateBlockStmt(SymbolTable<string, object> symbolTable)
        {
            BlockStmt blockStmtNode = base.CreateBlockStmt(symbolTable);
            Console.WriteLine("Creating a block statement");
            return blockStmtNode;
        }
    }
}