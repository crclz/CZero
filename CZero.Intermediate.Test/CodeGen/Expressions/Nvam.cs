using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;

namespace CZero.Intermediate.Test.CodeGen.Expressions
{
    class Nvam
    {
        private List<object[]> Instructions { get; }

        private List<object> Stack { get; } = new List<object>();
        public IReadOnlyList<object> StackView => Stack.AsReadOnly();

        public int Cursor { get; private set; } = 0;

        public bool ReachedEnd => Cursor >= Instructions.Count;

        public Nvam(List<object[]> instructions)
        {
            Instructions = instructions;
        }

        private T PopStack<T>()
        {
            if (Stack[^1] is T val)
            {
                Stack.RemoveAt(Stack.Count - 1);
                return val;
            }
            throw new InvalidOperationException();
        }

        public void Next()
        {
            if (ReachedEnd)
                throw new InvalidOperationException();

            var instruction = Instructions[Cursor];

            if (!(instruction[0] is string opcode))
                throw new InvalidOperationException($"BadOpCode '{instruction[0]}'");

            if (opcode == "push")
            {
                if (!(instruction[1] is double || instruction[1] is long))
                    throw new Exception();

                Stack.Add(instruction[1]);
            }
            else if (Regex.IsMatch(opcode, "^(add)|(sub)|(mul)|(div)\\.i|f$"))
            {
                var typec = opcode[4];
                if (typec == 'i')
                {
                    long i1 = PopStack<long>();
                    long i2 = PopStack<long>();

                    switch (opcode[..3])
                    {
                        case "add":
                            Stack.Add(i1 + i2);
                            break;
                        case "sub":
                            Stack.Add(i1 - i2);
                            break;
                        case "mul":
                            Stack.Add(i1 * i2);
                            break;
                        case "div":
                            Stack.Add(i1 / i2);
                            break;
                        default: throw new Exception();
                    }
                }
                else
                {
                    var f1 = PopStack<double>();
                    var f2 = PopStack<double>();

                    switch (opcode[..3])
                    {
                        case "add":
                            Stack.Add(f1 + f2);
                            break;
                        case "sub":
                            Stack.Add(f1 - f2);
                            break;
                        case "mul":
                            Stack.Add(f1 * f2);
                            break;
                        case "div":
                            Stack.Add(f1 / f2);
                            break;
                        default: throw new Exception();
                    }
                }
            }
            else if (opcode == "itof")
            {
                var i1 = PopStack<long>();
                var f1 = (double)i1;
                Stack.Add(f1);
            }
            else if (opcode == "ftoi")
            {
                var f1 = PopStack<double>();
                var i1 = (long)f1;
                Stack.Add(i1);
            }
            else if (opcode == "not")
            {
                var x = PopStack<long>();

                if (x != 0)
                    x = 0;
                else
                    x = 1;

                Stack.Add(x);
            }
            else if (opcode == "cmp.i")
            {
                var i1 = PopStack<long>();
                var i2 = PopStack<long>();

                long delta = 0;
                if (i1 > i2) delta = 1;
                if (i1 < i2) delta = -1;

                Stack.Add(delta);
            }
            else if (opcode == "cmp.f")
            {
                var f1 = PopStack<double>();
                var f2 = PopStack<double>();

                long delta = 0;
                if (f1 > f2) delta = 1;
                if (f1 < f2) delta = 0;

                Stack.Add(delta);
            }
            else if (opcode == "neg.i")
            {
                var i1 = PopStack<long>();
                Stack.Add(-i1);
            }
            else if (opcode == "neg.f")
            {
                var f1 = PopStack<double>();
                Stack.Add(-f1);
            }
            else
                throw new Exception($"Unknown opcode {opcode}");

            Cursor++;
        }
    }
}
