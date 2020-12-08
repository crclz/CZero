# while statement

```go
while cond {
    // while-block
}
```

```s
.START: nop
cond-expr;
jump(if false) to done

# while-block

jmp .START
.DONE:nop
```