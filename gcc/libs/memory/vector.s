.globl StoreLong
StoreLong:
    str r1,[r0]
    bx lr

.globl StoreWord
StoreWord:
    strh r1,[r0]
    bx lr

.globl StoreByte
StoreByte:
    strb r1,[r0]
    bx lr

.globl GetLong
GetLong:
    ldr r0,[r0]
    bx lr

.globl GETPC
GETPC:
    mov r0,lr
    bx lr

.globl BranchTo
BranchTo:
    bx r0
