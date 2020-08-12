SetCompressor /SOLID /FINAL lzma

Name SimpleBackup

# General Symbol Definitions
!define REGKEY "SOFTWARE\$(^Name)"
!define VERSION 1.0.4.0
!define COMPANY VPKSoft
!define URL http://www.vpksoft.net

# MUI Symbol Definitions
!define MUI_ICON Backup.ico
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_STARTMENUPAGE_REGISTRY_ROOT HKLM
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_REGISTRY_KEY ${REGKEY}
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME StartMenuGroup
!define MUI_STARTMENUPAGE_DEFAULTFOLDER SimpleBackup
!define MUI_UNICON UnBackup.ico
!define MUI_UNFINISHPAGE_NOAUTOCLOSE
!define MUI_FINISHPAGE_RUN # this needs to be not-defined for the MUI_FINISHPAGE_RUN_FUNCTION to work..
!define MUI_FINISHPAGE_RUN_FUNCTION "RunAsCurrentUser" # The check box for a query whether to run the installed software as the current user after the installation..

# Included files
!include "x64.nsh"
!include Sections.nsh
!include MUI2.nsh
!include "InstallOptions.nsh"
!include "DotNetChecker.nsh"

# Variables
Var StartMenuGroup

# Installer pages
!insertmacro MUI_PAGE_WELCOME
Page Custom ThirdParty 
!insertmacro MUI_PAGE_LICENSE license.txt
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_STARTMENU Application $StartMenuGroup
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

# Installer languages
!insertmacro MUI_LANGUAGE English

# Installer attributes
OutFile setup_simplebackup.1.0.4.0.exe
InstallDir $PROGRAMFILES64\SimpleBackup

CRCCheck on
XPStyle on
ShowInstDetails show
BrandingText "SimpleBackup © VPKSoft 2020"
VIProductVersion 1.0.4.0
VIAddVersionKey ProductName SimpleBackup
VIAddVersionKey ProductVersion "${VERSION}"
VIAddVersionKey CompanyName "${COMPANY}"
VIAddVersionKey CompanyWebsite "${URL}"
VIAddVersionKey FileVersion "${VERSION}"
VIAddVersionKey FileDescription ""
VIAddVersionKey LegalCopyright ""
InstallDirRegKey HKLM "${REGKEY}" Path
ShowUninstDetails show

# Installer sections
Section -Main SEC0000
    SetOutPath $INSTDIR
    SetOverwrite on
	
	nsExec::Exec 'taskkill /f /im "SimpleBackup.exe"'
    
    !insertmacro CheckNetFramework 47	

    SetOutPath "$LOCALAPPDATA\SimpleBackup"
    File simplebackup_lang.sqlite
    
    SetOutPath $INSTDIR
    File /r ..\SimpleBackup\bin\Release\*.*
	
    SetOutPath $SMPROGRAMS\$StartMenuGroup
    CreateShortcut $SMPROGRAMS\$StartMenuGroup\SimpleBackup.lnk $INSTDIR\SimpleBackup.exe
    SetOutPath $SMSTARTUP
    CreateShortcut $SMSTARTUP\SimpleBackup.lnk $INSTDIR\SimpleBackup.exe "--hidden --nob"
    WriteRegStr HKLM "${REGKEY}\Components" Main 1
SectionEnd

Section -post SEC0001
    WriteRegStr HKLM "${REGKEY}" Path $INSTDIR
    SetOutPath $INSTDIR
    WriteUninstaller $INSTDIR\uninstall.exe
    !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
    SetOutPath $SMPROGRAMS\$StartMenuGroup
    CreateShortcut "$SMPROGRAMS\$StartMenuGroup\Uninstall $(^Name).lnk" $INSTDIR\uninstall.exe
    !insertmacro MUI_STARTMENU_WRITE_END
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" DisplayName "$(^Name)"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" DisplayVersion "${VERSION}"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" Publisher "${COMPANY}"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" URLInfoAbout "${URL}"
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" DisplayIcon $INSTDIR\uninstall.exe
    WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" UninstallString $INSTDIR\uninstall.exe
    WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" NoModify 1
    WriteRegDWORD HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" NoRepair 1
SectionEnd

# Macro for selecting uninstaller sections
!macro SELECT_UNSECTION SECTION_NAME UNSECTION_ID
    Push $R0
    ReadRegStr $R0 HKLM "${REGKEY}\Components" "${SECTION_NAME}"
    StrCmp $R0 1 0 next${UNSECTION_ID}
    !insertmacro SelectSection "${UNSECTION_ID}"
    GoTo done${UNSECTION_ID}
next${UNSECTION_ID}:
    !insertmacro UnselectSection "${UNSECTION_ID}"
done${UNSECTION_ID}:
    Pop $R0
!macroend

# a function to execute the installed software as non-administrator.. 
Function RunAsCurrentUser	
	ShellExecAsUser::ShellExecAsUser "" "$INSTDIR\SimpleBackup.exe"
FunctionEnd

# Uninstaller sections
Section /o -un.Main UNSEC0000
    Delete /REBOOTOK $SMSTARTUP\SimpleBackup.lnk
    Delete /REBOOTOK $SMPROGRAMS\$StartMenuGroup\SimpleBackup.lnk
	RMDir /r /REBOOTOK $INSTDIR	
	    
    Delete /REBOOTOK $LOCALAPPDATA\SimpleBackup\simplebackup_lang.sqlite
    Delete /REBOOTOK $LOCALAPPDATA\SimpleBackup\simplebackup.sqlite
    
    DeleteRegValue HKLM "${REGKEY}\Components" Main
SectionEnd

Section -un.post UNSEC0001
    DeleteRegKey HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)"
    Delete /REBOOTOK "$SMPROGRAMS\$StartMenuGroup\Uninstall $(^Name).lnk"
    Delete /REBOOTOK $INSTDIR\uninstall.exe
    DeleteRegValue HKLM "${REGKEY}" StartMenuGroup
    DeleteRegValue HKLM "${REGKEY}" Path
    DeleteRegKey /IfEmpty HKLM "${REGKEY}\Components"
    DeleteRegKey /IfEmpty HKLM "${REGKEY}"
    RmDir /REBOOTOK $SMPROGRAMS\$StartMenuGroup
    RmDir /REBOOTOK $INSTDIR
    RmDir /REBOOTOK $LOCALAPPDATA\SimpleBackup
SectionEnd

# Installer functions
Function .onInit
    InitPluginsDir
    ${If} ${RunningX64}
		StrCpy $INSTDIR $PROGRAMFILES64\SimpleBackup
    ${Else}    
		StrCpy $INSTDIR $PROGRAMFILES\SimpleBackup
    ${EndIf}
FunctionEnd

# Uninstaller functions
Function un.onInit
    ReadRegStr $INSTDIR HKLM "${REGKEY}" Path
    !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuGroup
    !insertmacro SELECT_UNSECTION Main ${UNSEC0000}
FunctionEnd

Function ThirdParty
  ReserveFile "third_party.ini"
  !insertmacro MUI_HEADER_TEXT_PAGE "Third party" "Development environment and third party components used in this software"
  !insertmacro INSTALLOPTIONS_EXTRACT "third_party.ini"
  !insertmacro INSTALLOPTIONS_DISPLAY "third_party.ini"
FunctionEnd
