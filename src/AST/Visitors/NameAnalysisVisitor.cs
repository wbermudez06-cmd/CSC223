using AST;

public class NameAnalysisVisitor : IVisitor<Tuple<SymbolTable<string, object>, Statement>, bool>
{
    public List<string> Errors { get; } = new List<string>();

    public bool Analyze(BlockStmt ast)
    {
        Errors.Clear();
        var param = Tuple.Create(ast.SymbolTable, (Statement)ast);
        return ast.Accept(this, param);
    }

    public bool Visit(PlusNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(MinusNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(TimesNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(FloatDivNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(IntDivNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(ModulusNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(ExponentiationNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return node._left.Accept(this, param) && node._right.Accept(this, param);
    }

    public bool Visit(LiteralNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        return true;
    }

    public bool Visit(VariableNode node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        SymbolTable<string, object> table = param.Item1;
        Statement statement = param.Item2;
        if (table.ContainsKey(node._data)) return true;

        Errors.Add($"Undefined variable {node._data} in statement {statement.Unparse()}");
        return false;
    }

    public bool Visit(AssignmentStmt node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        SymbolTable<string, object> table = param.Item1;
        var nodeInfo = Tuple.Create(table, (Statement)node);
        if (!table.ContainsKeyLocal(node._var._data))
        {
            table.Add(new KeyValuePair<string, object>(node._var._data, null));
        }
        return node._expr.Accept(this, nodeInfo);
    }

    public bool Visit(ReturnStmt node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        SymbolTable<string, object> table = param.Item1;
        var nodeInfo = Tuple.Create(table, (Statement)node);
        return node._expr.Accept(this, nodeInfo);
    }

    public bool Visit(BlockStmt node, Tuple<SymbolTable<string, object>, Statement> param)
    {
        bool checkValid = true;
        SymbolTable<string, object> currentScope = node.SymbolTable;
        foreach (Statement stmt in node._statements)
        {
            var nodeInfo = Tuple.Create(currentScope, stmt);
            checkValid = checkValid && stmt.Accept(this, nodeInfo);
        }
        return checkValid;
    }
}

