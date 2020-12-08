## Assign Expression

IDENT = expr;

```s
load-variable-addr
value-expr
store.64
```

load-variable-addr:

1. 全局变量
2. 函数参数
3. 局部变量

## Call Expression

print(a1,a2,a3)

ret-space
load-a1 (这三个都是自动的)
load-a2
load-a3
call func id


## LiteralExpression
字符串：把全局变量号码放在栈上

