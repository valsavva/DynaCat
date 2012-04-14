;SVG to XGAME
;<Element Attribute="SubAttribute:1111;">Value</Element>
Global $ProgVers = "1.5.07"

#include "Array.au3"
#include <Constants.au3>
#include <GUIConstants.au3>
#include <WindowsConstants.au3>
#include <EditConstants.au3>
#include <GDIPlus.au3>
#include <ScreenCapture.au3>
#include <GUIConstantsEx.au3>
#include <m_LogFile.au3>
#include <StaticConstants.au3>

;HotKeySet("^q", "Halt")


Global $EOinF = False, $inFileName, $outFileName, $inFile, $outFile

;Tag Types
Const $cTT_Start = "Tag Type: Start"
Const $cTT_Single = "Tag Type: Single"
Const $cTT_End = "Tag Type: End"

Const $iniFile = @ScriptDir & "\ini.ini"
Global $lastFile, $lunohod_error, $timer

Global $ElementsDict
Global $Root, $Bin, $Metadata, $Content, $ArcRoot, $ResourcesRootFolder, $xmlEditorOfSVG

Global $cDecimalplaces

Global $LayerTagOpen, $GroupTagOpen, $TeleportChain = False
Const $MaxIdCounter = 8
Dim $IdCounter[$MaxIdCounter]
Dim $IdCounterNames[$MaxIdCounter]
$IdCounterNames[1] = "Food"
$IdCounterNames[2] = "Enemy"
$IdCounterNames[3] = "Block"
$IdCounterNames[4] = "DesignImage"
$IdCounterNames[5] = "Animation"
$IdCounterNames[6] = "Teleport Animations"
$IdCounterNames[7] = "FoodPack"

; win
Global $ProgName = "SVG to XGAME xml converter v" & $ProgVers
Global $MainWin, $msg, $TypeList
Global $mFile, $mExit
Global $lFilename, $bSelectFile, $bConvert, $bMakeArc, $cbToDropbox, $cbAndExec, $cbAndShow, $bErrorTxt, $bWipeErrorTxt, $bExit, $bOpenSVG, $bEditSvg, $bCoordsSvg2XGame, $bCoordsXGame2Svg


Func InitParams()
	m_LogAdd('InitParams()')
	
	$LayerTagOpen = 0
	$GroupTagOpen = 0
	
	$ElementsDict = IniRead($iniFile, "main", "ElementsDict", "")
	$cDecimalplaces = IniRead($iniFile, "main", "Decimalplaces", 2)
	
	$ResourcesRootFolder = IniRead($iniFile, "path", "ResourcesRootFolder", "")
	$Root = IniRead($iniFile, "path", "Root", "")
	$Bin = IniRead($iniFile, "path", "Bin", "")
	$Content = IniRead($iniFile, "path", "Content", "")
	$Metadata = IniRead($iniFile, "path", "Metadata", "")
	if ($ResourcesRootFolder = "") or ($Root = "") or ($Bin = "")  or ($Content = "") or ($Metadata = "") Then Halt("#001# Wrong path in the ini file")

	$lastFile = IniRead($iniFile, "main", "LastSVGFile", "")
	
	if (not FileExists($lastFile)) or (GetExt($lastFile) <> "svg") Then
		FilesSelect()
	Else
		$inFileName = $lastFile
	EndIf
	
	m_LogAdd("input file: " & $inFileName)

    $lunohod_error = IniRead($iniFile, "main", "lunohod_error", "lunohod_error.txt")
	$xmlEditorOfSVG = IniRead($iniFile, "path", "xmlEditorOfSVG", "C:\Program Files\AutoIt3\SciTE\SciTE.exe")
	
	ResetCounters()
	m_LogAdd('InitParams:OK')

EndFunc

Func MainWinInit()
Local $winW = 406, $winH = 86

	$MainWin = GUICreate($ProgName, $winW, $winH, @DesktopWidth - $winW, 25, $WS_POPUPWINDOW)
	
	$bSelectFile = GUICtrlCreateButton("select file", 3, 3, 60, 22)
	$lFilename = GUICtrlCreateLabel(GetFileName($inFileName), 70, 6, 200)
	GUICtrlSetTip($lFilename, $inFileName)
	$bOpenSVG = GUICtrlCreateButton("open", 275, 3, 50, 20)
	$bEditSvg = GUICtrlCreateButton("xml edit", 330, 3, 50, 20)
	
	$bExit = GUICtrlCreateButton("X", $winW - 17, 3, 14, 14)
	
	$bConvert = GUICtrlCreateButton("convert", 3, 30, 60, 22)
	$cbAndExec = GUICtrlCreateCheckbox('exec', 70, 31, 40, 20)
	$cbAndShow = GUICtrlCreateCheckbox('show', 120, 31, 50, 20)
	$bCoordsSvg2XGame = GUICtrlCreateButton("\/", 180, 30, 20, 20)
	$bCoordsXGame2Svg = GUICtrlCreateButton("/\", 210, 30, 20, 20)
	GUICtrlSetState($bCoordsXGame2Svg, $GUI_HIDE)
	
	$bErrorTxt = GUICtrlCreateButton("error txt", 275, 30, 50, 20)
	GUICtrlSetState($bErrorTxt, $GUI_HIDE)
	GUICtrlSetColor($bErrorTxt, 0xFF0000)
	
	$bWipeErrorTxt = GUICtrlCreateButton("wipe err", 330, 30, 50, 20)
	GUICtrlSetState($bWipeErrorTxt, $GUI_HIDE)
	GUICtrlSetColor($bWipeErrorTxt, 0x0000FF)
	
	$bMakeArc = GUICtrlCreateButton("make arc", 3, 57, 60, 22)
	$cbToDropbox = GUICtrlCreateCheckbox("to dropbox", 70, 57, 100)
	
	GUISetFont(7, 400, 2)
	GUICtrlCreateLabel($ProgVers & '   ', $winW-45, $winH-12, 45, 12, $SS_RIGHT)
	
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

Func FilesOpen($reWrite)
Local $outputFileExist = False
	m_LogAdd('FilesOpen($reWrite:' & $reWrite & ')')
	$inFile = FileOpen($inFileName, 256)
	if $inFile = -1 then Halt("#002# fail to open file: " & $inFileName)
	
	if $reWrite Then
		$outFileName = $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml"
		if FileExists($outFileName) Then
			myFileCopy($outFileName, $Root & "\" & $Metadata & "\Bak\" & GetFileName($inFileName) & '~bak~' & GetRandomChar() & GetRandomChar() & GetRandomChar() & GetRandomChar() & ".xml", 9)
			$outputFileExist = True
		EndIf
		$outFile = FileOpen($outFileName, 256+2)
		if $outFile = -1 then Halt("#003# fail to create file: " & $outFileName)
	Else
		$outFileName = $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml"
		$outFile = FileOpen($outFileName, 256)
	EndIf
	$EOinF = False
	m_LogAdd('FilesOpen:OK')
	Return $outputFileExist
EndFunc

Func myFileSafetyCopy($src, $dest)
EndFunc

Func myFileCopy($src, $dest, $flag)
Local $ret
	$ret = FileCopy($src, $dest, $flag)
	if $ret <> 1 Then 
		Error("###010### Couldn't copy file: " & @CRLF & $src & @CRLF & "to" & @CRLF & $dest)
	Else
		m_LogAdd("file copied: " & $src & " to " & $dest)
	EndIf
	Return $ret
EndFunc

Func myFileDelete($f)
Local $ret
	$ret = FileDelete($f)
	if $ret <> 1 Then 
		Error("###011### Couldn't delete file: " & $f)
	Else
		m_LogAdd("file deleted: " & $f)
	EndIf
	Return $ret
EndFunc


Func FilesClose()
Local $bool = true
	m_LogAdd('FilesClose()')
	if (FileClose($inFile) = 0) Or (FileClose($outFile) = 0) Then m_LogAdd('already closed')
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


Func ResetCounters()
	for $i = 0 to $MaxIdCounter - 1
		$IdCounter[$i] = 0
	next
EndFunc

Func CoordsSvg2XGame()
Local $Stack = '<Stack ', $moveCounter = 0
    
	GUICtrlSetState($bCoordsSvg2XGame, $GUI_DISABLE)
	
	m_LogAdd('CoordsSvg2XGame()')
	
	
	;read images bounds from svg
	$inFile = FileOpen($inFileName, 256)
	if $inFile = -1 then Halt("#015# fail to open file: " & $inFileName)
	
	$EOinF = False
	Do
		$str = ReadStr()
		if GetElementName($str) = 'image' Then 
			$Stack = $Stack & 'id' & GetAttr($str, 'id') & '="' & GetAttr($str, 'x') & ', ' & GetAttr($str, 'y') & '" '
		EndIf
	Until $EOinF
	$Stack = $Stack & '/>' 
	;MsgBox(0, "", $Stack)
	FileClose($inFile)
	
	; convert coords
	$cinFileName = $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml"
	if Not FileExists($cinFileName) Then
		Error($inFileName & ' have to be converted')
		GUICtrlSetState($bCoordsSvg2XGame, $GUI_ENABLE)
		Return
	EndIf
	$cinFile = FileOpen($cinFileName, 256)
	if $cinFile = -1 then Halt("#016# fail to open file: " & $cinFileName)
	$coutFile = FileOpen(@ScriptDir & '\zzztempfile', 256+2)
	if $coutFile = -1 then Halt("#017# fail to create file: " & @ScriptDir & '\zzztempfile')	
		
	Do
		$str = FileReadLine($cinFile)
		$EOinF = (@error = -1)
		$NewBounds = GetAttr($Stack, 'id' & GetAttr($str, 'TraceId'))
		$OldBounds = GetAttr($str, 'Bounds')
		if (StringInStr($OldBounds, $NewBounds) = 0) and ($NewBounds <> '') and ($OldBounds <> '') Then
			$bArr = StringSplit($OldBounds, ',')
			$str = StringReplace($str, 'Bounds="' & $OldBounds & '"', 'Bounds="' & $NewBounds & ', ' & $bArr[3] & ', ' & $bArr[4] & '"')
			$moveCounter = $moveCounter + 1
			;MsgBox(0, "", $str)
		EndIf
		FileWriteLine($coutFile, $str)
	Until $EOinF

	FileClose($cinFile)
	FileClose($coutFile)
	FileCopy(@ScriptDir & '\zzztempfile', $cinFileName, 1)
	FileCopy(@ScriptDir & '\zzztempfile', $Bin & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", 1)
	FileDelete(@ScriptDir & '\zzztempfile')
	if $moveCounter = 0 Then 
		MsgBox(0, "", "Nothing cnanged")
		m_LogAdd('Nothing cnanged')
	Else
		MsgBox(0, "", "Objects moved: " & $moveCounter)
		m_LogAdd("Objects moved: " & $moveCounter)
	EndIf
	m_LogAdd('CoordsSvg2XGame:OK')
	GUICtrlSetState($bCoordsSvg2XGame, $GUI_ENABLE)
EndFunc


Func CoordsXGame2Svg()

EndFunc


Func VerifySVG()
Local $i = 1, $ErrorList = "", $teleports = ""
Local $reqList = '/tower/|/cat/|inkscape:label="action"'

    m_LogAdd('VerifySVG()')
	$reqArr = StringSplit($reqList, "|")
	Do
		$str = ReadStr()
		
		;check the path to the image file
		if GetElementName($str) = "image" Then 
			if Not FileExists(GetFullPath($str)) Then $ErrorList = $ErrorList & '###013### Error in SVG file: image file not found:' & GetFullPath($str) & "(id=" & GetAttr($str, "id") & ', line #' & $i & ')' & @CRLF
		EndIf
		
		;check transformed images and layers
		if (GetAttr($str, "transform") <> "") and (GetElementName($str) <> 'image') Then
			$ErrorList = $ErrorList & '###014### Error in SVG file: ' & GetElementName($str) & ' has been transformed (id=' & GetAttr($str, "id") & ', line #' & $i & ')' & @CRLF
		EndIf
		
		;check s matching
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

	FileSetPos($inFile, 0, $FILE_BEGIN)
	$EOinF = False

	if $ErrorList <> "" Then 
		Error($ErrorList)
		m_LogAdd('VerifySVG:ERRORS')
		Return False
	Else
		m_LogAdd('VerifySVG:OK')
		Return True
	EndIf
	
EndFunc

Func FormatFile($FileName)
Local $offset = "", $cur, $par, $tt
    
	m_LogAdd('FormatFile($FileName:'&$FileName&')')
	
	$inFile = FileOpen($FileName, 256)
	$outFile = FileOpen($FileName & "~~~", 256 + 2)
	$EOinF = False
	$str = Read()
	Write($str, True)
	Do
		$str = Read()
		if GetTagType($str) = $cTT_End then $offset = StringReplace($offset, "    ", "", 1)
		if (GetTagType($str) = $cTT_Start) and (GetElementName($str) <> "!--") then $cur = "    "
		$newStr = $offset & $str
		Write($newStr, True)
		$offset = $offset & $cur
		$cur = ""
	Until $EOinF

	FileClose($inFile)
	FileClose($outFile)
	FileCopy($FileName & "~~~", $FileName, 1)
	FileDelete($FileName & "~~~")
	
	m_LogAdd('FormatFile:OK')
	
EndFunc

Func CollectAndWriteResources($WriteRes)
Local $dir, $srcFile, $destFile

    m_LogAdd('CollectAndWriteResources($WriteRes:'&$WriteRes&')')
	
	if $WriteRes Then Write('<Resources RootFolder="' & $ResourcesRootFolder & '">', False)
		
	;collect
	$textures = ""
	Do
		$str = ReadStr()
		if GetElementName($str) = "image" Then
			$newStr = ParseStr($str)
			if StringInStr($newStr, "TextureId=") > 0 Then
				$fullpath = GetFullPath($str)
				if (not StringInStr($textures, "|" & $fullpath)) and (not StringInStr($fullpath, "unvisible")) Then $textures = $textures & "|" & $fullpath
				$sprite = GetSubAttr($str, "inkscape:label", "Sprite")
				$fullpath2 = ""
				if $sprite <> "" Then $fullpath2 = GetPath($fullpath) & $sprite & ".png"
				if (not StringInStr($textures, "|" & $fullpath2)) Then 
					$textures = $textures & "|" & $fullpath2	
				EndIf
			EndIf
		EndIf
	Until $EOinF
	$texturesArr = StringSplit($textures, "|")
	$filesNotification = ""

	;write (copy files) resources
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
			if $WriteRes Then Write('<Texture Id="txr_' & $filename & '" Source="' & $filename & '" />', False)
			$destFile = $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png"
			if FileExists($fullpath) Then
				if FileExists($destFile) Then
					$iSize1 = GetImageSize($fullpath)
					$iSize2 = GetImageSize($destFile)					
					if (FileGetSize($fullpath) <> FileGetSize($destFile)) or ($iSize1[0] <> $iSize2[0]) or ($iSize1[1] <> $iSize2[1]) Then
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
					FileAddToCSProj($filename & ".png", $Content & "\" & $ResourcesRootFolder, 'Lunohod.csproj')
					FileAddToCSProj($filename & ".png", $Content & "\" & $ResourcesRootFolder, 'Lunohod.Windows.csproj')
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
	
	m_LogAdd('CollectAndWriteResources:OK')
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

Func GetDegree($str)
	$s = GetAttr($str, "transform")
	if $s = "" Then Return 0
	$s = StringReplace($s, "matrix(", "")
	$s = StringReplace($s, ")", "")
	$sarr = StringSplit($s, ",")
	if $sarr[0] = 1 Then Return 0 ;error
	
	;MsgBox(0, "RotaX", GetAttr($str, "x")*$sarr[1] + GetAttr($str, "y")*$sarr[2])
	;MsgBox(0, "RotaY", GetAttr($str, "x")*$sarr[3] + GetAttr($str, "y")*$sarr[4])
	
	Return ACos($sarr[1])*180/3.14159265358979
EndFunc

Func GetRotaX($str)
	$s = GetAttr($str, "transform")
	if $s = "" Then Return 0
	$s = StringReplace($s, "matrix(", "")
	$s = StringReplace($s, ")", "")
	$sarr = StringSplit($s, ",")
	if $sarr[0] = 1 Then Return 0 ;error
	$a = GetAttr(StringReplace($str, "matrix", "m-t-r-x"), "x")
	$b = GetAttr(StringReplace($str, "matrix", "m-t-r-x"), "y")
	$ret = $a*$sarr[1] - $b*$sarr[2]
	;MsgBox(0, '$a', $a)
	;MsgBox(0, '$sarr[1]', $sarr[1])
	;MsgBox(0, '$b', $b)
	;MsgBox(0, '$sarr[2]', $sarr[2])
	;MsgBox(0, 'ret', $ret)
	Return Round($ret)
EndFunc

Func GetRotaY($str)
	$s = GetAttr($str, "transform")
	if $s = "" Then Return 0
	$s = StringReplace($s, "matrix(", "")
	$s = StringReplace($s, ")", "")
	$sarr = StringSplit($s, ",")
	if $sarr[0] = 1 Then Return 0 ;error
	
	Return Round((GetAttr(StringReplace($str, "matrix", "m-t-r-x"), "x")*$sarr[2] + GetAttr(StringReplace($str, "matrix", "m-t-r-x"), "y")*$sarr[1]))
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
	
	if $attr = "@rotation" then  
		if GetAttr($str, "transform") <> "" Then 
			Return Round(GetDegree($str), $cDecimalplaces)
		Else
			Return "0"
		EndIf
	EndIf
	
	if $attr = "@halfWidh" then  Return Round(GetAttr($str, "width")/2)
	if $attr = "@halfHeight" then  Return Round(GetAttr($str, "height")/2)
    
	if StringInStr($attr, "@counter") <> 0 then  
		;MsgBox(0, "", GetSubAttr($str, "style", "opacity"))
		$counterNumber = StringMid($attr, 9, 1)
		$IdCounter[$counterNumber] = $IdCounter[$counterNumber] + 1
		Return $IdCounter[$counterNumber]
	EndIf

	if StringInStr($attr, "@cntrValue") <> 0 then  
		;MsgBox(0, "", GetSubAttr($str, "style", "opacity"))
		$counterNumber = StringMid($attr, 11, 1)
		Return $IdCounter[$counterNumber]
	EndIf

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
			if GetFileName(GetFullPath($str)) = "left" then 
				$k = -1
			Else
				$k=1
			EndIf
			Return $k*Round(GetAttr($str, "width"))
		Else
			if GetFileName(GetFullPath($str)) = "up" then 
				$k = -1
			Else
				$k=1
			EndIf
			Return $k*Round(GetAttr($str, "height"))
		EndIf
	EndIf
	
	if $attr = "@horX" Then
		if GetAttr($str, "width") > GetAttr($str, "height") Then
			if GetFileName(GetFullPath($str)) = "left" then 
				Return GetAttr($str, "x") + GetAttr($str, "width")
			Else
				Return GetAttr($str, "x")
			EndIf
		Else
			Return -7777777
		EndIf
	EndIf
	if $attr = "@horY" Then
		if GetAttr($str, "width") > GetAttr($str, "height") Then
			Return GetAttr($str, "y")
		Else
			Return -7777777
		EndIf
	EndIf
	if $attr = "@vertY" Then
		if GetAttr($str, "height") > GetAttr($str, "width") Then
			if GetFileName(GetFullPath($str)) = "up" then 
				Return GetAttr($str, "y") + GetAttr($str, "height")
			Else
				Return GetAttr($str, "y")
			EndIf
		Else
			Return -7777777
		EndIf
	EndIf
	if $attr = "@vertX" Then
		if GetAttr($str, "height") > GetAttr($str, "width") Then
			Return GetAttr($str, "x")
		Else
			Return -7777777
		EndIf
	EndIf
	
	if $attr = "@Points" Then 
		$ret = GetSubAttr($str, "inkscape:label", "Points")
		if $ret = "" Then
			;MsgBox(0, "", 'Points is not defined in the tag <' & GetElementName($str) &'> (id="' & GetAttr($str, "id") & '"). Points set to default (1)')
			$ret = "1"
		EndIf
		Return $ret
	EndIf
	if $attr = "@Duration" Then 
		$ret = GetSubAttr($str, "inkscape:label", "Duration")
		if $ret = "" Then
			;calc a Duration by hero speed
			$len = GetAttr($str, "width")
			if GetAttr($str, "height") > $len Then $len = GetAttr($str, "height")
			$dur = Round($len/84, 2)
			;MsgBox(0, "", 'Duration is not defined in the tag <' & GetElementName($str) &'> (id="' & GetAttr($str, "id") & '"). Duration set to hero speed (' & $dur & ')')
			$ret = $dur
		EndIf
		Return $ret
	EndIf
	if $attr = "@TeleportDuration" Then 
		$ret = GetSubAttr($str, "inkscape:label", "Duration")
		if $ret = "" Then
			;calc a Teleport Duration 
			$len = GetAttr($str, "width")
			if GetAttr($str, "height") > $len Then $len = GetAttr($str, "height")
			$dur = Round($len/200, 2)
			;MsgBox(0, "", 'Duration is not defined in the tag <' & GetElementName($str) &'> (id="' & GetAttr($str, "id") & '"). Duration set to hero speed (' & $dur & ')')
			$ret = $dur
		EndIf
		Return $ret
	EndIf
	if $attr = "@OutDirection" Then 
		$ret = ''
		$ret = StringReplace(GetFileName(GetFullPath($str)), 'out_', '')
		if $ret = '' Then 
			Return 'up'
		Else
			Return $ret
		EndIf
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
			
			if $attr = "x" and (StringInStr($str, 'matrix') > 0 ) Then
				Return GetRotaX($str)
			EndIf
			if $attr = "y" and (StringInStr($str, 'matrix') > 0 ) Then
				Return GetRotaY($str)
			EndIf
			if $attr = "x" or $attr = "y" or $attr = "width" or $attr = "height" Then
				Return Round($arr[$i+1])
			else
				Return $arr[$i+1]
			EndIf
		EndIf
	next
	Return ""	
EndFunc

Func SetAttr($str, $attr, $newValue)
	
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
		Halt("###005### BAD SIGNS /\/\ : " & $fullPath)
	Else
		$arr = StringSplit($fullPath, "\")
	EndIf
	if $arr[0] = 1 Then Return ""
	Return StringLeft($arr[$arr[0]], StringInStr($arr[$arr[0]], ".") - 1)
EndFunc

Func GetExt($fullPath)
	if StringInStr($fullPath, "/") Then
		Halt("###006### BAD SIGNS /\/\ : " & $fullPath)
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
		Halt("###007### BAD SIGNS /\/\ : " & $fullPath)
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
			if $TeleportChain Then $TeleportChain = False
			Return "</Group>"
		EndIf
		
		if $tt = $cTT_End and ($LayerTagOpen > 0) Then
			$LayerTagOpen = $LayerTagOpen - 1 
			Return "</Layer>"
		EndIf
		
		if $tt = $cTT_End Then Error('###012### Parse tags error!')
			
		if $tt = $cTT_Single Then Return "";delete empty layer or group
			
		if GetAttr($str, "inkscape:groupmode") = "layer" Then	;layer
			$LayerTagOpen = $LayerTagOpen + 1
			Return '<Layer Id="' & GetAttr($str, "inkscape:label") & '" BackColor="#FFFFFFFF">'
		Else	;group
			$GroupTagOpen = $GroupTagOpen + 1
			if GetSubAttr($str, "inkscape:label", "Teleport") <> "" Then 
				$TeleportChain = True
			EndIf
			Return '<Group Id="grp_' & GetAttr($str, "id") & '">' ; old: '" Edges="None">'
			
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

Func FixScale($str)
	if StringInStr($str, 'scale(-1,-1)') Then 
		Return StringReplace($str, 'scale(-1,-1)', 'matrix(-1,0,0,-1,0,0)')
	Else
		Return $str
	EndIf
EndFunc

Func ReadStr()
	$str = Read()
	$str = FixScale($str)
	
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

    m_LogAdd('PutLevelToGameXml()')

	if Not FileExists($oldGameXmlFileName) Then Halt("###009### Could not open file " & $oldGameXmlFileName)
	
	$oldGameXmlFile = FileOpen($oldGameXmlFileName, 256)
	$newGameXmlFile = FileOpen($oldGameXmlFileName & ".temp", 256 + 2)
	
	Do
		$str = FileReadLine($oldGameXmlFile)
		$eeooff = (@error = -1)
		if GetAttr($str, "Id") <> GetFileName($inFileName) Then FileWriteLine($newGameXmlFile, $str)
		if StringInStr($str, "<Levels>") Then
			$str = FileReadLine($oldGameXmlFile)
			$eeooff = (@error = -1)
			if GetAttr($str, "Id") <> GetFileName($inFileName) Then
				FileWriteLine($newGameXmlFile, '		<Level Id="' & GetFileName($inFileName) & '" File="' & GetFileName($inFileName) & '.xml" />')
			EndIf
			if GetAttr($str, "Name") <> GetFileName($inFileName) Then FileWriteLine($newGameXmlFile, $str)
		EndIf
	Until  $eeooff
	FileClose($oldGameXmlFile)
	FileClose($newGameXmlFile)
	FileCopy($oldGameXmlFileName & ".temp", $oldGameXmlFileName, 1)
	FileDelete($oldGameXmlFileName & ".temp")
	
	m_LogAdd('PutLevelToGameXml:OK')
EndFunc

Func FileAddToCSProj($fn, $path, $prj); write $path\$fn to $prj file
	
Local $oldPrjFileName = $Root & "\" & $prj
Local $oldPrjFile, $eeooff = False

    m_LogAdd('FileAddToCSProj(' & $fn & ', ' & $prj & ')')

	if Not FileExists($oldPrjFileName) Then Halt("###018### Could not find project file " & $Root & '\' & $prj)
	
	$oldPrjFile = FileOpen($oldPrjFileName)
	$newPrjFile = FileOpen($oldPrjFileName & ".temp", 2)
	
	Do
		$str = FileReadLine($oldPrjFile)
		$eeooff = (@error = -1)
		FileWriteLine($newPrjFile, $str)
		if StringInStr($str, '<Content Include="Metadata\Game.xml">') Then
			Do 
				$str = FileReadLine($oldPrjFile)
				$eeooff = (@error = -1)
				FileWriteLine($newPrjFile, $str)
			Until StringInStr($str, '</Content>')
			FileWriteLine($newPrjFile, '    <Content Include="' & $path & '\' & $fn & '">')
			FileWriteLine($newPrjFile, '      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>')
			FileWriteLine($newPrjFile, '    </Content>')
		EndIf
	Until  $eeooff
	FileClose($oldPrjFile)
	FileClose($newPrjFile)
	FileCopy($oldPrjFileName & ".temp", $oldPrjFileName, 1)
	FileDelete($oldPrjFileName & ".temp")
	
	m_LogAdd('FileAddToCSProj(' & $fn & ', ' & $prj & '):OK')
EndFunc

Func Convert()
Local $includes = "", $cls, $xmlExist = False

    m_LogAdd('Convert()')

    GUICtrlSetState($bConvert, $GUI_DISABLE)

	InitParams()
	$xmlExist = FilesOpen(True)
	if VerifySVG() Then
		Write('<?xml version="1.0" encoding="utf-8"?>', False)
		Write('<!-- '& $ProgName &' -->', False)
		;Write('<!-- date: ' & @MDAY & '/' & @MON & ' ' & @YEAR & '   ' & @HOUR & ':' & @MIN &' -->', False)
		Write('<!-- source: '& $inFileName &' -->', False)
		Write('', False)
		Write('<Level xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:noNamespaceSchemaLocation="file:///D:/Lunohod/Documentation/schema0.xsd" Name="' & GetFileName($inFileName) & '" Bounds="0, 0, 480, 320">', False)
		Write('', False)
		Write('<LevelSettings Difficulty="-1" BombCount="7777777" HeroHealth="100" ExplosionClass="clsExplosion"/>', False) ; 
		Write('', False)

		; write includes
		
		Do
			$str = ReadStr()
			$newStr = ParseStr($str)
			
			$cls = GetAttr($newStr, "Class")
			if $cls <> "" Then 
				$className = StringRight($cls, StringLen($cls)-3)
				if StringInStr($includes, $className & '.xml') = 0 Then $includes = $includes & '<Include File="Class' & $className & '.xml" />' & @CRLF
			EndIf
				
		Until $EOinF
		
		Write($includes, False)		
		Write('', False)
		FileSetPos($inFile, 0, $FILE_BEGIN)
		$EOinF = False
		
		CollectAndWriteResources(True)
		Write('', False)

		; convert
		ResetCounters()
		
		Do
			$str = ReadStr()
			$newStr = ParseStr($str)
			
			$cls = GetAttr($newStr, "Class")
			if $cls <> "" Then 
				$className = StringRight($cls, StringLen($cls)-3)
				if StringInStr($includes, $className & '.xml') = 0 Then $includes = $includes & '<Include File="Class' & $className & '.xml" />' & @CRLF
			EndIf
				
			Write($newStr, False)
		Until $EOinF
		Write('', False)
		Write('<Include File="ClassExplosion.xml" />', False)		
		Write('<Include File="Dashboard.xml" />', False)		
		Write('</Level>', False)
		
		$stat = '<!-- '
		For $i = 1 to $MaxIdCounter - 1
			$stat = $stat & $IdCounterNames[$i] & '=' & $IdCounter[$i] & '  '
		Next
		
		Write($stat &' -->', False)
		Write('', False)

		FilesClose()
		FormatFile($outFileName)
		;PutLevelToGameXml()
		
		if Not $xmlExist Then
			FileAddToCSProj(GetFileName($inFileName) & ".xml", $Metadata, 'Lunohod.csproj')
			FileAddToCSProj(GetFileName($inFileName) & ".xml", $Metadata, 'Lunohod.Windows.csproj')
		EndIf
		
		FileCopy($Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", $Bin & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", 1)
		FileCopy($Root & "\" & $Metadata & "\" & "Game.xml", $Bin & "\" & $Metadata & "\" & "Game.xml", 1)
		
		if GUICtrlRead($cbAndExec) = $GUI_CHECKED Then 
			Sleep(1000)
			ShellExecute($Bin & "\Lunohod.Windows.exe", "", $Root & "\")
		EndIf

		if GUICtrlRead($cbAndShow) = $GUI_CHECKED Then 
			Sleep(1000)
			ShellExecute($outFileName)
		EndIf
		
		
		GUICtrlSetState($bConvert, $GUI_ENABLE)
		
		m_LogAdd('Convert:OK')
		
	EndIf
	
EndFunc

Func LevelExec()
	ShellExecute($Root & "\Lunohod.Windows.exe", "", $Root & "\")
EndFunc

Func LevelShow()
	ShellExecute($outFileName)
EndFunc

Func MakeArc()
Local $destFile

    m_LogAdd('MakeArc()')
	
	ClipPut(GetFileName($inFileName))
	if GUICtrlRead($cbToDropbox) = $GUI_CHECKED Then
		$ArcRoot = FileSelectFolder("Select folder for resources root", IniRead($iniFile, "path", "DropboxFolder", @DesktopDir), 1)
	Else
		$ArcRoot = FileSelectFolder("Select folder for resources root", @DesktopDir, 1)
	EndIf
	
	if $ArcRoot <> "" Then
		FilesOpen(False)
		CollectAndWriteResources(False)
		FilesClose()
		$destFile = $ArcRoot & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml"
		myFileCopy($outFileName, $destFile, 9)
		MsgBox(64, "Collected", 'File ' & $outFileName & ' copied to ' &  $destFile)
	EndIf
	
	m_LogAdd('MakeArc:OK')
EndFunc

Func Error($txt)
	m_LogAdd('ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR')
	m_LogAdd('ERROR:' & $txt)
	if MsgBox(17, 'Error', $txt & @CRLF & @CRLF & "Ignore and continue?" ) <> 1 Then Halt('Halt from dialog')
EndFunc

Func Halt($txt)
	m_LogAdd('CRITICAL ERROR CRITICAL ERROR CRITICAL ERROR CRITICAL ERROR CRITICAL ERROR')
	m_LogAdd('CRITICAL ERROR' & $txt)
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
		Case $msg = $bOpenSVG
			ShellExecute($inFileName)
		Case $msg = $bEditSvg
			ShellExecute($xmlEditorOfSVG, $inFileName)
		Case $msg = $bConvert
			Convert()
		Case $msg = $bCoordsSvg2XGame
			CoordsSvg2XGame()
		Case $msg = $bMakeArc
			MakeArc()
		Case $msg = $bErrorTxt
			ShellExecute($bin & '\' & $lunohod_error)
			;GUICtrlSetState($bErrorTxt, $GUI_HIDE)
		Case $msg = $bWipeErrorTxt
			FileOpen($Bin & '\' & $lunohod_error, 256 + 2)
			FileWriteLine($Bin & '\' & $lunohod_error, "")
			FileClose($Bin & '\' & $lunohod_error)
		EndSelect
		
		if $lastFile <> $inFileName Then
			IniWrite($iniFile, "main", "LastSVGFile", $inFileName)			
			GUICtrlSetData($lFilename, GetFileName($inFileName))
			$lastFile = $inFileName
		EndIf
		
		if TimerDiff($timer) > 2000 Then
			$timer = TimerInit()
			if FileGetSize($Bin & '\' & $lunohod_error) > 5 Then
				GUICtrlSetState($bErrorTxt, $GUI_SHOW)
				GUICtrlSetState($bWipeErrorTxt, $GUI_SHOW)
			Else
				GUICtrlSetState($bErrorTxt, $GUI_HIDE)
				GUICtrlSetState($bWipeErrorTxt, $GUI_HIDE)
			EndIf
		EndIf

	WEnd
	

EndFunc


m_LogStart("log.log", 1)
m_LogAdd($ProgName)
InitParams()
MainWinInit()
MainLoop()