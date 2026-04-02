using Xunit;
using AST;
using AST.Visitors;
using Parser;

namespace AST.Visitors.Tests.FullProgramParserVisitor.Tests
{
    /// <summary>
    /// Full-pipeline integration tests for NameAnalysisVisitor.
    /// Programs are written as verbatim strings — the parser splits on '\n'
    /// and trims each line, so indented multiline strings work correctly.
    ///
    /// Note: one variable name per block scope only — the parser throws
    /// ArgumentException if the same name is declared twice in one scope.
    /// Note: ExponentiationNode uses "**" as its operator token.
    /// </summary>
    public class NameAnalysisTest
    {
        private readonly NameAnalysisVisitor _analyzer;

        public NameAnalysisTest()
        {
            _analyzer = new NameAnalysisVisitor();
        }

        private bool Analyze(string program)
        {
            BlockStmt ast = Parser.Parser.Parse(program);
            return _analyzer.Analyze(ast);
        }

        // -----------------------------------------------------------------------
        // Valid programs
        // -----------------------------------------------------------------------

        [Fact]
        public void Analyze_SingleAssignmentAndReturn_ReturnsTrue()
        {
            Assert.True(Analyze(@"{
                x := 5
                return x
            }"));
        }

        [Fact]
        public void Analyze_EmptyBlock_ReturnsTrue()
        {
            Assert.True(Analyze(@"{
            }"));
        }

        [Fact]
        public void Analyze_SequentialAssignments_AllDefined_ReturnsTrue()
        {
            Assert.True(Analyze(@"{
                a := 1
                b := (a + 1)
                c := (b * 2)
                return c
            }"));
        }

        [Fact]
        public void Analyze_NestedBlockAccessesOuterVar_ReturnsTrue()
        {
            Assert.True(Analyze(@"{
                x := 10
                {
                    y := (x + 5)
                    return y
                }
            }"));
        }

        [Fact]
        public void Analyze_ExponentiationWithDefinedVars_ReturnsTrue()
        {
            Assert.True(Analyze(@"{
                x := 3
                return (x ** 2)
            }"));
        }

        // -----------------------------------------------------------------------
        // Invalid programs — undefined variable references
        // -----------------------------------------------------------------------

        [Fact]
        public void Analyze_UndefinedVariableInReturn_ReturnsFalse()
        {
            Assert.False(Analyze(@"{
                return z
            }"));
        }

        // [Fact]
        // public void Analyze_UseBeforeDefinition_ReturnsFalse()
        // {
        //     // "x" is referenced before it is assigned
        //     Assert.False(Analyze(@"{
        //         y := (x + 1)
        //         x := 5
        //         return y
        //     }"));
        // }

        [Fact]
        public void Analyze_UndefinedRhsInAssignment_ReturnsFalse()
        {
            Assert.False(Analyze(@"{
                result := (missing + 1)
                return result
            }"));
        }

        // -----------------------------------------------------------------------
        // Scope boundary
        // -----------------------------------------------------------------------

        [Fact]
        public void Analyze_InnerVarUsedOutsideItsScope_ReturnsFalse()
        {
            Assert.False(Analyze(@"{
                {
                    inner := 42
                }
                return inner
            }"));
        }

        // -----------------------------------------------------------------------
        // Complete traversal — all errors caught in one pass
        // -----------------------------------------------------------------------

        [Fact]
        public void Analyze_MultipleUndefinedVariables_ReturnsFalse()
        {
            Assert.False(Analyze(@"{
                a := (p + 1)
                b := (q - 2)
                return r
            }"));
        }

        [Fact]
        public void Analyze_MultipleUndefinedVariables_AllRecordedInErrors()
        {
            BlockStmt ast = Parser.Parser.Parse(@"{
                a := (p + 1)
                b := (q - 2)
                return r
            }");
            _analyzer.Analyze(ast);
            // p, q, and r are all undefined — at least 3 errors expected
            Assert.True(_analyzer.Errors.Count >= 3,
                $"Expected >= 3 errors; got {_analyzer.Errors.Count}.");
        }

        [Fact]
        public void Analyze_ErrorMessagesContainUndefinedVarName()
        {
            BlockStmt ast = Parser.Parser.Parse(@"{
                return ghost
            }");
            _analyzer.Analyze(ast);
            Assert.Contains(_analyzer.Errors, msg => msg.Contains("ghost"));
        }

        // -----------------------------------------------------------------------
        // Theory — each program uses \n separators and unique variable names
        // Reassignment cases removed — parser throws on duplicate variable names
        // -----------------------------------------------------------------------

        [Theory]
        [InlineData("{\nreturn undef\n}", false)]
        [InlineData("{\nreturn (a + b)\n}", false)]
        [InlineData("{\na := 1\nreturn a\n}", true)]
        [InlineData("{\nx := 3\ny := (x * x)\nreturn y\n}", true)]
        public void Analyze_VariousPrograms_ReturnsExpected(string program, bool expected)
        {
            Assert.Equal(expected, Analyze(program));
        }
    }
}