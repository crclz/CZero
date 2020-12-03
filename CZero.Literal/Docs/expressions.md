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