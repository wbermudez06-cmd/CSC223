using System.IO.Pipelines;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Tokenizer;

// implemented statements
public abstract class Statement
{
    public abstract string Unparse(int level = 0);
}

// need to convert this to a sysmbol table of blocks
public class BlockStmt : Statement
{
    public List<Statement> _statements;
    public BlockStmt(SymbolTable<string, object> statements)
    {
        _statements = new List<Statement>();
    }
    public void AddState(Statement s)
    {
        _statements.Add(s);
    }

    public override string Unparse(int level = 0)
    {

        string indent = new string(' ', level * 4);
        string result = indent;
        foreach (Statement statement in _statements)
        {
            result += statement + "}\n" + indent;
        }
        return result;
    }
}
public class AssignmentStmt : Statement
{
    public VariableNode _var;
    public ExpressionNode _expr;
    public AssignmentStmt(VariableNode var, ExpressionNode expr)
    {
        _var = var;
        _expr = expr;
    }
    public override string Unparse(int level = 0) => $"{_var}";
}
public class ReturnStmt : Statement
{
    public ExpressionNode _expr;
    public ReturnStmt(ExpressionNode expr)
    {
        _expr = expr;
    }
    public override string Unparse(int level = 0) => ":=";
}


// Implemented AST node methods
public abstract class ExpressionNode
{
    public abstract string Unparse(int level = 0);
}

    
public class LiteralNode : ExpressionNode
{
    public object? _value;
    public LiteralNode(object value)
    {
        _value = value;
    }

    public override string Unparse(int level = 0) => _value.ToString();
}
public class VariableNode : ExpressionNode
{
    public string _data;
    public VariableNode(string data)
    {
        _data = data;
    }

    public override string Unparse(int level = 0) => _data;

}


public abstract class Operator : ExpressionNode
{  }

public abstract class BinaryOperator : Operator
{
    public ExpressionNode? _left;
    public ExpressionNode? _right;
    public BinaryOperator(ExpressionNode left, ExpressionNode right)
    {
        _left = left;
        _right = right;
    }

    public override string Unparse(int level = 0) => $"{_left.Unparse()} + {_right.Unparse()}";
    
}

public class PlusNode : BinaryOperator
{
    public PlusNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
    
}
public class MinusNode : BinaryOperator
{
    public MinusNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
}
public class TimesNode : BinaryOperator
{
    public TimesNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
}
public class FloatDivNode : BinaryOperator
{
    public FloatDivNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
}
public class IntDivNode : BinaryOperator
{
    public IntDivNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
}
public class ModulusNode : BinaryOperator
{
    public ModulusNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
}
public class ExponentiationNode : BinaryOperator
{
    public ExponentiationNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }
}



