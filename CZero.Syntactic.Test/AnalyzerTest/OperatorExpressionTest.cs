using System;
using System.Collections.Generic;
using System.Text;

namespace CZero.Syntactic.Test.AnalyzerTest
{
    /*
    测试计划

    分为多阶段。每个阶段启用一点东西。
    在每个阶段中，先单元测试，然后集成测试。

    expr -> 
        | operator_exp
        | negate_expr   禁用，下一步就启用
        | assign_expr   禁用
        | call_expr     逐步启用，解释器设置几个专用函数调用的结果计算
        | literal_expr  
        | ident_expr    禁用
        | group_expr

    */

    public class OperatorExpressionTest
    {
    }
}
