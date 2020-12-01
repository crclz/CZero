using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace CZero.Lexical.Test
{
    public class SourceReaderTest
    {
        [Fact]
        void PeekAtStart_return_first()
        {
            var reader = new SourceReader(new StringReader("abc"));

            var c = reader.Peek();

            Assert.Equal('a', c);
        }

        [Fact]
        void PeekAfterOneNext_return_second()
        {
            var reader = new SourceReader(new StringReader("abc"));

            reader.Next();
            var c = reader.Peek();

            Assert.Equal('b', c);
        }

        [Fact]
        void PeekAtTheEOF_return_false()
        {
            var reader = new SourceReader(new StringReader("abc"));

            reader.Next();
            reader.Next();
            reader.Next();

            Assert.False(reader.TryNext(out char _));
        }

        [Fact]
        void NextAtStart_return_first()
        {
            var reader = new SourceReader(new StringReader("abc"));

            Assert.Equal('a', reader.Next());
        }

        [Fact]
        void NextSecond_return_second()
        {
            var reader = new SourceReader(new StringReader("abc"));
            reader.Next();

            Assert.Equal('b', reader.Next());
        }

        [Fact]
        void NextAtEnd_return_false()
        {
            var reader = new SourceReader(new StringReader("abc"));
            reader.Next();
            reader.Next();
            reader.Next();

            Assert.False(reader.TryNext(out char _));
        }

        [Fact]
        void NextNonWhiteChar_at_start()
        {
            var reader = new SourceReader(new StringReader(" \t \r abc"));

            Assert.True(reader.NextNonWhiteChar(out char c));
            Assert.Equal('a', c);
        }

        [Fact]
        void NextNonWhiteChar_returns_false_when_no_more_nonwhite()
        {
            var reader = new SourceReader(new StringReader("   \t  \r  \n  \r  "));

            Assert.False(reader.NextNonWhiteChar(out char _));
        }

        [Fact]
        void Position_points_at_0_0_when_begin()
        {
            var reader = new SourceReader(new StringReader("   \t  \r  \n  \r  "));

            Assert.Equal(0, reader.Position.Line);
            Assert.Equal(0, reader.Position.Column);
        }

        [Fact]
        void Position_column_increases_when_next()
        {
            var reader = new SourceReader(new StringReader("abc"));

            reader.Next();

            Assert.Equal(0, reader.Position.Line);
            Assert.Equal(1, reader.Position.Column);
        }

        [Fact]
        void Position_column_set_0_and_line_incr_1()
        {
            var source = "ab\nasd";
            var reader = new SourceReader(new StringReader(source));

            reader.Next();// a
            reader.Next();// b
            reader.Next();// This reads \n

            Assert.Equal(0, reader.Position.Column);
            Assert.Equal(1, reader.Position.Line);
        }

        [Fact]
        void PrevioutPositionTest()
        {
            var reader = new SourceReader(new StringReader("io"));

            reader.Next();

            Assert.Equal(0, reader.PreviousPosition.Column);
            Assert.Equal(0, reader.PreviousPosition.Line);
        }
    }
}
