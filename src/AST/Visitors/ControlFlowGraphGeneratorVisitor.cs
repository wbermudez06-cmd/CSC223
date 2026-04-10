// The ControlFlowGraphGeneratorVisitor traverses the AST to build a CFG, implementing the
// Visitor pattern we established in the previous assignment. This visitor serves as the bridge 
// between the syntactic structure of the program (AST) and its operational execution flow (CFG), 
// translating one representation into the other through a systematic traversal process.

// The visitor maintains a CFG and tracks the flow of control through the program, ensuring 
// statements are connected in the order they would execute at runtime. It is advised that, while 
// the AST is traversed, we pass and return Statement objects as TParam and TResult of the IVisitor 
// interface. Thus, we can create a linked chain that naturally connects sequential statements, 
// accurately modeling how code would execute at runtime.

// Observe that our CFG should only consist of AssignmentStmt and ReturnStmt objects because
// BlockStmt objects are syntactic units that ‘become folded’ into the program execution structure
// represented by the CFG.


using AST;
using Optimizer;

public class ControlFlowGraphGeneratorVisitor : IVisitor<Statement, Statement>
{
    private CFG _controlflowgraph = new CFG();

    public CFG MakeNewCFG(Statement ast)
    {
        _controlflowgraph = new CFG();
        //Start with null since there is no statement before the first
        ast.Accept(this, null);
        return _controlflowgraph;
    }

    public Statement Visit(PlusNode node, Statement param) => param;

    public Statement Visit(MinusNode node, Statement param) => param;

    public Statement Visit(TimesNode node, Statement param) => param;

    public Statement Visit(FloatDivNode node, Statement param) => param;

    public Statement Visit(IntDivNode node, Statement param) => param;

    public Statement Visit(ModulusNode node, Statement param) => param;

    public Statement Visit(ExponentiationNode node, Statement param) => param;

    public Statement Visit(LiteralNode node, Statement param) => param;

    public Statement Visit(VariableNode node, Statement param) => param;

    //AssignmentStmt and ReturnStmt ar the only ones added as vertices
    public Statement Visit(AssignmentStmt node, Statement param)
    {
        //Add statement as a vertex in the CFG
        _controlflowgraph.AddVertex(node);

        //If first statement, becomes the start of the CFG
        if (_controlflowgraph.Start == null)
        {
            _controlflowgraph.Start = node;
        }

        //If not first statement, add edge from previous node to current node
        if (param != null)
        {
            _controlflowgraph.AddEdge(param, node);
        }

        return node;
    }

    //AssignmentStmt and ReturnStmt ar the only ones added as vertices
    public Statement Visit(ReturnStmt node, Statement param)
    {

        _controlflowgraph.AddVertex(node);

        if (_controlflowgraph.Start == null)
        {
            _controlflowgraph.Start = node;
        }

        if (param != null)
        {
            _controlflowgraph.AddEdge(param, node);
        }

        return node;
    }

    public Statement Visit(BlockStmt node, Statement param)
    {
        
        Statement last = param;

        foreach (Statement stmt in node._statements)
        {
            
            last = stmt.Accept(this, last);
        }

        
        return last;
    }
}