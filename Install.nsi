; 该脚本使用 HM VNISEdit 脚本编辑器向导产生
BrandingText "SkyViewer V1.0"
; 安装程序初始定义常量
!define PRODUCT_NAME "SkyViewer"
!define PRODUCT_VERSION "V1.0"
!define PRODUCT_PUBLISHER "Skyworth"
!define PRODUCT_WEB_SITE "http://www.skyworthled.com"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"
!define SERVICE_NAME "SkyViewerService"

SetCompressor lzma

; ------ MUI 现代界面定义 (1.67 版本以上兼容) ------
!include "MUI2.nsh"
!include "WordFunc.nsh"
Unicode true

; MUI 预定义常量
!define MUI_ABORTWARNING
!define MUI_ICON "Resources\app.ico"
!define MUI_UNICON "Resources\icon_uninstall.ico"

; 语言选择窗口常量设置
!define MUI_LANGDLL_REGISTRY_ROOT "${PRODUCT_UNINST_ROOT_KEY}"
!define MUI_LANGDLL_REGISTRY_KEY "${PRODUCT_UNINST_KEY}"
!define MUI_LANGDLL_REGISTRY_VALUENAME "NSIS:Language"

; 欢迎页面
!insertmacro MUI_PAGE_WELCOME

!insertmacro MUI_PAGE_DIRECTORY
; 安装过程页面
!insertmacro MUI_PAGE_INSTFILES
; 安装完成页面
!insertmacro MUI_PAGE_FINISH

; 安装卸载过程页面
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "SimpChinese"

; 安装预释放文件
!insertmacro MUI_RESERVEFILE_LANGDLL
;!insertmacro MUI_RESERVEFILE_INSTALLOPTIONS
; ------ MUI 现代界面定义结束 ------

Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"
OutFile "${PRODUCT_NAME} ${PRODUCT_VERSION}.exe"
InstallDir "$PROGRAMFILES\${PRODUCT_NAME}"
ShowInstDetails show
ShowUnInstDetails show
Caption "$(^Name)"
XPStyle on

RequestExecutionLevel admin

Var appCacheDirectory

Section ""
  ReadEnvStr $appCacheDirectory SYSTEMDRIVE
  StrCpy $appCacheDirectory "$appCacheDirectory\Users\Public\Documents\${PRODUCT_NAME}"
  
  ReadRegStr $0 ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName"
  IfErrors 0 +2
    Goto done
  StrCmp $0 "" 0 +2
    Goto done

  IntCmp $LANGUAGE 1033 is1033 lessthan1033 morethan1033
  is1033:
    MessageBox MB_ICONINFORMATION|MB_OK " $0 already installed，Please reinstall after uninstall！"
    Abort
  
  lessthan1033:
  morethan1033:
    MessageBox MB_ICONINFORMATION|MB_OK " $0 已安装，请先卸载后重新安装！"
    Abort
	
  done:
SectionEnd

Section "${PRODUCT_NAME} ${PRODUCT_VERSION}" SEC01
  nsExec::Exec 'net stop ${SERVICE_NAME}'
  
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  File /r "Output\*.*"
  
  CreateDirectory "$SMPROGRAMS\${PRODUCT_NAME}"
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME} V1.0.lnk" "$INSTDIR\${PRODUCT_NAME}.exe"
  CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_NAME}.exe"  

  StrCpy $2 "False"
  StrCpy $0 0
  
Netloop:
  ClearErrors
  EnumRegKey $1 HKLM "Software\Microsoft\NET Framework Setup\NDP" $0
  IfErrors Endloop

  StrCmp $1 "" 0 +2
    Goto Endloop

  StrCmp $1 "v4" 0 +3
    StrCpy $2 "True"
    Goto Endloop

  StrCmp $1 "v4.0" 0 +3
    StrCpy $2 "True"
    Goto Endloop
  
  IntOp $0 $0 + 1
  Goto Netloop
 
Endloop:

  StrCmp $2 "False" 0 +3
    ExecWait "$INSTDIR\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"
  
  Delete "$INSTDIR\NDP452-KB2901907-x86-x64-AllOS-ENU.exe"
  
  #Pop $R0
  #System::Call "advapi32::GetUserName(t.r0,*i${NSIS_MAX_STRLEN}r1)" 
  #${WordReplace} "$DOCUMENTS" $0 "Public" "+" $R0
  
  SetOutPath $appCacheDirectory
  Delete "$appCacheDirectory\setting.ini"
  SetOverwrite ifnewer
  File /r "InstallSettingFile\setting.ini"

SectionEnd

Section "SkyViewerService" SEC02
  nsExec::Exec 'sc Create ${SERVICE_NAME} binPath= "$INSTDIR\${SERVICE_NAME}.exe" start= auto '
  nsExec::Exec 'sc start ${SERVICE_NAME}'
SectionEnd

Section -AdditionalIcons
  CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk" "$INSTDIR\uninst.exe"
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "ProductPath" "$INSTDIR"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "ProductName" "$INSTDIR\${PRODUCT_NAME}.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
SectionEnd

#-- 根据 NSIS 脚本编辑规则，所有 Function 区段必须放置在 Section 区段之后编写，以避免安装程序出现未可预知的问题。--#

Function .onInit
  !insertmacro MUI_LANGDLL_DISPLAY
FunctionEnd

/******************************
 *  以下是安装程序的卸载部分  *
 ******************************/
Section Uninstall
  nsExec::Exec 'sc stop ${SERVICE_NAME}'
  nsExec::Exec 'sc delete ${SERVICE_NAME}'
  
  Delete "$INSTDIR\${PRODUCT_NAME}.url"
  Delete "$INSTDIR\uninst.exe"

  Delete "$SMPROGRAMS\${PRODUCT_NAME}\Uninstall.lnk"
  Delete "$DESKTOP\${PRODUCT_NAME}.lnk"
  Delete "$SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME} V1.0.lnk"

  RMDir "$SMPROGRAMS\${PRODUCT_NAME}"

  RMDir /r "$INSTDIR"
  
  ReadEnvStr $appCacheDirectory SYSTEMDRIVE
  StrCpy $appCacheDirectory "$appCacheDirectory\Users\Public\Documents\${PRODUCT_NAME}"
  RMDir /r "$appCacheDirectory"
  
  DeleteRegKey ${PRODUCT_UNINST_ROOT_KEY} "${PRODUCT_UNINST_KEY}"
  SetAutoClose true
SectionEnd

#-- 根据 NSIS 脚本编辑规则，所有 Function 区段必须放置在 Section 区段之后编写，以避免安装程序出现未可预知的问题。--#

Function un.onInit
!insertmacro MUI_UNGETLANGUAGE
  IntCmp $LANGUAGE 1033 is1033 lessthan1033 morethan1033
  is1033:
  IntCmp $0 1 is1Eng lessthan1Eng morethan1Eng
    is1Eng:
      MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) is running, Please uninstall after exit！"
      Abort
      goto done
    lessthan1Eng:
    morethan1Eng:
      MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Really want to completely remove $(^Name), and all of the components？" IDYES +2
      Abort
      goto done
  
  lessthan1033:
  morethan1033:
  IntCmp $0 1 is1zh lessthan1zh morethan1zh
    is1zh:
      MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) 正在运行中，请关闭后卸载！"
      Abort
      goto done
    lessthan1zh:
    morethan1zh:
      MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "你确定要移除 $(^Name)，及其所有组件？" IDYES +2
      	Abort
      	goto done

  done:
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  IntCmp $LANGUAGE 1033 is1033 lessthan1033 morethan1033
  is1033:
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) already successfully from your computer removed。"
   goto done
   
  lessthan1033:
  morethan1033:
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) 已成功地从你的计算机移除。"
  goto done
  done:
FunctionEnd
