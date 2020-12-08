# if statement

## if
```go
if cond {
    // if-block
}
```

```s
cond-expr;
jmp(if false) to .DONE

# if-block

.DONE:nop
```

## if-else
```go
if cond {
    // if-block
} else {
    // else-block
}
```

```s
cond-expr;
jmp(if false) to .ELSE

# if-block

jmp .DONE
.ELSE: nop

# else-block

.DONE:nop

```

## if-elif

```go
if cond {

} else if cond2 {

}

// 上面这种和下面这种等价，所以cond2交给子if去处理，不用管

if cond {

} else {
    if cond2 {

    }
}

```