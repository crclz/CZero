﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace CZero.Intermediate {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource1 {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource1() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CZero.Intermediate.Resource1", typeof(Resource1).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 0x00	nop	-	-	-	空指令
        ///0x01	push	num:u64	-	1:num	将 num 压栈
        ///0x02	pop	-	1		弹栈 1 个 slot
        ///0x03	popn	num:u32	1-num	-	弹栈 num 个 slot
        ///0x04	dup	-	1:num	1:num, 2:num	复制栈顶 slot
        ///0x0a	loca	off:u32	-	1:addr	加载 off 个 slot 处局部变量的地址
        ///0x0b	arga	off:u32	-	1:addr	加载 off 个 slot 处参数/返回值的地址
        ///0x0c	globa	n:u32	-	1:addr	加载第 n 个全局变量/常量的地址
        ///0x10	load.8	-	1:addr	1:val	从 addr 加载 8 位 value 压栈
        ///0x11	load.16	-	1:addr	1:val	从 addr 加载 16 位 value 压栈
        ///0x12	load.32	-	1:addr	1:val	从 addr 加载 32 位 value 压栈
        ///0x13	load.64	-	1:addr	1:val	从 addr 加载 64  [字符串的其余部分被截断]&quot;; 的本地化字符串。
        /// </summary>
        internal static string InstructionTable {
            get {
                return ResourceManager.GetString("InstructionTable", resourceCulture);
            }
        }
    }
}
