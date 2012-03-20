;SVG to level XML 0.8 
;<Element Attribute="SubAttribute:1111;">Value</Element>

#include "Array.au3"
#include <Constants.au3>
#include <GUIConstants.au3>
#include <WindowsConstants.au3>
#include <EditConstants.au3>
#include <GDIPlus.au3>
#include <ScreenCapture.au3>
#include <GUIConstantsEx.au3>
#include <m_LogFile.au3>

;HotKeySet("^q", "Halt")


Global $EOinF = False, $inFileName, $outFileName, $inFile, $outFile

;Tag Types
Const $cTT_Start = "Tag Type: Start"
Const $cTT_Single = "Tag Type: Single"
Const $cTT_End = "Tag Type: End"

Const $iniFile = @ScriptDir & "\ini.ini"
Global $lastFile, $quickBoot, $lunohod_error, $timer

Global $ElementsDict
Global $quickBoot, $Root, $Metadata, $Content, $ArcRoot, $ResourcesRootFolder

Global $cDecimalplaces

Global $LayerTagOpen, $GroupTagOpen

; win
Global $ProgName = "SVG to level XML"
Global $MainWin, $msg, $TypeList
Global $mFile, $mExit
Global $lFilename, $bSelectFile, $bConvert, $bMakeArc, $cbToDropbox, $cbAndExec, $cbAndShow, $bErrorTxt, $bExit


Func InitParams()
	
	$LayerTagOpen = 0
	$GroupTagOpen = 0
	
	$quickBoot = IniRead($iniFile, "main", "quickBoot", "False")
	
	$ElementsDict = IniRead($iniFile, "main", "ElementsDict", "")
	$cDecimalplaces = IniRead($iniFile, "main", "Decimalplaces", 2)
	
	$ResourcesRootFolder = IniRead($iniFile, "path", "ResourcesRootFolder", "")
	$Root = IniRead($iniFile, "path", "Root", "")
	$Content = IniRead($iniFile, "path", "Content", "")
	$Metadata = IniRead($iniFile, "path", "Metadata", "")
	if ($ResourcesRootFolder = "") or ($Root = "") or ($Content = "") or ($Metadata = "") Then Halt("#001# Wrong path in the ini file")

	$lastFile = IniRead($iniFile, "main", "LastSVGFile", "")
	
	if (not FileExists($lastFile)) or (GetExt($lastFile) <> "svg") Then 
		FilesSelect()
	Else
		$inFileName = $lastFile
	EndIf
	
EndFunc

Func MainWinInit()
Local $winW = 260, $winH = 86

	$MainWin = GUICreate($ProgName, $winW, $winH, @DesktopWidth - $winW, 25, $WS_POPUPWINDOW)
	
	$bSelectFile = GUICtrlCreateButton("select file", 3, 3, 60, 22)
	$lFilename = GUICtrlCreateLabel(GetFileName($inFileName), 70, 6, $winW - 72 - 13 - 10)
	GUICtrlSetTip($lFilename, $inFileName)
	
	$bExit = GUICtrlCreateButton("X", $winW - 17, 3, 14, 14)
	
	$bConvert = GUICtrlCreateButton("convert", 3, 28, 60, 22)
	$cbAndExec = GUICtrlCreateCheckbox('exec', 70, 28, 50, 22)
	$cbAndShow = GUICtrlCreateCheckbox('show', 127, 28, 60, 22)
	$bErrorTxt = GUICtrlCreateButton("error txt", 194, 28, 60, 22)
	GUICtrlSetState($bErrorTxt, $GUI_HIDE)
	GUICtrlSetColor($bErrorTxt, 0xFF0000)
	
	$bMakeArc = GUICtrlCreateButton("make arc", 3, 52, 60, 22)
	$cbToDropbox = GUICtrlCreateCheckbox("to dropbox", 70, 52)
	GUISetState(@SW_SHOW)
	
	WinSetOnTop($MainWin, "", 1)
	
EndFunc

Func MainWinClose()
	GUIDelete($MainWin)
EndFunc


Func FilesSelect()
Local $newFile
	$newFile = FileOpenDialog("Select input file", GetPath($inFileName), "SVG Files (*.svg)", 1, GetFileName($inFileName))
	if $newFile <> "" Then 
		$inFileName = $newFile
		GUICtrlSetTip($lFilename, $inFileName)
	EndIf
EndFunc

Func FilesOpen()
	$inFile = FileOpen($inFileName, 0)
	if $inFile = -1 then Halt("#002# fail to open file: " & $inFileName)
	
	if FileExists($Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml") Then
		myFileCopy($Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", $Root & "\" & $Metadata & "\Bak\" & GetFileName($inFileName) & '~bak~' & GetRandomChar() & GetRandomChar() & GetRandomChar() & GetRandomChar() & ".xml", 9)
	EndIf
	$outFileName = $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml"
	$outFile = FileOpen($outFileName, 2)
	if $outFile = -1 then Halt("#003# fail to create file: " & $outFileName)
	$EOinF = False
EndFunc

Func myFileCopy($src, $dest, $flag)
Local $ret
	$ret = FileCopy($src, $dest, $flag)
	if $ret <> 1 Then 
		Error("Couldn't copy file: " & @CRLF & $src & @CRLF & "to" & @CRLF & $dest)
	Else
		m_LogAdd("file copied: " & @CRLF & $src & @CRLF & "to" & @CRLF & $dest)
	EndIf
	Return $ret
EndFunc

Func myFileDelete($f)
Local $ret
	$ret = FileDelete($f)
	if $ret <> 1 Then 
		Error("Couldn't delete file: " & $f)
	Else
		m_LogAdd("file deleted: " & $f)
	EndIf
	Return $ret
EndFunc


Func FilesClose()
	FileClose($inFile)
	FileClose($outFile)
EndFunc

Func GetRandomChar()
	if Random() > 0.5 Then 
		Return Random(0, 9, 1)
	Else
		Return Chr(Random(Asc("A"), Asc("Z"), 1))
	EndIf
EndFunc

Func GetImageSize($fullpath)
Local $hImage
Dim $ret[2]
	_GDIPlus_Startup ()
	$hImage = _GDIPlus_ImageLoadFromFile ($fullpath)
	$ret[0] = _GDIPlus_ImageGetWidth ($hImage)
	$ret[1] = _GDIPlus_ImageGetHeight ($hImage)
	_GDIPlus_ImageDispose ($hImage)
	_GDIPlus_ShutDown ()
	Return $ret
EndFunc

Func VerifySVG()
Local $i = 1, $name, $ErrorList = "", $teleports = ""
Local $reqList = '/tower/|/cat/|inkscape:label="action"'

	$reqArr = StringSplit($reqList, "|")
	Do
		$str = ReadStr()
		
		if GetAttr($str, "transform") <> "" then 
			$el = GetElementName($str)
			if $el = "g" then 
				$el = "layer(group)"
				$name = GetAttr($str, "inkscape:label")
				if $name = "" Then $name = GetAttr($str, "id")
			Else
				$name = "id=" & GetAttr($str, "id")
			EndIf
			$ErrorList = $ErrorList & 'Error in SVG file: ' & $el & ' "' & $name & '" has been transformed (line #' & $i & ')' & @CRLF
		EndIf
		
		$tpId = GetSubAttr($str, "inkscape:label", "TeleportId")

		if $tpId <> "" Then
			if (not StringInStr($teleports, $tpId)) Then 
				$teleports = $teleports & " " & $tpId
			Else
				$teleports = StringReplace($teleports, $tpId, "")
			EndIf
		EndIf
		
		$i = $i + 1
	Until $EOinF
	
	if not StringIsSpace($teleports) And not $teleports = "" Then $ErrorList = $ErrorList & "Teleports are not matched: " & $teleports & @CRLF
		
	if $ErrorList <> "" Then
		if MsgBox(17, 'Errors in SVG', $ErrorList & @CRLF & @CRLF & "Ignore and continue?" ) <> 1 Then Halt("Verifying")
	EndIf
	
	FileSetPos($inFile, 0, $FILE_BEGIN)
	$EOinF = False
EndFunc

Func FormatFile($FileName)
Local $offset = "", $cur, $par, $tt

	$inFile = FileOpen($FileName)
	$outFile = FileOpen($FileName & "~~~", 2)
	$EOinF = False
	$str = Read()
	Write($str, True)
	Do
		$str = Read()
		if GetTagType($str) = $cTT_End then $offset = StringReplace($offset, "    ", "", 1)
		if GetTagType($str) = $cTT_Start then $cur = "    "
		$newStr = $offset & $str
		Write($newStr, True)
		$offset = $offset & $cur
		$cur = ""
	Until $EOinF


	FileClose($inFile)
	FileClose($outFile)
	FileCopy($FileName & "~~~", $FileName, 1)
	FileDelete($FileName & "~~~")
EndFunc

Func CollectAndWriteResources($WriteRes)
Local $dir, $srcFile, $destFile
	
	if $WriteRes Then Write('<Resources RootFolder="' & $ResourcesRootFolder & '">', False)
	$textures = ""
	Do
		$str = ReadStr()
		if GetElementName($str) = "image" Then
			$fullpath = GetFullPath($str)
			if (not StringInStr($textures, "|" & $fullpath)) and (not StringInStr($fullpath, "unvisible")) Then $textures = $textures & "|" & $fullpath
			$sprite = GetSubAttr($str, "inkscape:label", "Sprite")
			$fullpath2 = ""
			if $sprite <> "" Then $fullpath2 = GetPath($fullpath) & $sprite & ".png"
			if (not StringInStr($textures, "|" & $fullpath2)) Then 
				$textures = $textures & "|" & $fullpath2	
			EndIf
		EndIf
	Until $EOinF
	$texturesArr = StringSplit($textures, "|")
	$filesNotification = ""

	if $WriteRes Then 
		$dir = $Root
	Else
		$dir = $ArcRoot
	EndIf
	if $dir = "" then Halt("#004# Have no directory for resources")
	for $i = 1 to $texturesArr[0]
		if $texturesArr[$i] <> "" Then
			$fullpath = $texturesArr[$i]
			$filename = GetFileName($fullpath)
			if $WriteRes Then Write('<Texture Id="' & $filename & '" Source="' & $filename & '" />', False)
			$destFile = $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png"
			if FileExists($fullpath) Then
				if FileExists($destFile) Then
					$iSize1 = GetImageSize($fullpath)
					$iSize2 = GetImageSize($destFile)
					;MsgBox(0, $iSize1[0], $iSize2[0])
					;MsgBox(0, $iSize1[1], $iSize2[1])
					
					if (FileGetSize($fullpath) <> FileGetSize($destFile)) or ($iSize1[0] <> $iSize2[0]) or ($iSize1[1] <> $iSize2[1]) Then
						#cs
						ConsoleWrite(@CRLF)
						ConsoleWrite("======================"& @CRLF)
						
						ConsoleWrite($fullpath & @CRLF)
						ConsoleWrite($destFile & @CRLF)
						ConsoleWrite(FileGetSize($fullpath) & @CRLF)
						ConsoleWrite(FileGetSize($destFile) & @CRLF)
						
						
						ConsoleWrite("======================"& @CRLF)
						ConsoleWrite(@CRLF)
						#ce
						$randomName = GetRandomChar() &  GetRandomChar() & GetRandomChar() & GetRandomChar()
						myFileCopy($destFile, $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\bak\" & $filename & "~bak~" & $randomName & ".png", 9)
						$filesNotification = $filesNotification & "BACKUP: " & $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\bak\" & $filename & "~bak~" & $randomName & ".png" & @CRLF
						myFileCopy($fullpath, $destFile, 9)
						$filesNotification = $filesNotification & "Copied: " & $destFile & @CRLF
						myFileDelete($dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".xnb")
						$filesNotification = $filesNotification & "XNB DELETED: " & $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".xnb"
					EndIf
				Else
					myFileCopy($fullpath, $destFile, 9)
					$filesNotification = $filesNotification & "Copied: " & $destFile & @CRLF
				EndIf
			Else
				Halt("###008### File not found : " & $fullpath)
			EndIf
		EndIf
	Next
	if $filesNotification <> "" Then MsgBox(64, "Collected", $filesNotification)					
	if $WriteRes Then Write("</Resources>", False)
	
	FileSetPos($inFile, 0, $FILE_BEGIN)
	$EOinF = False
EndFunc

;TAGS ==>>

Func GetTagType($str)
	if (StringMid($str, 1, 2) = "</") then Return $cTT_End
	if StringMid($str, StringLen($str)-1, 2) = "/>" then Return $cTT_Single
	Return $cTT_Start
EndFunc


Func GetElementName($str)
	$str = StringRegExpReplace($str, "[/<>{TAB}]", "")
	$arr = StringSplit($str, " ")
	Return $arr[1]
EndFunc


Func GetAttr($str, $attr)

	;getting special attribute
	
	if $attr = "@filename" then Return GetFileName(GetFullPath($str))
	if $attr = "@opacity" then  
		;MsgBox(0, "", GetSubAttr($str, "style", "opacity"))
		if StringInStr($str, "opacity:") Then 
			Return Round(GetSubAttr($str, "style", "opacity"), $cDecimalplaces)
		Else
			Return "1"
		EndIf
	EndIf
	if $attr = "@blockid" then Return GetAttr($str, "id") & "_block"
	if $attr = "@animationid" then Return GetAttr($str, "id") & "_animation"
	if $attr = "@stretch" Then 
		$size = GetImageSize(GetFullPath($str))
		if ($size[0] <> GetAttr($str, "width")) or ($size[1] <> GetAttr($str, "height")) Then
			;MsgBox(0, GetAttr($str, "@filename"), $iX & " - " & GetAttr($str, "width") & " - " & $iY & " - " & GetAttr($str, "height"))
			Return "true"
		Else
			Return "false"
		EndIf
		
	EndIf
	if $attr = "@CRLF" then Return @CRLF
	if $attr = "@MovingLen" Then
		if GetAttr($str, "width") > GetAttr($str, "height") Then 
			if GetFileName(GetAttr($str, "xlink:href")) = "left" then 
				$k = -1
			Else
				$k=1
			EndIf
			Return $k*Round(GetAttr($str, "width"))
		Else
			if GetFileName(GetAttr($str, "xlink:href")) = "up" then 
				$k = -1
			Else
				$k=1
			EndIf
			Return $k*Round(GetAttr($str, "height"))
		EndIf
	EndIf
	if $attr = "@Duration" Then 
		$ret = GetSubAttr($str, "inkscape:label", "Duration")
		if $ret = "" Then
			MsgBox(0, "", 'Duration is not defined in the tag <' & GetElementName($str) &'> (id="' & GetAttr($str, "id") & '"). Duration set to default (1)')
			$ret = "1"
		EndIf
		Return $ret
	EndIf
	if $attr = "@Direction" Then
		if GetAttr($str, "width") > GetAttr($str, "height") Then 
			Return "X"
		Else
			Return "Y"
		EndIf
	EndIf
	
	;getting regular attribute
	
	$arr = StringSplit($str, '"')
	if $arr[0] = 1 then Return ""
	for $i = 1 to $arr[0] - 1
		if StringInStr($arr[$i], $attr & "=") > 0 then 
			if $attr = "x" or $attr = "y" or $attr = "width" or $attr = "height" Then
				Return Round($arr[$i+1])
			else
				Return $arr[$i+1]
			EndIf
		EndIf
	next
	Return ""	
EndFunc


Func GetSubAttr($str, $attr, $subAttr)
	$attrStr = GetAttr($str, $attr)
	$arr = StringSplit($attrStr, ';')
	for $i = 0 to $arr[0]
		if StringInStr($arr[$i], $subAttr & ":") then 
			$arr2 = StringSplit($arr[$i], ':')
			if $arr2[0] = 1 then Return ""
			Return $arr2[2]
		EndIf
	next
	Return ""	
EndFunc


Func GetValue($str)
	Return StringMid($str, StringInStr($str, ">") + 1, StringInStr($str, "<", 0, 2) - 1 - StringInStr($str, ">"))
EndFunc


;<<== TAGS


;FILE:/// ==>>


Func GetFullPath($str)
	$fullpath = GetAttr($str, "xlink:href")
	$fullpath = StringReplace($fullpath, "file:///", "")
	$fullpath = StringReplace($fullpath, "/", "\")
	Return $fullpath
EndFunc


Func GetFileName($fullPath)
	if StringInStr($fullPath, "/") Then
		Halt("###005### BAD SIGNS /\/\")
	Else
		$arr = StringSplit($fullPath, "\")
	EndIf
	if $arr[0] = 1 Then Return ""
	Return StringLeft($arr[$arr[0]], StringInStr($arr[$arr[0]], ".") - 1)
EndFunc


Func GetExt($fullPath)
	if StringInStr($fullPath, "/") Then
		Halt("###006### BAD SIGNS /\/\")
	Else
		$arr = StringSplit($fullPath, "\")
	EndIf
	if $arr[0] = 1 Then Return ""
	$arr2 = StringSplit($arr[$arr[0]], ".")
	if $arr2[0] = 1 Then Return ""
	Return $arr2[$arr2[0]]
EndFunc


Func GetPath($fullPath)
	Return StringReplace($fullPath, GetFileName($fullPath) & "." & GetExt($fullPath), "")
EndFunc


Func GetParentDir($fullPath)
	if StringInStr($fullPath, "/") Then
		Halt("###007### BAD SIGNS /\/\")
	Else
		$arr = StringSplit($fullPath, "\")
	EndIf
	if $arr[0] = 1 Then Return ""
	Return $arr[$arr[0]-1]
EndFunc


;<<== FILE:///


Func PassElement($str)
	
	if (GetTagType($str) = $cTT_Single) or (GetTagType($str) = $cTT_End) Then Return

	$el = GetElementName($str)
	$elCounter = 1
	Do
		$str = Read()
		if (GetElementName($str) = $el) and (GetTagType($str) = $cTT_Start) Then $elCounter = $elCounter + 1
		if (GetElementName($str) = $el) and (GetTagType($str) = $cTT_End) Then $elCounter = $elCounter - 1	
		;MsgBox(0, $elCounter,$str)
	Until $EOinF or $elCounter = 0
EndFunc


Func ParseStr($str)
Local $maskFile = ""

	$el = GetElementName($str)
	
	;g
	if $el = "g" Then
		
		$tt = GetTagType($str)
		if $tt = $cTT_End and ($GroupTagOpen > 0) Then
			$GroupTagOpen = $GroupTagOpen - 1
			Return "</Group>"
		EndIf
		
		if $tt = $cTT_End and ($LayerTagOpen > 0) Then
			$LayerTagOpen = $LayerTagOpen - 1 
			Return "</Layer>"
		EndIf
		
		if $tt = $cTT_End Then Return "ERROR"
			
		if $tt = $cTT_Single Then Return "";delete empty layer or group
			
		if GetAttr($str, "inkscape:groupmode") = "layer" Then	;layer
			$LayerTagOpen = $LayerTagOpen + 1
			Return '<Layer Id="' & GetAttr($str, "inkscape:label") & '" BackColor="#FFFFFFFF">'
		Else	;group
			$GroupTagOpen = $GroupTagOpen + 1
			Return '<Group Id="' & GetAttr($str, "id") & '">' ; old: '" Edges="None">'
			
		EndIf
	EndIf
	;g
	
	;image
	if $el = "image" Then
		$fullpath = GetFullPath($str)
		$fn = GetFileName($fullpath)
		$ret = ""

		if GetSubAttr($str, "inkscape:label", "Sprite") <> "" Then
			$maskFile = GetPath($fullPath) & "sprite_" & GetSubAttr($str, "inkscape:label", "Sprite") & ".txt"
		EndIf
		
		if GetParentDir($fullpath) = "block" Then
			$maskFile = GetPath($fullPath) & GetFileName($fullPath) & ".txt"
		EndIf
		
		if $maskFile = "" Then
			$maskFile = GetPath($fullPath) & GetParentDir($fullpath) & ".txt"
		EndIf
		
		if not FileExists($maskFile) Then $maskFile = @ScriptDir & '\default_image_mask_file.txt'
		
		$maskFile = FileOpen($maskFile)
		
		if $maskFile <> -1 Then
			$END = False
			Do
				$line = FileReadLine($maskFile)
				$END = (@error = -1)
				$line = StringStripCR($line)
				if StringLeft($line, 3) = "###" then 
					$ret = $ret & GetAttr($str, StringMid($line, 4, StringLen($str)-3))
				Else
					$ret = $ret & $line
				EndIf
			Until $END
			FileClose($maskFile)
			Return $ret
		EndIf

	EndIf

	
	Return ""
EndFunc



Func Read()
	$str = ""
	if Not $EOinF Then 
		$str = FileReadLine($inFile)
		$EOinF = (@error = -1)
	Else
		Return ""
	EndIf
	
	Return StringStripWS($str, 7)
EndFunc


Func ReadStr()
	$str = Read()
	
	if StringInStr($ElementsDict, "|" & GetElementName($str) & "|" ) = 0 Then Return ""

	if (GetElementName($str) = "g") and (GetSubAttr($str, 'style', 'display') = 'none') Then
		PassElement($str)
		Return ""
	EndIf
	
	Return $str
EndFunc



Func Write($newStr, $andInConsole)
	if $newStr <> "" Then
		FileWriteLine($outFile, $newStr)
		if $andInConsole then ConsoleWrite($newStr & @CRLF)
	EndIf
EndFunc


Func PutLevelToGameXml()
Local $oldGameXmlFileName = $Root & "\" & $Metadata & "\Game.xml"
Local $oldGameXmlFile, $eeooff = False

	if Not FileExists($oldGameXmlFileName) Then 
		MsgBox(16, "Error", "Could not open file " & $oldGameXmlFileName)
		Exit
	EndIf
	
	$oldGameXmlFile = FileOpen($oldGameXmlFileName)
	$newGameXmlFile = FileOpen($oldGameXmlFileName & ".temp", 2)
	
	Do
		$str = FileReadLine($oldGameXmlFile)
		$eeooff = (@error = -1)
		if GetAttr($str, "Name") <> GetFileName($inFileName) Then FileWriteLine($newGameXmlFile, $str)
		if StringInStr($str, "<Levels>") Then
			$str = FileReadLine($oldGameXmlFile)
			$eeooff = (@error = -1)
			if GetAttr($str, "File") <> GetFileName($inFileName) Then
				FileWriteLine($newGameXmlFile, '		<Level Name="' & GetFileName($inFileName) & '" File="' & GetFileName($inFileName) & '.xml" />')
			EndIf
			if GetAttr($str, "Name") <> GetFileName($inFileName) Then FileWriteLine($newGameXmlFile, $str)
		EndIf
	Until  $eeooff
	FileClose($oldGameXmlFile)
	FileClose($newGameXmlFile)
	FileCopy($oldGameXmlFileName & ".temp", $oldGameXmlFileName, 1)
	FileDelete($oldGameXmlFileName & ".temp")
EndFunc



Func Convert()
	InitParams()
	FilesOpen()
	VerifySVG()
	Write('<?xml version="1.0" encoding="utf-8"?>', False)
	Write('<Level Name="' & $inFileName & '" Bounds="0, 0, 480, 320">', False)
	CollectAndWriteResources(True)

	Do
		$str = ReadStr()
		$newStr = ParseStr($str)
		Write($newStr, False)
	Until $EOinF

	Write('<Include File="Dashboard.xml" />', False)		
	Write('</Level>', False)
	
	FilesClose()
	FormatFile($outFileName)
	PutLevelToGameXml()
	
	if GUICtrlRead($cbAndExec) = $GUI_CHECKED Then ShellExecute($Root & "\Lunohod.Windows.exe", "", $Root & "\")
	if GUICtrlRead($cbAndShow) = $GUI_CHECKED Then ShellExecute($outFileName)
	
EndFunc


Func LevelExec()
	ShellExecute($Root & "\Lunohod.Windows.exe", "", $Root & "\")
EndFunc


Func LevelShow()
	ShellExecute($outFileName)
EndFunc



Func MakeArc()
Local $destFile

	ClipPut(GetFileName($inFileName))
	if GUICtrlRead($cbToDropbox) = $GUI_CHECKED Then
		FileSelectFolder("Select folder for resources root", IniRead($iniFile, "path", "DropboxFolder", @DesktopDir), 1)
	Else
		$ArcRoot = FileSelectFolder("Select folder for resources root", @DesktopDir, 1)
	EndIf
	
	if $ArcRoot <> "" Then
		FilesOpen()
		CollectAndWriteResources(False)
		FilesClose()
		$destFile = $ArcRoot & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml"
		myFileCopy($outFileName, $destFile, 9)
	EndIf
EndFunc


Func Error($txt)
	m_LogAdd('Error :' & $txt)
	MsgBox(16, "Error", $txt)
EndFunc


Func Halt($txt)
	m_LogAdd('Critical Error :' & $txt)
	MsgBox(16, "Critical Error", $txt)
	ExitProgram()
EndFunc


Func ExitProgram()
	FilesClose()
	MainWinClose()
	m_LogClose()
	Exit
EndFunc


Func MainLoop()
	
	$timer = TimerInit()
	
	While 1
		$msg = GUIGetMsg()
		Select
		Case $msg = $GUI_EVENT_CLOSE or $msg = $bExit
			ExitProgram()
		Case $msg = $bSelectFile
			FilesSelect()
		Case $msg = $bConvert
			Convert()
		Case $msg = $bMakeArc
			MakeArc()
		Case $msg = $bErrorTxt
			ShellExecute($lunohod_error)
			GUICtrlSetState($bErrorTxt, $GUI_HIDE)
		EndSelect
		
		if $lastFile <> $inFileName Then
			IniWrite($iniFile, "main", "LastSVGFile", $inFileName)			
			GUICtrlSetData($lFilename, GetFileName($inFileName))
			$lastFile = $inFileName
		EndIf
		
		if TimerDiff($timer) > 2000 Then
			$timer = TimerInit()
			if FileGetSize($lunohod_error) > 5 Then
				GUICtrlSetState($bErrorTxt, $GUI_SHOW)
			Else
				GUICtrlSetState($bErrorTxt, $GUI_HIDE)
			EndIf
		EndIf

	WEnd
	

EndFunc


m_LogStart("log.log", 1)
InitParams()
MainWinInit()
MainLoop()