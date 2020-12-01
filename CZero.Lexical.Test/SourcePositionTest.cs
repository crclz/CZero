using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CZero.Lexical.Test
{
    public class SourcePositionTest
    {
        [Fact]
        void NextCloumn()
        {
            var pos = new SourcePosition(5, 5).NextCloumn();
            Assert.Equal(6, pos.Column);
            Assert.Equal(5, pos.Line);
        }

        [Fact]
        void PreviousColumn()
        {
            var pos = new SourcePosition(5, 5).PreviousColumn();
            Assert.Equal(5, pos.Line);
            Assert.Equal(4, pos.Column);
        }

        [Fact]
        void StartOfNextLine()
        {
            var pos = new SourcePosition(5, 5).StartOfNextLine();
            Assert.Equal(6, pos.Line);
            Assert.Equal(0, pos.Column);
        }
    }
}
