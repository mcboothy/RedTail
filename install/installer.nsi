# This installs two files, app.exe and logo.ico, creates a start menu shortcut, builds an uninstaller, and
# adds uninstall information to the registry for Add/Remove Programs
 
# To get started, put this script into a folder with the two files (app.exe, logo.ico, and license.rtf -
# You'll have to create these yourself) and run makensis on it
 
# If you change the names "app.exe", "logo.ico", or "license.rtf" you should do a search and replace - they
# show up in a few places.
# All the other settings can be tweaked by editing the !defines at the top of this script
!define APPNAME "RedTail-Console"
!define COMPANYNAME "Sandy Beach Productions"
!define DESCRIPTION "A console based program to compile RedTail executables"
!define TOOLSDIR "tools"
# These three must be integers
!define VERSIONMAJOR 1
!define VERSIONMINOR 1
!define VERSIONBUILD 1

!define GCCTOOLSPATH "$INSTDIR\${TOOLSDIR}\gnuarm\bin"

RequestExecutionLevel admin ;Require admin rights on NT6+ (When UAC is turned on)
 
InstallDir "C:\${APPNAME}"
 
# rtf or txt file - remember if it is txt, it must be in the DOS text format (\r\n)
LicenseData "license.txt"

# This will be in the installer/uninstaller's title bar
Name "${COMPANYNAME} - ${APPNAME}"
OutFile "redtail-setup.exe"
 
!include LogicLib.nsh
!include EnvVarUpdate.nsh

#!include "MUI.nsh"
#!include dotnet.nsh
!include "Sections.nsh"
 
Insttype "/CUSTOMSTRING=Custom Install"
InstType "Full Install"
InstType "Minimal Install"


# Just three pages - license agreement, install location, and installation
Page license
Page directory
Page components
Page instfiles

Section "RedTail GCC (Required)"
	SectionIn RO
	
	# RedTail GCC files here
	SetOutPath "$INSTDIR\${TOOLSDIR}"
	File /r "..\rpi\tools\gnuarm"
	WriteUninstaller "$INSTDIR\uninstall.exe"

	# Path
	${EnvVarUpdate} $0 "PATH" "P" "HKLM" "${GCCTOOLSPATH}"
	
	# Start Menu
	CreateDirectory "$SMPROGRAMS\${COMPANYNAME}"
	CreateShortCut "$SMPROGRAMS\${COMPANYNAME}\${APPNAME}.lnk" "$INSTDIR\app.exe" "" "$INSTDIR\logo.ico"
 
	# Registry information for add/remove programs
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayName" "${COMPANYNAME} - ${APPNAME} - ${DESCRIPTION}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "InstallLocation" "$\"$INSTDIR$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "Publisher" "$\"${COMPANYNAME}$\""
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "DisplayVersion" "$\"${VERSIONMAJOR}.${VERSIONMINOR}.${VERSIONBUILD}$\""
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMajor" ${VERSIONMAJOR}
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "VersionMinor" ${VERSIONMINOR}
	
	# There is no option for modifying or repairing the install
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}" "NoRepair" 1
	
SectionEnd
	
	
Section /o "Raspberry Pi Libraries and Firmware"
	SectionIn 1
	SetOutPath "$INSTDIR"
	# Raspberry Pi library files here
	File /r "..\rpi\libs"
	File /r "..\rpi\firmware"
SectionEnd

Section /o "RedTail Console Files"
	SectionIn 1
	SetOutPath $INSTDIR
	File "..\RedTail-Console\bin\Release\RedTail-Console.exe"
	File "..\RedTail-Console\bin\Release\RedTail-Console.exe.config"
	File "..\RedTail-Console\bin\Release\RedTailLib.dll"
SectionEnd	

 !macro VerifyUserIsAdmin
UserInfo::GetAccountType
pop $0
${If} $0 != "admin" ;Require admin rights on NT4+
        messageBox mb_iconstop "Administrator rights required!"
        setErrorLevel 740 ;ERROR_ELEVATION_REQUIRED
        quit
${EndIf}
!macroend
 
Function .onInit
	setShellVarContext all
	!insertmacro VerifyUserIsAdmin
FunctionEnd
 

# Uninstaller
Function un.onInit
	SetShellVarContext all
 
	#Verify the uninstaller - last chance to back out
	MessageBox MB_OKCANCEL "Permanantly remove ${APPNAME}?" IDOK next
		Abort
	next:
	!insertmacro VerifyUserIsAdmin
FunctionEnd
 
Section "uninstall"
 
	# Remove Start Menu launcher
	delete "$SMPROGRAMS\${COMPANYNAME}\${APPNAME}.lnk"
	# Try to remove the Start Menu folder - this will only happen if it is empty
	rmDir "$SMPROGRAMS\${COMPANYNAME}"
 
	# Remove files
	delete $INSTDIR\RedTail-Console.exe
 
	delete $INSTDIR\RedTail-Console.exe.config
	delete $INSTDIR\RedTailLib.dll

	RMDir /r $INSTDIR\libs
	RMDir /r $INSTDIR\firmware
	RMDir /r $INSTDIR\${TOOLSDIR}\gnuarm
	RMDir /r $INSTDIR\${TOOLSDIR}
	
	# Always delete uninstaller as the last action
	delete $INSTDIR\uninstall.exe
 
	# Try to remove the install directory - this will only happen if it is empty
	rmDir $INSTDIR
 
	# Fix up path
	${un.EnvVarUpdate} $0 "PATH" "R" "HKLM" "${GCCTOOLSPATH}"

	# Remove uninstaller information from the registry
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${COMPANYNAME} ${APPNAME}"
SectionEnd