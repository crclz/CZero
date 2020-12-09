using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CZero.Intermediate
{
    public class ExecutableWriter
    {
        private MemoryStream Stream { get; } = new MemoryStream();

        private Dictionary<string, byte> InstructionReference = new Dictionary<string, byte>();

        public ExecutableWriter()
        {
            GenerateInstructionReference();
        }

        public void WriteByte(byte b)
        {
            Stream.WriteByte(b);
        }

        public void WriteInt(int x)
        {
            var bytes = BitConverter.GetBytes(x);
            Debug.Assert(bytes.Length == 4);

            foreach (var b in bytes.Reverse())
            {
                WriteByte(b);
            }
        }

        public void WriteLong(long x)
        {
            var bytes = BitConverter.GetBytes(x);
            Debug.Assert(bytes.Length == 8);

            foreach (var b in bytes.Reverse())
            {
                WriteByte(b);
            }
        }

        public void WriteDouble(double x)
        {
            var bytes = BitConverter.GetBytes(x);
            Debug.Assert(bytes.Length == 8);

            foreach (var b in bytes.Reverse())
            {
                WriteByte(b);
            }
        }

        public void WriteOpCode(string opcode)
        {
            // get opcode byte
            Debug.Assert(InstructionReference.ContainsKey(opcode));

            var opCodeByte = InstructionReference[opcode];

            // write byte
            WriteByte(opCodeByte);

        }

        public byte[] GetData()
        {
            return Stream.ToArray();
        }

        public void GenerateInstructionReference()
        {
            var lines = Resource1.InstructionTable.Split("\n");
            foreach (var line in lines)
            {
                var match = Regex.Match(line, @"^(.{4})\s(.+?)\s");
                Debug.Assert(match.Success);

                var byteString = match.Groups[1].Value;
                var opcode = match.Groups[2].Value;

                Debug.Assert(byteString.StartsWith("0x"));
                byteString = byteString[2..];// delete 0x

                var byteData = byte.Parse(byteString, System.Globalization.NumberStyles.HexNumber);

                InstructionReference.Add(opcode, byteData);
            }
        }
    }
}
