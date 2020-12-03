# 表达式

- [ ] 消除左递归
- [ ] Or候选式First不相交
- [ ] LL(1)

## 表达式文法

明显有左递归

```js
expr -> 
      operator_expr /*运算符表达式*/ expr binary_operator expr
    | negate_expr /*取反表达式*/ '-' expr
    | assign_expr /*赋值表达式*/ l_expr '=' expr  (l_expr->IDENT 左值表达式)
    | as_expr /*类型转换表达式*/ expr 'as' ty (ty -> IDENT)
    | call_expr /*函数调用表达式*/ IDENT '(' call_param_list? ')'
    | literal_expr /*字面量表达式*/ UINT_LITERAL | DOUBLE_LITERAL | STRING_LITERAL
    | ident_expr /*标识符表达式*/ IDENT
    | group_expr /*括号表达式*/ '(' expr ')'

binary_operator -> '+' | '-' | '*' | '/' | '==' | '!=' | '<' | '>' | '<=' | '>='

```

## ❌ 局部-算符优先文法


> 提示：对于 运算符表达式 operator_expr 、取反表达式 negate_expr 和类型转换表达式 as_expr 可以使用局部的算符优先文法进行分析。

from https://c0.karenia.cc/c0/expr.html

分析：
1. 在调用算符优先文法的分析函数之前，需要预先确定“局部”的范围吗？答：需要。因为算符优先文法需要知道左、右边界，来放`#`。左边界是知道的，但是右边界不知道，所以问题变成了如何**确定右边界**。
2. 同样地，在算符优先文法分析函数里面，也有“局部递归下降子程序法”的需求，例如`1+3+get(1,2)*5`。由于“递归下降”是自顶向下的方法，不是需要确定边界，而是需要确定**目标非终结符**。

这两个问题我感觉不太好解决。