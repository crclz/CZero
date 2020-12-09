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
        public static List<string> Generate(GlobalBuilder globalBuilder)
        {
            var codeLines = GenerateGlobalVars(globalBuilder);

            codeLines.Add("");
            codeLines.Add("");
            codeLines.Add("__START__");
            codeLines.AddRange(GenerateStartFunction(globalBuilder));

            codeLines.AddRange(GenerateFunctions(globalBuilder));

            return codeLines;
        }

        public static string getString(object x)
        {
            if (!((x is int) || (x is long) || (x is double)))
                throw new ArgumentException($"x (type {x.GetType()}) is not int, long or double");
            return x.ToString();
        }

        public static List<string> GenerateGlobalVars(GlobalBuilder globalBuilder)
        {
            var code = new List<string>();
            foreach (var v in globalBuilder.GlobalVariablesView)
            {
                code.Add("");
                code.Add("");
                code.Add($"# id: {v.GlobalVariableBuilder.Id}");
                code.Add($"# name: {v.Name}");
                code.Add($"# const: {v.IsConstant}");
                code.Add($"# type: {v.Type}");

                if (v.GlobalVariableBuilder.StringConstantValue != null)
                {
                    code.Add($"# string constant: {v.GlobalVariableBuilder.StringConstantValue}");
                }
                else if (v.GlobalVariableBuilder.HasInitialValue)
                {
                    code.Add($"# initial:");
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
                }
                else
                {
                    code.Add($"# not initial value");
                }

            }

            return code;
        }

        public static List<string> GenerateFunctions(GlobalBuilder globalBuilder)
        {
            var code = new List<string>();

            foreach (var f in globalBuilder.FunctionsView.Where(p => p.Name != "_start"))
            {
                var paramList = "";
                foreach (var param in f.ParamTypes)
                    paramList += $"{param} ";
                code.Add($"# fn {f.Name} ({paramList}) -> {f.ReturnType}");

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
                code.AddRange(FunctionBody(f.Builder.Bucket));
                code.Add("");
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

        public static List<string> FunctionBody(Bucket bucket)
        {
            var counter = 1;

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

        public static List<string> GenerateStartFunction(GlobalBuilder globalBuilder)
        {
            // 设置全局变量初始值
            var code = new List<string>();

            foreach (var v in globalBuilder.GlobalVariablesView)
            {
                var vb = v.GlobalVariableBuilder;

                if (vb.StringConstantValue != null)
                {
                    // string constant is already in the data after compiling
                    code.Add($"# {v.Name}: (.data) {vb.StringConstantValue}");
                }
                else
                {
                    if (!vb.HasInitialValue)
                    {
                        code.Add($"# {v.Name}: no initial value");
                    }
                    else
                    {
                        code.Add($"# {v.Name}: initial value setter code:");

                        // load-addr
                        code.Add($"globa {vb.Id}");

                        // init-expr
                        code.AddRange(vb.LoadValueInstructions.Select(p => PlainInstructionToString(p)));

                        // store
                        code.Add("store.64");
                    }
                }
            }

            // call main
            var mainFunction = globalBuilder.FunctionsView.Single(p => p.Name == "main");

            // Real generator shoul Check if there is main

            code.Add("");
            code.Add("call " + mainFunction.Builder.Id);

            return code;
        }
    }
}
