using Xunit;
using System;
using System.Linq;

// ============================================================
//  Adjust the namespace to match your project layout.
//  Assumes: Utilities.Containers.DiGraph<T>
// ============================================================

namespace DEC.Tests
{
    public class DiGraphTests
    {
        // ─────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────

        /// <summary>Returns a fresh empty DiGraph of strings.</summary>
        private static DiGraph<string> EmptyGraph() => new DiGraph<string>();

        /// <summary>
        /// Returns a graph with vertices "A", "B", "C" and edges A→B and B→C.
        /// </summary>
        private static DiGraph<string> LinearGraph()
        {
            var g = new DiGraph<string>();
            g.AddVertex("A");
            g.AddVertex("B");
            g.AddVertex("C");
            g.AddEdge("A", "B");
            g.AddEdge("B", "C");
            return g;
        }

        // ─────────────────────────────────────────────────────
        //  AddVertex
        // ─────────────────────────────────────────────────────

        [Fact]
        public void AddVertex_NewVertex_ReturnsTrue()
        {
            var g = EmptyGraph();
            Assert.True(g.AddVertex("X"));
        }

        [Fact]
        public void AddVertex_NewVertex_IncreasesVertexCount()
        {
            var g = EmptyGraph();
            g.AddVertex("X");
            Assert.Equal(1, g.VertexCount());
        }

        [Fact]
        public void AddVertex_DuplicateVertex_ReturnsFalse()
        {
            var g = EmptyGraph();
            g.AddVertex("X");
            Assert.False(g.AddVertex("X"));
        }

        [Fact]
        public void AddVertex_DuplicateVertex_DoesNotIncreaseCount()
        {
            var g = EmptyGraph();
            g.AddVertex("X");
            g.AddVertex("X");
            Assert.Equal(1, g.VertexCount());
        }

        [Theory]
        [InlineData("A")]
        [InlineData("hello")]
        [InlineData("123")]
        public void AddVertex_VariousValues_AppearsInGetVertices(string value)
        {
            var g = EmptyGraph();
            g.AddVertex(value);
            Assert.Contains(value, g.GetVertices());
        }

        // ─────────────────────────────────────────────────────
        //  AddEdge
        // ─────────────────────────────────────────────────────

        [Fact]
        public void AddEdge_ValidVertices_ReturnsTrue()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddVertex("B");
            Assert.True(g.AddEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_ValidVertices_EdgeExists()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddVertex("B");
            g.AddEdge("A", "B");
            Assert.True(g.HasEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_IsDirected_DoesNotCreateReverseEdge()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddVertex("B");
            g.AddEdge("A", "B");
            Assert.False(g.HasEdge("B", "A"));
        }

        [Fact]
        public void AddEdge_DuplicateEdge_ReturnsFalse()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddVertex("B");
            g.AddEdge("A", "B");
            Assert.False(g.AddEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_DuplicateEdge_DoesNotIncreaseEdgeCount()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddVertex("B");
            g.AddEdge("A", "B");
            g.AddEdge("A", "B");
            Assert.Equal(1, g.EdgeCount());
        }

        [Fact]
        public void AddEdge_MissingSourceVertex_ThrowsArgumentException()
        {
            var g = EmptyGraph();
            g.AddVertex("B");
            Assert.Throws<ArgumentException>(() => g.AddEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_MissingDestinationVertex_ThrowsArgumentException()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            Assert.Throws<ArgumentException>(() => g.AddEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_BothVerticesMissing_ThrowsArgumentException()
        {
            var g = EmptyGraph();
            Assert.Throws<ArgumentException>(() => g.AddEdge("A", "B"));
        }

        [Fact]
        public void AddEdge_SelfLoop_IsAllowedAndDetectable()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddEdge("A", "A");
            Assert.True(g.HasEdge("A", "A"));
        }

        [Fact]
        public void AddEdge_SelfLoop_IncrementsEdgeCount()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.AddEdge("A", "A");
            Assert.Equal(1, g.EdgeCount());
        }

        // ─────────────────────────────────────────────────────
        //  RemoveVertex
        // ─────────────────────────────────────────────────────

        [Fact]
        public void RemoveVertex_ExistingVertex_ReturnsTrue()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            Assert.True(g.RemoveVertex("A"));
        }

        [Fact]
        public void RemoveVertex_ExistingVertex_DecreasesVertexCount()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            g.RemoveVertex("A");
            Assert.Equal(0, g.VertexCount());
        }

        [Fact]
        public void RemoveVertex_NonExistentVertex_ReturnsFalse()
        {
            var g = EmptyGraph();
            Assert.False(g.RemoveVertex("Z"));
        }

        [Fact]
        public void RemoveVertex_WithEdges_RemovesOutgoingEdge()
        {
            var g = LinearGraph(); // A→B→C
            g.RemoveVertex("B");

            // B no longer exists, so HasEdge("B","C") should be false or throw
            Assert.False(g.HasEdge("C", "B")); 
        }

        [Fact]
        public void RemoveVertex_WithEdges_RemovesIncomingEdge()
        {
            var g = LinearGraph(); // A→B→C
            g.RemoveVertex("B");
            Assert.False(g.HasEdge("A", "B"));
        }

        [Fact]
        public void RemoveVertex_WithEdges_UpdatesEdgeCount()
        {
            var g = LinearGraph(); // 2 edges: A→B, B→C
            g.RemoveVertex("B");   // both edges involving B are removed
            Assert.Equal(0, g.EdgeCount());
        }

        [Fact]
        public void RemoveVertex_RemovedVertex_NoLongerInGetVertices()
        {
            var g = LinearGraph();
            g.RemoveVertex("B");
            Assert.DoesNotContain("B", g.GetVertices());
        }

        // ─────────────────────────────────────────────────────
        //  RemoveEdge
        // ─────────────────────────────────────────────────────

        [Fact]
        public void RemoveEdge_ExistingEdge_ReturnsTrue()
        {
            var g = LinearGraph();
            Assert.True(g.RemoveEdge("A", "B"));
        }

        [Fact]
        public void RemoveEdge_ExistingEdge_EdgeNoLongerExists()
        {
            var g = LinearGraph();
            g.RemoveEdge("A", "B");
            Assert.False(g.HasEdge("A", "B"));
        }

        [Fact]
        public void RemoveEdge_ExistingEdge_DecreasesEdgeCount()
        {
            var g = LinearGraph(); // 2 edges
            g.RemoveEdge("A", "B");
            Assert.Equal(1, g.EdgeCount());
        }

        [Fact]
        public void RemoveEdge_OnlyRemovesSpecifiedEdge_OtherEdgesIntact()
        {
            var g = LinearGraph(); // A→B, B→C
            g.RemoveEdge("A", "B");
            Assert.True(g.HasEdge("B", "C"));
        }

        [Fact]
        public void RemoveEdge_NonExistentEdge_ReturnsFalse()
        {
            var g = LinearGraph();
            Assert.False(g.RemoveEdge("A", "C")); // no direct A→C edge
        }

        [Fact]
        public void RemoveEdge_MissingSourceVertex_ThrowsArgumentException()
        {
            var g = EmptyGraph();
            g.AddVertex("B");
            Assert.Throws<ArgumentException>(() => g.RemoveEdge("A", "B"));
        }

        [Fact]
        public void RemoveEdge_MissingDestinationVertex_ThrowsArgumentException()
        {
            var g = EmptyGraph();
            g.AddVertex("A");
            Assert.Throws<ArgumentException>(() => g.RemoveEdge("A", "B"));
        }

        // ─────────────────────────────────────────────────────
        //  HasEdge
        // ─────────────────────────────────────────────────────

        [Theory]
        [InlineData("A", "B", true)]   // direct edge exists
        [InlineData("B", "C", true)]   // direct edge exists
        [InlineData("A", "C", false)]  // no direct edge (would require two hops)
        [InlineData("C", "A", false)]  // reverse direction
        [InlineData("B", "A", false)]  // reverse direction
        public void HasEdge_VariousCombinations_ReturnsExpected(
            string source, string dest, bool expected)
        {
            var g = LinearGraph(); // A→B→C
            Assert.Equal(expected, g.HasEdge(source, dest));
        }

        // ─────────────────────────────────────────────────────
        //  GetNeighbors
        // ─────────────────────────────────────────────────────

        [Fact]
        public void GetNeighbors_VertexWithOneNeighbor_ReturnsThatNeighbor()
        {
            var g = LinearGraph(); // A→B
            var neighbors = g.GetNeighbors("A");
            Assert.Single(neighbors);
            Assert.Contains("B", neighbors);
        }

        [Fact]
        public void GetNeighbors_VertexWithNoOutEdges_ReturnsEmptyList()
        {
            var g = LinearGraph(); // C has no outgoing edges
            Assert.Empty(g.GetNeighbors("C"));
        }

        [Fact]
        public void GetNeighbors_VertexWithMultipleNeighbors_ReturnsAll()
        {
            var g = EmptyGraph();
            g.AddVertex("A"); g.AddVertex("B"); g.AddVertex("C");
            g.AddEdge("A", "B");
            g.AddEdge("A", "C");
            var neighbors = g.GetNeighbors("A");
            Assert.Equal(2, neighbors.Count);
            Assert.Contains("B", neighbors);
            Assert.Contains("C", neighbors);
        }

        [Fact]
        public void GetNeighbors_DoesNotReturnIncomingNeighbors()
        {
            var g = LinearGraph(); // A→B→C, so B's only out-neighbor is C, not A
            var neighbors = g.GetNeighbors("B");
            Assert.DoesNotContain("A", neighbors);
        }

        [Fact]
        public void GetNeighbors_NonExistentVertex_ThrowsArgumentException()
        {
            var g = EmptyGraph();
            Assert.Throws<ArgumentException>(() => g.GetNeighbors("Z"));
        }

        // ─────────────────────────────────────────────────────
        //  GetVertices
        // ─────────────────────────────────────────────────────

        [Fact]
        public void GetVertices_EmptyGraph_ReturnsEmpty()
        {
            Assert.Empty(EmptyGraph().GetVertices());
        }

        [Fact]
        public void GetVertices_AfterAddingVertices_ContainsAll()
        {
            var g = LinearGraph(); // A, B, C
            var vertices = g.GetVertices().ToList();
            Assert.Equal(3, vertices.Count);
            Assert.Contains("A", vertices);
            Assert.Contains("B", vertices);
            Assert.Contains("C", vertices);
        }

        [Fact]
        public void GetVertices_AfterRemovingVertex_DoesNotContainIt()
        {
            var g = LinearGraph();
            g.RemoveVertex("B");
            Assert.DoesNotContain("B", g.GetVertices());
        }

        // ─────────────────────────────────────────────────────
        //  VertexCount
        // ─────────────────────────────────────────────────────

        [Fact]
        public void VertexCount_EmptyGraph_IsZero()
        {
            Assert.Equal(0, EmptyGraph().VertexCount());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(10)]
        public void VertexCount_AfterAddingNVertices_EqualsN(int n)
        {
            var g = EmptyGraph();
            for (int i = 0; i < n; i++)
                g.AddVertex(i.ToString());
            Assert.Equal(n, g.VertexCount());
        }

        // ─────────────────────────────────────────────────────
        //  EdgeCount
        // ─────────────────────────────────────────────────────

        [Fact]
        public void EdgeCount_EmptyGraph_IsZero()
        {
            Assert.Equal(0, EmptyGraph().EdgeCount());
        }

        [Fact]
        public void EdgeCount_LinearGraph_IsTwo()
        {
            Assert.Equal(2, LinearGraph().EdgeCount());
        }

        [Fact]
        public void EdgeCount_AfterRemovingEdge_DecreasesByOne()
        {
            var g = LinearGraph();
            g.RemoveEdge("A", "B");
            Assert.Equal(1, g.EdgeCount());
        }

        [Fact]
        public void EdgeCount_AfterRemovingAllEdges_IsZero()
        {
            var g = LinearGraph();
            g.RemoveEdge("A", "B");
            g.RemoveEdge("B", "C");
            Assert.Equal(0, g.EdgeCount());
        }

        // ─────────────────────────────────────────────────────
        //  ToString
        // ─────────────────────────────────────────────────────

        [Fact]
        public void ToString_EmptyGraph_ReturnsNonNullString()
        {
            Assert.NotNull(EmptyGraph().ToString());
        }

        [Fact]
        public void ToString_ContainsAllVertexNames()
        {
            var g = LinearGraph();
            string s = g.ToString()!;
            Assert.Contains("A", s);
            Assert.Contains("B", s);
            Assert.Contains("C", s);
        }

        // ─────────────────────────────────────────────────────
        //  Generic type — int
        // ─────────────────────────────────────────────────────

        [Fact]
        public void DiGraph_IntType_AddAndDetectEdge()
        {
            var g = new DiGraph<int>();
            g.AddVertex(1);
            g.AddVertex(2);
            g.AddEdge(1, 2);
            Assert.True(g.HasEdge(1, 2));
            Assert.False(g.HasEdge(2, 1));
        }

        [Fact]
        public void DiGraph_IntType_DuplicateVertex_ReturnsFalse()
        {
            var g = new DiGraph<int>();
            g.AddVertex(42);
            Assert.False(g.AddVertex(42));
        }
    }
}
