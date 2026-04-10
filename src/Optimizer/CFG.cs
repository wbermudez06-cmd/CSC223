using AST;

namespace Optimizer
{
    public class CFG : DiGraph<Statement>
    {
        public Statement? Start { get; set; }
    }
}

//Yes this is everything. No touch