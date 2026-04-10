using AST;

namespace Optimizer
{
    /// <summary>
    /// Represents a Control Flow Graph (CFG), which is a specialized directed graph
    /// where each vertex is a <see cref="Statement"/> from the AST.
    /// A CFG models the flow of execution between statements in a program,
    /// and is commonly used in compiler optimization and analysis.
    /// </summary>
    public class CFG : DiGraph<Statement>
    {
        /// <summary>
        /// Gets or sets the starting statement (entry point) of the control flow graph.
        /// </summary>
        public Statement? Start { get; set; }
    }
}
