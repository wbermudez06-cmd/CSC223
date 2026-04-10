using System.IO.Pipelines;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Tokenizer;

namespace AST
{
    public interface IVisitor<TParam, TResult>
    {
        // Expression nodes
        TResult Visit(PlusNode node, TParam param);
        TResult Visit(MinusNode node, TParam param);
        TResult Visit(TimesNode node, TParam param);
        TResult Visit(FloatDivNode node, TParam param);
        TResult Visit(IntDivNode node, TParam param);
        TResult Visit(ModulusNode node, TParam param);
        TResult Visit(ExponentiationNode node, TParam param);
        TResult Visit(LiteralNode node, TParam param);
        TResult Visit(VariableNode node, TParam param);

        // Statement nodes
        TResult Visit(AssignmentStmt node, TParam param);
        TResult Visit(ReturnStmt node, TParam param);
        TResult Visit(BlockStmt node, TParam param);
    }

    // implemented statements
    public abstract class Statement
    {
        public abstract TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param);
        
        public abstract string Unparse(int level = 0);
    }

    // need to convert this to a sysmbol table of blocks
    public class BlockStmt : Statement
    {
        public List<Statement> _statements;
        public SymbolTable<string, object> SymbolTable { get; }
        public BlockStmt(SymbolTable<string, object> statements)
        {
            SymbolTable = statements;
            _statements = new List<Statement>();
        }
        public void AddState(Statement s)
        {
            _statements.Add(s);
        }

        public override string Unparse(int level = 0)
        {
            string indent = new string(' ', level * 4);

            if (_statements.Count == 0)
                return $"{indent}{{" + "\n" + $"{indent}}}";

            string body = string.Join("\n", _statements.Select(s => s.Unparse(level + 1)));
            return $"{indent}{{" + "\n" + body + "\n" + $"{indent}}}";
        }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            throw new NotImplementedException();
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
        public override string Unparse(int level = 0)
        {
            string indent = new string(' ', level * 4);
            return $"{indent}{_var.Unparse()} := {_expr.Unparse()}";
        }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class ReturnStmt : Statement
    {
        public ExpressionNode _expr;
        public ReturnStmt(ExpressionNode expr)
        {
            _expr = expr;
        }

        public override string Unparse(int level = 0)
        {
            string indent = new string(' ', level * 4);
            return $"{indent}return {_expr.Unparse()}";
        }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
           return visitor.Visit(this, param);
        }
    }


    // Implemented AST node methods
    public abstract class ExpressionNode
    {
        public abstract TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param);

        public abstract string Unparse(int level = 0);
    }


    public class LiteralNode : ExpressionNode
    {
        public object? _value;
        public LiteralNode(object value)
        {
            _value = value;
        }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
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

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }

        public override string Unparse(int level = 0) => _data;

    }


    public abstract class Operator : ExpressionNode
    { }

    public abstract class BinaryOperator : Operator
    {
        public ExpressionNode? _left;
        public ExpressionNode? _right;
        protected abstract string _op { get; }
        public BinaryOperator(ExpressionNode left, ExpressionNode right)
        {
            _left = left;
            _right = right;
        }

        public override string Unparse(int level = 0) => $"({_left.Unparse()} {_op} {_right.Unparse()})";

    }

    public class PlusNode : BinaryOperator
    {
        protected override string _op => "+";
        public PlusNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class MinusNode : BinaryOperator
    {
        protected override string _op => "-";
        public MinusNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class TimesNode : BinaryOperator
    {
        protected override string _op => "*";
        public TimesNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class FloatDivNode : BinaryOperator
    {
        protected override string _op => "/";
        public FloatDivNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class IntDivNode : BinaryOperator
    {
        protected override string _op => "//";
        public IntDivNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class ModulusNode : BinaryOperator
    {
        protected override string _op => "%";
        public ModulusNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
    public class ExponentiationNode : BinaryOperator
    {
        protected override string _op => "**";
        public ExponentiationNode(ExpressionNode left, ExpressionNode right) : base(left, right) { }

        public override TResult Accept<TParam, TResult>(IVisitor<TParam, TResult> visitor, TParam param)
        {
            return visitor.Visit(this, param);
        }
    }
}
