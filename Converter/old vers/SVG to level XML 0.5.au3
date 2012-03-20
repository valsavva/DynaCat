;SVG to level XML 0.5
;<Element Attribute="SubAttribute:1111;">Value</Element>

#include "Array.au3"
#include <Constants.au3>
#include <GDIPlus.au3>
#include <ScreenCapture.au3>


Global $EOinF = False, $inFileName, $outFileName, $inFile, $outFile

;Tag Types
Const $cTT_Start = "Tag Type: Start"
Const $cTT_Single = "Tag Type: Single"
Const $cTT_End = "Tag Type: End"

Const $iniFile = @ScriptDir & "\ini.ini"
Global $lastFile = "", $quickBoot = "False"

Global $ElementsDict = ""
Global $FirstLayer = True, $quickBoot = "False", $Root = "", $Metadata = "", $Content = "", $ArcRoot = "", $ResourcesRootFolder = ""

Global $cDecimalplaces

Global $LayerTagOpen = 0, $GroupTagOpen = 0


Func ProgInit()
	
	$quickBoot = IniRead($iniFile, "main", "quickBoot", "False")
	$lastFile = IniRead($iniFile, "main", "LastSVGFile", "")
	
	if ($quickBoot = "True") and ($lastFile <> "") then 
		$inFileName = $lastFile
	Else
		if $CmdLine[0] <> 0 Then
			$inFileName = $CmdLine[1]
			if (not FileExists($inFileName)) or (GetExt($inFileName) <> "svg") Then 
				$inFileName = FileOpenDialog("Select input file", @ScriptDir & "\", "SVG Files (*.svg)", 5, "test_b-1.svg") ;
			EndIf
		Else
				$inFileName = FileOpenDialog("Select input file", $lastFile, "SVG Files (*.svg)", 1) ;
				IniWrite($iniFile, "main", "LastSVGFile", $inFileName)
		EndIf
	EndIf
	
	
	if $inFileName = "" then Exit
	
	$inFile = FileOpen($inFileName, 0)
	if $inFile = -1 then 
		MsgBox(0,  "", "fail to open file: " & $inFileName)
		Exit
	EndIf
	
	$outFileName = $inFileName & ".xml"
	$outFile = FileOpen($outFileName, 2)
	if $outFile = -1 then 
		FileClose($inFile)
		MsgBox(0,  "", "fail to open file: " & $inFileName)
		Exit
	EndIf
		
	$ElementsDict = IniRead($iniFile, "main", "ElementsDict", "")
	$cDecimalplaces = IniRead($iniFile, "main", "Decimalplaces", 2)
	
	$ResourcesRootFolder = IniRead($iniFile, "path", "ResourcesRootFolder", "")
	$Root = IniRead($iniFile, "path", "Root", "")
	$Content = IniRead($iniFile, "path", "Content", "")
	$Metadata = IniRead($iniFile, "path", "Metadata", "")
	if ($ResourcesRootFolder = "") or ($Root = "") or ($Content = "") or ($Metadata = "") Then ProgClose()
	
	if MsgBox(33, "Make Arc", 'Make arc?') = 1 Then
		ClipPut(GetFileName($inFileName))
		$ArcRoot = FileSelectFolder("Select folder for resources root", GetPath($lastFile), 1)
	Else
		$ArcRoot = ""
	EndIf

EndFunc

Func ProgClose()
	FileClose($inFile)
	FileClose($outFile)
EndFunc

Func VerifySVG()
Local $i = 1, $name, $ErrorList = "", $teleports = ""
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
		if MsgBox(17, 'Errors in SVG', $ErrorList & @CRLF & @CRLF & "Ignore and continue?" ) <> 1 Then Exit
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

Func CollectAndWriteResources($TakeIniFile)
	
	if $TakeIniFile Then Write('<Resources RootFolder="' & $ResourcesRootFolder & '">', False)
	$textures = ""
	Do
		$str = ReadStr()
		if GetElementName($str) = "image" Then
			$fullpath = GetFullPath($str)
			if (not StringInStr($textures, "|" & $fullpath)) and (not StringInStr($fullpath, "unvisible")) Then $textures = $textures & "|" & $fullpath
			$sprite = GetSubAttr($str, "inkscape:label", "Sprite")
			$fullpath2 = ""
			if $sprite <> "" Then $fullpath2 = GetPath($fullpath) & $sprite & ".png"
			if (not StringInStr($textures, "|" & $fullpath2)) and (not StringInStr($fullpath2, "unvisible")) Then 
				$textures = $textures & "|" & $fullpath2	
			EndIf
		EndIf
	Until $EOinF
	$texturesArr = StringSplit($textures, "|")
	$filesNotification = ""
Local $dir
	if $TakeIniFile Then 
		$dir = $Root
	Else
		$dir = $ArcRoot
	EndIf
	if $dir = "" then ProgClose()
	for $i = 1 to $texturesArr[0]
		if $texturesArr[$i] <> "" Then
			$fullpath = $texturesArr[$i]
			$filename = GetFileName($fullpath)
			if $TakeIniFile Then Write('<Texture Id="' & $filename & '" Source="' & $filename & '" />', False)
			if FileExists($dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png") Then
				if (FileGetSize($fullpath) <> FileGetSize($dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png")) or (FileGetTime($fullpath, 0, 1) <> FileGetTime($dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png", 0, 1))  Then
					$randomName = Random(10000, 99999, 1) &  Chr(Random(Asc("A"), Asc("Z"), 1)) & Chr(Random(Asc("A"), Asc("Z"), 1)) & Chr(Random(Asc("A"), Asc("Z"), 1))
					FileCopy($dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png", $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".bak~" & $randomName & ".png")
					$filesNotification = $filesNotification & "BACKUP: " & $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".bak~" & $randomName & ".png" & @CRLF
					FileCopy($fullpath, $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png", 9)
					$filesNotification = $filesNotification & "Copied: " & $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png" & @CRLF
					FileDelete($dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".xnb")
					$filesNotification = $filesNotification & "DELETED: " & $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".xnb"
				EndIf
			Else
				FileCopy($fullpath, $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png", 9)
				$filesNotification = $filesNotification & "Copied: " & $dir & "\" & $Content & "\" & $ResourcesRootFolder & "\" & $filename & ".png" & @CRLF
			EndIf
		EndIf
	Next
	if $filesNotification <> "" Then MsgBox(64, "", $filesNotification)					
	if $TakeIniFile Then Write("</Resources>", False)
	
	FileSetPos($inFile, 0, $FILE_BEGIN)
	$EOinF = False
EndFunc

;----
;TAGS

Func GetTagType($str)
	if (StringMid($str, 1, 2) = "</") then Return $cTT_End
	if StringMid($str, StringLen($str)-1, 2) = "/>" then Return $cTT_Single
	Return $cTT_Start
EndFunc

Func GetElementName($str)
	$str = StringRegExpReplace($str, "[/<>]", "")
	$arr = StringSplit($str, " ")
	Return $arr[1]
EndFunc

Func GetAttr($str, $attr)

	;getting special attribute
	
	if $attr = "@filename" then Return GetFileName(GetAttr($str, "xlink:href"))
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
		_GDIPlus_Startup ()
		$hImage = _GDIPlus_ImageLoadFromFile (GetFullPath($str))
		$iX = _GDIPlus_ImageGetWidth ($hImage)
		$iY = _GDIPlus_ImageGetHeight ($hImage)
		_GDIPlus_ImageDispose ($hImage)
		_GDIPlus_ShutDown ()
		if ($iX <> GetAttr($str, "width")) or ($iY <> GetAttr($str, "height")) Then
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

;TAGS
;----


;---------
;FILE:///
Func GetFullPath($str)
	$fullpath = GetAttr($str, "xlink:href")
	$fullpath = StringReplace($fullpath, "file:///", "")
	Return $fullpath
EndFunc

Func GetFileName($fullPath)
	$arr = StringSplit($fullPath, "/")
	if $arr[0] = 1 then $arr = StringSplit($fullPath, "\")
	if $arr[0] = 1 Then Return ""
	Return StringLeft($arr[$arr[0]], StringInStr($arr[$arr[0]], ".") - 1)
EndFunc

Func GetExt($fullPath)
	$arr = StringSplit($fullPath, "/")
	if $arr[0] = 1 then $arr = StringSplit($fullPath, "\")
	if $arr[0] = 1 Then Return ""
	$arr2 = StringSplit($arr[$arr[0]], ".")
	if $arr2[0] = 1 Then Return ""
	Return $arr2[$arr2[0]]
EndFunc

Func GetPath($fullPath)
	Return StringReplace($fullPath, GetFileName($fullPath) & "." & GetExt($fullPath), "")
EndFunc

Func GetParentDir($fullPath)
	$arr = StringSplit($fullPath, "/")
	if $arr[0] = 1 then $arr = StringSplit($fullPath, "\")
	if $arr[0] = 1 Then Return ""
	Return $arr[$arr[0]-1]
EndFunc

;FILE:///
;--------

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
			Return '<Group Id="' & GetAttr($str, "id") & '" DefaultEdge="None">'
			
		EndIf
	EndIf
	;g
	
	;image
	if $el = "image" Then
		$fullpath = GetFullPath($str)
		$fn = GetFileName($fullpath)
		$ret = ""
		if GetParentDir($fullpath) = "block" Then 
			$ret='<Block Id="' & GetAttr($str, 'id') & '" Bounds="' & GetAttr($str, 'x') & ', ' & GetAttr($str, 'y') & ', ' & GetAttr($str, 'width') & ', ' & GetAttr($str, 'height') & '" DefaultEdge="'
			if StringInStr($fn, "B", 0, 2) > 0 Then
				$DefaultEdge = "Bounce"
			Else
				$DefaultEdge = "Stick"
			EndIf
			$ret = $ret & $DefaultEdge
			$ttOpened = False
			$align = "Top|Right|Bottom|Left"
			$alignArr = StringSplit($align, "|")
			for $i = 1 to 4
				if StringMid($fn, $i, 1) <> StringMid($DefaultEdge, 1, 1) Then
					if not $ttOpened Then $ret = $ret & '">' & @CRLF
					$ttOpened = True
					Switch StringMid($fn, $i, 1)
					Case "B"
						$ret = $ret & '<Edge Type="' & 'Bounce' & '" Align="' & $alignArr[$i] & '" />' & @CRLF
					Case "S"
						$ret = $ret & '<Edge Type="' & 'Stick' & '" Align="' & $alignArr[$i] & '" />' & @CRLF
					Case "T"
						$ret = $ret & '<Teleport Align="' & $alignArr[$i] & '" Id="' & GetSubAttr($str, "inkscape:label", "TeleportId") & '" />' & @CRLF
					EndSwitch
				EndIf
			Next
			if not $ttOpened Then 
				$ret = $ret & '" />'
			Else
				$ret = $ret & '</Block>'
			EndIf
			Return $ret
		Else
			if GetSubAttr($str, "inkscape:label", "Sprite") <> "" Then
				$maskFile = GetPath($fullPath) & "sprite_" & GetSubAttr($str, "inkscape:label", "Sprite") & ".txt"
			Else
				$maskFile = GetPath($fullPath) & GetParentDir($fullpath) & ".txt"
			EndIf
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


ProgInit()
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


if $quickBoot = "False" Then
	if $ArcRoot <> "" Then
		FileSetPos($inFile, 0, $FILE_BEGIN)
		$EOinF = False
		CollectAndWriteResources(False)
		
		;$ArcRoot
	EndIf
EndIf

ProgClose()
FormatFile($outFileName)

if FileExists($Root & "\" & $Metadata & "\") Then
	if $quickBoot = "False" Then 
		if MsgBox(33, "Copy", 'Copy file ' & @CRLF & $outFileName & @CRLF & 'to ' & $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml" & ' ?') = 1 Then
			FileCopy($outFileName, $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", 1)
		EndIf
	Else
		FileCopy($outFileName, $Root & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", 1)
	EndIf
EndIf

if ($quickBoot = "False") and ($ArcRoot <> "") Then
	FileCopy($outFileName, $ArcRoot & "\" & $Metadata & "\" & GetFileName($inFileName) & ".xml", 9)
EndIf
	
ClipPut(GetFileName($inFileName))
	
	
	
	