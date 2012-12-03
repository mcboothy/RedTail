/******************************************************************************
*	kernel.ld
*	 by Alex Chadwick
*	 modified by Scott McCain
*
*	A linker script for generation of raspberry pi kernel images.
*   Modifications for allowing a bootloader.
******************************************************************************/

MEMORY
{
	ram : ORIGIN = 0x8000, LENGTH = 0x1000000
}

SECTIONS {
	/*
	* First and formost we need the .init section, containing the IVT.
	*/
	.init 0x0000 : {
		*(.init)
	}
	
	/* 
	* We allow room for the ATAGs and the stack and then start our code at
	* 0x8000.
	*/
	.text 0x8000 : {
		*(.text)
	} > ram
	
	/* 
	* Next we put the data.
	*/
	.data : {
		*(.data)
	} > ram
	
	.bss : { *(.bss)
	} > ram

	/*
	* Finally comes everything else. A fun trick here is to put all other 
	* sections into this section, which will be discarded by default.
	*/
	/DISCARD/ : {
		*(*)
	}
}