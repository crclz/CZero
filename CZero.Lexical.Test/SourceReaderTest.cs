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
    }
}
