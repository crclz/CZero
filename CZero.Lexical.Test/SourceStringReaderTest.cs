using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CZero.Lexical.Test
{
    public class SourceStringReaderTest
    {
        [Fact]
        void PeekAtStart_return_first()
        {
            var reader = new SourceStringReader("abc");

            var c = reader.CurrentChar();

            Assert.Equal('a', c);
        }

        [Fact]
        void PeekAfterOneNext_return_second()
        {
            var reader = new SourceStringReader("abc");

            reader.Advance();
            var c = reader.CurrentChar();

            Assert.Equal('b', c);
        }

        [Fact]
        void PeekAtTheEOF_throws()
        {
            var reader = new SourceStringReader("abc");

            reader.Advance();
            reader.Advance();
            reader.Advance();

            Assert.Throws<InvalidOperationException>(() => reader.CurrentChar());
        }

        [Fact]
        void NextAtStart_return_first()
        {
            var reader = new SourceStringReader("abc");

            Assert.Equal('a', reader.CurrentChar());
        }

        [Fact]
        void NextSecond_return_second()
        {
            var reader = new SourceStringReader("abc");
            reader.Advance();

            Assert.Equal('b', reader.CurrentChar());
        }

        [Fact]
        void NextAtEnd_throws()
        {
            var reader = new SourceStringReader("abc");
            reader.Advance();
            reader.Advance();
            reader.Advance();

            Assert.Throws<InvalidOperationException>(() => reader.Advance());
        }

        [Fact]
        void NextNonWhiteChar_at_start()
        {
            var reader = new SourceStringReader(" \t \r abc");

            var success = reader.AdvanceUntilNonWhite();
            Assert.True(success);

            Assert.Equal('a', reader.CurrentChar());
        }

        [Fact]
        void NextNonWhiteChar_returns_false_when_no_more_nonwhite()
        {
            var reader = new SourceStringReader("   \t  \r  \n  \r  ");

            Assert.False(reader.AdvanceUntilNonWhite());
        }

        [Fact]
        void Position_points_at_0_0_when_begin()
        {
            var reader = new SourceStringReader("   \t  \r  \n  \r  ");

            Assert.Equal(0, reader.Position.Line);
            Assert.Equal(0, reader.Position.Column);
        }

        [Fact]
        void Position_column_increases_when_next()
        {
            var reader = new SourceStringReader("abc");

            reader.Advance();

            Assert.Equal(0, reader.Position.Line);
            Assert.Equal(1, reader.Position.Column);
        }

        [Fact]
        void Position_column_set_0_and_line_incr_1()
        {
            var source = "ab\nasd";
            var reader = new SourceStringReader(source);

            reader.Advance();// a
            reader.Advance();// b
            reader.Advance();// This reads \n

            Assert.Equal(0, reader.Position.Column);
            Assert.Equal(1, reader.Position.Line);
        }

        [Fact]
        void PrevioutPositionTest()
        {
            var reader = new SourceStringReader("io");

            reader.Advance();

            Assert.Equal(0, reader.PreviousPosition.Column);
            Assert.Equal(0, reader.PreviousPosition.Line);
        }
    }
}
