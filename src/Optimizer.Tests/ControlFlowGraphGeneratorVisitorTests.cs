using Xunit;
using System.Collections.Generic;
using System.Linq;
using Optimizer;
using AST;

namespace DEC.Tests
{
    public class ControlFlowGraphGeneratorVisitorTests
    {
        // ─────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────

        /// <summary>Wraps a statement list in a BlockStmt and runs the visitor.</summary>
        private static CFG BuildCFG(List<Statement> stmts)
        {
            var block = new BlockStmt(new SymbolTable<string, object>());
            foreach (var stmt in stmts)
                block.AddState(stmt);

            var visitor = new ControlFlowGraphGeneratorVisitor();
            // explicit type args required so compiler can resolve null as Statement?
            block.Accept<Statement?, Statement?>(visitor, null);
            return visitor.tracker;
        }

        private static AssignmentStmt Assign(string variable, int value) =>
            new AssignmentStmt(new VariableNode(variable), new LiteralNode(value));

        private static ReturnStmt Return(string variable) =>
            new ReturnStmt(new VariableNode(variable));

        // ─────────────────────────────────────────────────────
        //  Empty program
        // ─────────────────────────────────────────────────────

        [Fact]
        public void EmptyProgram_ProducesEmptyCFG()
        {
            var cfg = BuildCFG(new List<Statement>());
            Assert.Equal(0, cfg.VertexCount());
            Assert.Equal(0, cfg.EdgeCount());
        }

        [Fact]
        public void EmptyProgram_StartIsNull()
        {
            var cfg = BuildCFG(new List<Statement>());
            Assert.Null(cfg.Start);
        }

        // ─────────────────────────────────────────────────────
        //  Single-statement programs
        // ─────────────────────────────────────────────────────

        [Fact]
        public void SingleAssignment_OneVertexZeroEdges()
        {
            var cfg = BuildCFG(new List<Statement> { Assign("x", 5) });
            Assert.Equal(1, cfg.VertexCount());
            Assert.Equal(0, cfg.EdgeCount());
        }

        [Fact]
        public void SingleAssignment_StartIsTheAssignment()
        {
            var stmt = Assign("x", 5);
            var cfg = BuildCFG(new List<Statement> { stmt });
            Assert.Same(stmt, cfg.Start);
        }

        [Fact]
        public void SingleReturn_OneVertexZeroEdges()
        {
            var cfg = BuildCFG(new List<Statement> { Return("x") });
            Assert.Equal(1, cfg.VertexCount());
            Assert.Equal(0, cfg.EdgeCount());
        }

        [Fact]
        public void SingleReturn_StartIsTheReturn()
        {
            var ret = Return("x");
            var cfg = BuildCFG(new List<Statement> { ret });
            Assert.Same(ret, cfg.Start);
        }

        // ─────────────────────────────────────────────────────
        //  Linear programs — vertex / edge counts
        // ─────────────────────────────────────────────────────

        [Fact]
        public void FigureTwo_LinearProgram_FourVerticesThreeEdges()
        {
            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 5),
                Assign("y", 10),
                Assign("z", 15),
                Return("z")
            });
            Assert.Equal(4, cfg.VertexCount());
            Assert.Equal(3, cfg.EdgeCount());
        }

        [Fact]
        public void FigureThree_FiveStatementProgram_FiveVerticesFourEdges()
        {
            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 5),
                Assign("y", 10),
                Assign("z", 15),
                Assign("w", 30),
                Return("w")
            });
            Assert.Equal(5, cfg.VertexCount());
            Assert.Equal(4, cfg.EdgeCount());
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(6)]
        public void NAssignments_VertexCountIsN_EdgeCountIsNMinus1(int n)
        {
            var stmts = Enumerable.Range(0, n)
                .Select(i => (Statement)Assign($"v{i}", i))
                .ToList();
            var cfg = BuildCFG(stmts);
            Assert.Equal(n, cfg.VertexCount());
            Assert.Equal(n - 1, cfg.EdgeCount());
        }

        // ─────────────────────────────────────────────────────
        //  Edge ordering / directedness
        // ─────────────────────────────────────────────────────

        [Fact]
        public void ThreeStatements_EdgesConnectInOrder()
        {
            var s1 = Assign("x", 5);
            var s2 = Assign("y", 10);
            var s3 = Return("y");
            var cfg = BuildCFG(new List<Statement> { s1, s2, s3 });

            Assert.True(cfg.HasEdge(s1, s2), "Expected edge s1 → s2");
            Assert.True(cfg.HasEdge(s2, s3), "Expected edge s2 → s3");
        }

        [Fact]
        public void ThreeStatements_NoSkipEdge()
        {
            var s1 = Assign("x", 5);
            var s2 = Assign("y", 10);
            var s3 = Return("y");
            var cfg = BuildCFG(new List<Statement> { s1, s2, s3 });

            Assert.False(cfg.HasEdge(s1, s3), "s1 should not skip directly to s3");
        }

        [Fact]
        public void ThreeStatements_NoReverseEdges()
        {
            var s1 = Assign("x", 5);
            var s2 = Assign("y", 10);
            var s3 = Return("y");
            var cfg = BuildCFG(new List<Statement> { s1, s2, s3 });

            Assert.False(cfg.HasEdge(s2, s1), "Edges must not run backwards");
            Assert.False(cfg.HasEdge(s3, s2), "Edges must not run backwards");
        }

        // ─────────────────────────────────────────────────────
        //  Start vertex
        // ─────────────────────────────────────────────────────

        [Fact]
        public void Start_IsFirstStatement()
        {
            var s1 = Assign("a", 1);
            var s2 = Assign("b", 2);
            var cfg = BuildCFG(new List<Statement> { s1, s2 });
            Assert.Same(s1, cfg.Start);
        }

        [Fact]
        public void Start_IsAlwaysAssignmentOrReturn_NeverBlock()
        {
            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 1),
                Return("x")
            });
            Assert.True(cfg.Start is AssignmentStmt || cfg.Start is ReturnStmt);
        }

        // ─────────────────────────────────────────────────────
        //  Return is always a terminal node
        // ─────────────────────────────────────────────────────

        [Fact]
        public void ReturnStatement_HasNoOutgoingEdges()
        {
            var ret = Return("x");
            var cfg = BuildCFG(new List<Statement> { Assign("x", 5), ret });
            Assert.Empty(cfg.GetNeighbors(ret));
        }

        // ─────────────────────────────────────────────────────
        //  BlockStmt folding — no BlockStmt vertices in CFG
        // ─────────────────────────────────────────────────────

        [Fact]
        public void CFG_NeverContainsBlockStmtVertex()
        {
            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.AddState(Assign("y", 2));
            inner.AddState(Assign("z", 3));

            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 1),
                inner,
                Return("z")
            });

            foreach (var vertex in cfg.GetVertices())
                Assert.False(vertex is BlockStmt,
                    "BlockStmt must be folded into the CFG, not added as a vertex");
        }

        [Fact]
        public void NestedBlock_StatementsFlattenedCorrectly_FourVertices()
        {
            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.AddState(Assign("y", 2));
            inner.AddState(Assign("z", 3));

            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 1),
                inner,
                Return("z")
            });

            Assert.Equal(4, cfg.VertexCount());
            Assert.Equal(3, cfg.EdgeCount());
        }

        [Fact]
        public void NestedBlock_EdgesCrossBlockBoundaries()
        {
            // Program:  x := 1 | { y := 2 } | return y
            // s1 lives in the outer block, s2 lives in the inner block,
            // s3 lives in the outer block — edges must cross the boundary.
            var s1 = Assign("x", 1);
            var s2 = Assign("y", 2);
            var s3 = Return("y");

            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.AddState(s2);  // s2 is ONLY in the inner block

            var outer = new BlockStmt(new SymbolTable<string, object>());
            outer.AddState(s1);
            outer.AddState(inner);
            outer.AddState(s3);

            var visitor = new ControlFlowGraphGeneratorVisitor();
            outer.Accept<Statement?, Statement?>(visitor, null);
            var cfg = visitor.tracker;

            Assert.True(cfg.HasEdge(s1, s2), "Edge should cross into inner block");
            Assert.True(cfg.HasEdge(s2, s3), "Edge should exit inner block");
        }

        [Fact]
        public void DeeplyNestedBlocks_FlattenedCorrectly()
        {
            var s1 = Assign("x", 1);
            var s2 = Return("x");

            var level3 = new BlockStmt(new SymbolTable<string, object>());
            level3.AddState(s1);
            level3.AddState(s2);

            var level2 = new BlockStmt(new SymbolTable<string, object>());
            level2.AddState(level3);

            var level1 = new BlockStmt(new SymbolTable<string, object>());
            level1.AddState(level2);

            var visitor = new ControlFlowGraphGeneratorVisitor();
            level1.Accept<Statement?, Statement?>(visitor, null);
            var cfg = visitor.tracker;

            Assert.Equal(2, cfg.VertexCount());
            Assert.True(cfg.HasEdge(s1, s2));
        }

        // ─────────────────────────────────────────────────────
        //  Reachability — every vertex reachable from Start
        // ─────────────────────────────────────────────────────

        [Fact]
        public void AllVertices_ReachableFromStart_LinearProgram()
        {
            var cfg = BuildCFG(new List<Statement>
            {
                Assign("a", 1),
                Assign("b", 2),
                Assign("c", 3),
                Return("c")
            });

            var visited = BFS(cfg);
            Assert.Equal(cfg.VertexCount(), visited.Count);
        }

        [Fact]
        public void AllVertices_ReachableFromStart_WithNestedBlock()
        {
            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.AddState(Assign("y", 10));
            inner.AddState(Assign("z", 20));

            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 5),
                inner,
                Return("z")
            });

            var visited = BFS(cfg);
            Assert.Equal(cfg.VertexCount(), visited.Count);
        }

        // ─────────────────────────────────────────────────────
        //  Only AssignmentStmt and ReturnStmt in CFG
        // ─────────────────────────────────────────────────────

        [Fact]
        public void AllVertices_AreAssignmentOrReturn()
        {
            var inner = new BlockStmt(new SymbolTable<string, object>());
            inner.AddState(Assign("y", 2));

            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 1),
                inner,
                Return("y")
            });

            foreach (var vertex in cfg.GetVertices())
                Assert.True(vertex is AssignmentStmt || vertex is ReturnStmt,
                    $"Unexpected vertex type: {vertex.GetType().Name}");
        }

        // ─────────────────────────────────────────────────────
        //  Graph has no parallel edges
        // ─────────────────────────────────────────────────────

        [Fact]
        public void GeneratedCFG_HasNoParallelEdges()
        {
            var s1 = Assign("a", 0);
            var s2 = Return("a");
            var cfg = BuildCFG(new List<Statement> { s1, s2 });

            Assert.False(cfg.AddEdge(s1, s2),
                "The CFG must not have parallel (duplicate) edges");
        }

        // ─────────────────────────────────────────────────────
        //  Two independent visitor runs produce independent graphs
        // ─────────────────────────────────────────────────────

        [Fact]
        public void TwoVisitorRuns_ProduceIndependentGraphInstances()
        {
            var stmts = new BlockStmt(new SymbolTable<string, object>());
            stmts.AddState(Assign("x", 1));
            stmts.AddState(Return("x"));

            var v1 = new ControlFlowGraphGeneratorVisitor();
            stmts.Accept<Statement?, Statement?>(v1, null);

            var v2 = new ControlFlowGraphGeneratorVisitor();
            stmts.Accept<Statement?, Statement?>(v2, null);

            Assert.NotSame(v1.tracker, v2.tracker);
        }

        [Fact]
        public void TwoVisitorRuns_ProduceEquivalentStructure()
        {
            var stmts = new BlockStmt(new SymbolTable<string, object>());
            stmts.AddState(Assign("x", 1));
            stmts.AddState(Return("x"));

            var v1 = new ControlFlowGraphGeneratorVisitor();
            stmts.Accept<Statement?, Statement?>(v1, null);

            var v2 = new ControlFlowGraphGeneratorVisitor();
            stmts.Accept<Statement?, Statement?>(v2, null);

            Assert.Equal(v1.tracker.VertexCount(), v2.tracker.VertexCount());
            Assert.Equal(v1.tracker.EdgeCount(), v2.tracker.EdgeCount());
        }

        // ─────────────────────────────────────────────────────
        //  Full constant-propagation scenario (spec Figure 3)
        // ─────────────────────────────────────────────────────

        [Fact]
        public void ConstantPropagationExample_CFGStructureIsLinear()
        {
            var s1 = Assign("x", 5);
            var s2 = Assign("y", 10);
            var s3 = Assign("z", 15);
            var s4 = Assign("w", 30);
            var s5 = Return("w");

            var cfg = BuildCFG(new List<Statement> { s1, s2, s3, s4, s5 });

            Assert.Same(s1, cfg.Start);
            Assert.True(cfg.HasEdge(s1, s2));
            Assert.True(cfg.HasEdge(s2, s3));
            Assert.True(cfg.HasEdge(s3, s4));
            Assert.True(cfg.HasEdge(s4, s5));
            Assert.Empty(cfg.GetNeighbors(s5));
        }

        [Fact]
        public void ConstantPropagationExample_AllVerticesReachable()
        {
            var cfg = BuildCFG(new List<Statement>
            {
                Assign("x", 5),
                Assign("y", 10),
                Assign("z", 15),
                Assign("w", 30),
                Return("w")
            });

            var visited = BFS(cfg);
            Assert.Equal(5, visited.Count);
        }

        // ─────────────────────────────────────────────────────
        //  BFS helper — walks the CFG from Start
        // ─────────────────────────────────────────────────────

        private static HashSet<Statement> BFS(CFG cfg)
        {
            var visited = new HashSet<Statement>();
            if (cfg.Start is null) return visited;

            var queue = new Queue<Statement>();
            queue.Enqueue(cfg.Start);
            visited.Add(cfg.Start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in cfg.GetNeighbors(current))
                {
                    if (visited.Add(neighbor))
                        queue.Enqueue(neighbor);
                }
            }
            return visited;
        }
    }
}
