.section .init
.globl _start
_start:
	b reset

.section .text

.space 0x200000-0x8004,0

reset:
    mov sp,#0x08000000
    bl _main

hang$: b hang$
	
	