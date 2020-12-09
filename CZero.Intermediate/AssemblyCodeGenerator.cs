using CZero.Intermediate.Builders;
using CZero.Intermediate.Instructions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace CZero.Intermediate
{
    class AssemblyCodeGenerator
    {
        public ExecutableWriter Executable { get; } = new ExecutableWriter();

        private GlobalBuilder globalBuilder { get; }

        public AssemblyCodeGenerator(GlobalBuilder globalBuilder)
        {
            this.globalBuilder = globalBuilder;

            // magic
            int magic = 0x72303b3e;
            Executable.WriteInt(magic);

            // version
            int version = 0x00000001;
            Executable.WriteInt(version);
        }

        public List<string> Generate()
        {
            //
            globalBuilder.BuildStartFunction();
            //

            var codeLines = GenerateGlobalVars();

            codeLines.Add("# ==================================== # ");

            // functions: Array<FunctionDef> {count=functionCount, ...}
            // functionCount
            Executable.WriteInt(globalBuilder.FunctionsView.Count);

            codeLines.AddRange(GenerateFunctions(globalBuilder));

            return codeLines;
        }

        public static string getString(object x)
        {
            if (!((x is int) || (x is long) || (x is double)))
                throw new ArgumentException($"x (type {x.GetType()}) is not int, long or double");
            return x.ToString();
        }

        public List<string> GenerateGlobalVars()
        {
            // globals: Array<GlobalDef>
            // write 'count'
            Executable.WriteInt(globalBuilder.GlobalVariablesView.Count);

            var code = new List<string>();
            foreach (var v in globalBuilder.GlobalVariablesView)
            {
                // write is_const: u8
                if (v.IsConstant)
                    Executable.WriteByte(1);
                else
                    Executable.WriteByte(0);

                code.Add("");
                var lined = "#";
                lined += $" {v.GlobalVariableBuilder.Id}";
                lined += $" {v.Name}";
                lined += $" const={v.IsConstant}";
                lined += $" type: {v.Type}";
                code.Add(lined);

                if (v.GlobalVariableBuilder.StringConstantValue != null)
                {
                    var stringValue = v.GlobalVariableBuilder.StringConstantValue;
                    code.Add($"# string constant: {stringValue}");

                    // value: Array<u8>, {count,data}

                    // count
                    Executable.WriteInt(stringValue.Length);

                    // data
                    for (int i = 0; i < stringValue.Length; i++)
                        Executable.WriteByte((byte)stringValue[i]);
                }
                else if (v.GlobalVariableBuilder.HasInitialValue)
                {
                    code.Add($"# initial expr:");
                    foreach (var instruction in v.GlobalVariableBuilder.LoadValueInstructions)
                    {
                        var line = "";
                        Debug.Assert(instruction.Parts[0] is string);
                        line += instruction.Parts[0];

                        foreach (object part in instruction.Parts.ToArray()[1..])
                        {
                            line += " " + getString(part);
                        }
                        code.Add(line);
                    }

                    // value: Array<u8>, {count=sizeof(long) ,data}

                    // count
                    Executable.WriteInt(sizeof(long));

                    // data
                    Executable.WriteLong(0L);
                }
                else
                {
                    code.Add($"# (no initial expr)");

                    // count
                    Executable.WriteInt(sizeof(long));
                    // data: set 0
                    Executable.WriteLong(0L);
                }

            }

            return code;
        }

        public List<string> GenerateFunctions(GlobalBuilder globalBuilder)
        {
            var code = new List<string>();

            foreach (var f in globalBuilder.FunctionsView)
            {
                // name: u32
                Executable.WriteInt(f.Builder.NameAt);

                // return_slots: u32
                if (f.ReturnType == DataType.Void)
                    Executable.WriteInt(0);
                else
                    Executable.WriteInt(1);

                // param_slots: u32
                Executable.WriteInt(f.ParamTypes.Count);

                // loc_slots: u32,
                Executable.WriteInt(f.Builder.LocalVariables.Count);

                code.Add("# ================================================== #");
                code.Add("");
                var paramList = "";
                foreach (var param in f.ParamTypes)
                    paramList += $"{param} ";
                code.Add($"._{f.Name} # {f.Builder.Id} fn {f.Name} ({paramList}) -> {f.ReturnType}");

                foreach (var x in f.Builder.Arguments)
                {
                    var id = x.LocalLocation.Id;
                    code.Add($"# {id} {x.Name} {x.Type} const: {x.IsConstant}");
                }
                code.Add("");

                var occupation = f.Builder.LocalVariables.Count;
                code.Add($"# local var occupation: {occupation}");
                code.Add("");

                // local vars
                foreach (var lvar in f.Builder.LocalVariables)
                {
                    var id = lvar.LocalLocation.Id;
                    code.Add($"# {id} {lvar.Name} {lvar.Type} const: {lvar.IsConstant}");
                }

                code.Add("");

                // Function body
                code.Add("# function body");
                code.AddRange(FunctionBody(f.Builder.Bucket));
                code.Add("");

            }

            return code;
        }

        private static void EachPart(Bucket bucket, Action<object> action)
        {
            foreach (var ins in bucket.InstructionList)
            {
                foreach (var p in ins.Parts)
                {
                    action(p);
                }
            }
        }

        public List<string> FunctionBody(Bucket bucket)
        {
            // body: Array<Instruction>, {count, ...}
            Executable.WriteInt(bucket.InstructionList.Count);

            var counter = 1;

            // Preprocess instructions, add offsets
            for (int i = 0; i < bucket.InstructionList.Count; i++)
                bucket.InstructionList[i].Offset = i;

            // Write instructions
            foreach (var instruction in bucket.InstructionList)
            {
                var opCode = (string)instruction.Parts[0];

                // Write opcode
                Executable.WriteOpCode(opCode);

                Debug.Assert(instruction.Parts.Count <= 2);

                foreach (var part in instruction.Parts.ToArray()[1..])
                {
                    var reqs2 = new[] {
                            "push", "popn", "loca", "arga", "globa",
                            "br","br.true", "br.false", "call","callname"};

                    Debug.Assert(reqs2.Contains(opCode));

                    if (part is Instruction ins)
                    {

                        // write offset
                        var offset = ins.Offset - instruction.Offset - 1;
                        Executable.WriteInt(offset);
                    }
                    else
                    {
                        // write operand
                        if (part is int intOperand)
                            Executable.WriteInt(intOperand);
                        else if (part is long longOperand)
                            Executable.WriteLong(longOperand);
                        else if (part is double doubleOperand)
                            Executable.WriteDouble(doubleOperand);
                        else
                            throw new Exception($"Bad operand type {part.GetType()}");
                    }
                }
            }

            // asm shits

            EachPart(bucket, p =>
            {
                if (p is Instruction instruction)
                {
                    instruction.Alias = ".L" + counter;
                    counter++;
                }
            });

            var code = new List<string>();

            foreach (var instruction in bucket.InstructionList)
            {
                var line = (string)instruction.Parts[0];

                foreach (var part in instruction.Parts.ToArray()[1..])
                {
                    if (part is Instruction ins)
                    {
                        Debug.Assert(ins.Alias != null);
                        line += " " + ins.Alias;
                    }
                    else
                    {
                        line += " " + getString(part);
                    }
                }

                if (instruction.Alias != null)
                    line = $"{instruction.Alias}: {line}";

                if (instruction.Comment != null)
                    line = line + " # " + instruction.Comment;

                code.Add(line);
            }

            return code;
        }

        static string PlainInstructionToString(Instruction instruction)
        {
            var line = (string)instruction.Parts[0];

            foreach (var part in instruction.Parts.ToArray()[1..])
            {
                if (part is Instruction ins)
                {
                    throw new Exception();
                }
                else
                {
                    line += " " + getString(part);
                }
            }

            return line;
        }

        public void WritePlainInstruction(Instruction instruction)
        {
            var opcode = (string)instruction.Parts[0];
            Executable.WriteOpCode(opcode);

            foreach (var part in instruction.Parts.ToArray()[1..])
            {
                if (part is Instruction ins)
                {
                    throw new ArgumentException("Not plain instruction");
                }
                else
                {
                    // write operand
                    if (part is int intOperand)
                        Executable.WriteInt(intOperand);
                    else if (part is long longOperand)
                        Executable.WriteLong(longOperand);
                    else if (part is double doubleOperand)
                        Executable.WriteDouble(doubleOperand);
                    else
                        throw new Exception($"Bad operand type {part.GetType()}");
                }
            }
        }

        //public List<string> GenerateStartFunction(GlobalBuilder globalBuilder)
        //{
        //    // TODO: header

        //    // 设置全局变量初始值
        //    var code = new List<string>();

        //    foreach (var v in globalBuilder.GlobalVariablesView)
        //    {
        //        var vb = v.GlobalVariableBuilder;

        //        if (vb.StringConstantValue != null)
        //        {
        //            // string constant is already in the data after compiling
        //            code.Add($"# {v.Name}: (.data) {vb.StringConstantValue}");
        //        }
        //        else
        //        {
        //            if (!vb.HasInitialValue)
        //            {
        //                code.Add($"# {v.Name}: no initial value");
        //            }
        //            else
        //            {
        //                code.Add($"# {v.Name}: initial value setter code:");

        //                // load-addr
        //                code.Add($"globa {vb.Id}");
        //                Executable.WriteOpCode("globa");
        //                Executable.WriteInt(vb.Id);

        //                // init-expr
        //                code.AddRange(vb.LoadValueInstructions.Select(p => PlainInstructionToString(p)));
        //                foreach (var ins in vb.LoadValueInstructions)
        //                    WritePlainInstruction(ins);

        //                // store
        //                code.Add("store.64");
        //                Executable.WriteOpCode("store.64");

        //            }
        //        }
        //    }

        //    // call main
        //    var mainFunction = globalBuilder.FunctionsView.Single(p => p.Name == "main");

        //    // Real generator shoul Check if there is main

        //    code.Add("");
        //    code.Add("call " + mainFunction.Builder.Id);

        //    return code;
        //}
    }
}
