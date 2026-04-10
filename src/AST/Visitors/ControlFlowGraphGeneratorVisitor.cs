using System.ComponentModel;
using System.Data;
using AST;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Optimizer;
using Tokenizer;

/// <summary>
/// A visitor that traverses an AST and constructs a Control Flow Graph (CFG).
/// Each statement node becomes a vertex in the CFG, and edges represent
/// the flow of execution between statements.
/// </summary>
public class ControlFlowGraphGeneratorVisitor : IVisitor<Statement?, Statement?>
{
    /// <summary>
    /// The CFG being constructed during traversal.
    /// </summary>
    public CFG tracker;

    /// <summary>
    /// Begins CFG construction from the given block statement.
    /// </summary>
    public void BeginControlFlow(BlockStmt ast)
    {
        tracker = new CFG();
        ast.Accept(this, null);
    }

    /// <summary>
    /// Visits a block of statements and processes them.
    /// Each statement is connected to the previous one to model execution order.
    /// </summary>
    public Statement Visit(BlockStmt ast, Statement prev)
    {
        // keeps track of the most recent statement in the block
        Statement lastVisited = prev;
        
        // loops through the list of statements
        foreach (Statement stmt in ast._statements)
        {
            // uses Accept method to link the control flow
            lastVisited = stmt.Accept(this, lastVisited);
        }
        // returns the most recent last visited statement
        return lastVisited;
    }

    /// <summary>
    /// Visits an assignment statement and adds it to the CFG.
    /// Connects it to the previous statement if one exists.
    /// </summary>
    public Statement Visit(AssignmentStmt stmt, Statement prev)
    {
        // add current statement as a vertex in the tracker
        tracker.AddVertex(stmt);

        // if there is no previous statement, make the current statement the starting one
        if (prev == null) tracker.Start = stmt;

        // if not, create and edge between this statement and previous one
        else tracker.AddEdge(prev, stmt);

        // return the statement
        return stmt;
    }

    public Statement Visit(ReturnStmt stmt, Statement prev)
    {
        // add current statement as a vertex in the tracker
        tracker.AddVertex(stmt);
        
        // if there is no previous statement, make the current statement the starting one
        if (prev == null) tracker.Start = stmt;

        // if not, create and edge between this statement and previous one
        else tracker.AddEdge(prev, stmt);

        // return statement
        return stmt;
    }

    /// <summary>
    /// Visits expression-level AST nodes that do not influence control flow.
    /// These nodes (e.g., arithmetic operations, literals, variables) are part of
    /// expressions rather than standalone statements, so they do not create new
    /// vertices or edges in the Control Flow Graph (CFG).
    /// As a result, each of these visit methods simply returns the previously
    /// visited statement unchanged.
    /// </summary>
    public Statement Visit(PlusNode node, Statement prev) => prev;

    public Statement Visit(MinusNode node, Statement prev) => prev;

    public Statement Visit(TimesNode node, Statement prev) => prev;

    public Statement Visit(FloatDivNode node, Statement prev) => prev;

    public Statement Visit(IntDivNode node, Statement prev) => prev;

    public Statement Visit(ModulusNode node, Statement prev) => prev;

    public Statement Visit(ExponentiationNode node, Statement prev) => prev;

    public Statement Visit(LiteralNode node, Statement prev) => prev;

    public Statement Visit(VariableNode node, Statement prev) => prev;

}
