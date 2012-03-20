;rotate 90 and flip
;<Element Attribute="SubAttribute:1111;">Value</Element>

#include "Array.au3"
#include <Constants.au3>


Global $EOinF = False, $inFileName, $outFileName, $inFile, $outFile

;Tag Types
Const $cTT_Start = "Tag Type: Start"
Const $cTT_Single = "Tag Type: Single"
Const $cTT_End = "Tag Type: End"

Global $FirstLayer = True
Global $cntObj = 0, $cntBlock = 0, $cntDesign = 0
Global $flip = False
Global $WW, $HH, $dX, $dY

Func ProgInit()

	$dX = InputBox("dX", "dX", "0")
	$dY = InputBox("dY", "dY", "0")
	$ans = MsgBox(36, "Flip?", "Flip?")
	$flip = ($ans = 6)

	if $CmdLine[0] <> 0 Then
		$inFileName = $CmdLine[1]
		if (not FileExists($inFileName)) or (GetExt($inFileName) <> "xml") Then 
			$inFileName = FileOpenDialog("Select input file", @ScriptDir & "\", "XML Files (*.xml)", 1) ;
		EndIf
	Else
			$inFileName = FileOpenDialog("Select input file", @ScriptDir & "\", "XML Files (*.xml)", 1) ;
	EndIf
	
	if $inFileName = "" then Exit
	
	$inFile = FileOpen($inFileName, 0)
	if $inFile = -1 then 
		MsgBox(0,  "", "fail to open file: " & $inFileName)
		Exit
	EndIf
	
	if $flip Then 
		$outFileName = $inFileName & "~flip~"
	Else
		$outFileName = $inFileName & "~norm~"
	EndIf
	$outFileName = $outFileName & "~bttns~.xml"
	

	$outFile = FileOpen($outFileName, 2)
	if $outFile = -1 then 
		FileClose($inFile)
		MsgBox(0,  "", "fail to open file: " & $inFileName)
		Exit
	EndIf
	
EndFunc


Func ProgClose()
	FileClose($inFile)
	FileClose($outFile)
EndFunc

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
	$arr = StringSplit($str, '"')
	if $arr[0] = 1 then Return ""
	for $i = 1 to $arr[0] - 1
		if StringInStr($arr[$i], $attr & "=") > 0 then Return $arr[$i+1]
	next
	Return ""	
EndFunc

Func ReplaceAttr($str, $attr, $newAttr)
	$arr = StringSplit($str, '"')
	if $arr[0] = 1 then Return ""
	for $i = 1 to $arr[0] - 1
		if StringInStr($arr[$i], $attr & "=") > 0 then $arr[$i+1] = $newAttr
	next
	$ret = ""	
	for $i = 1 to $arr[0]
		$ret = $ret & $arr[$i]
		if $i <> $arr[0] then $ret = $ret & '"'
	next
	Return $ret	
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

Func GetValue($str)
	Return StringMid($str, StringInStr($str, ">") + 1, StringInStr($str, "<", 0, 2) - 1 - StringInStr($str, ">"))
EndFunc

Func Width_to_Height($str)
	if StringInStr($str, "Width") = 0 then Return $str
	if StringInStr($str, "Height") = 0 then Return $str
	$HH = GetAttr($str, "Width")
	$WW = GetAttr($str, "Height")
	$ret = ReplaceAttr($str, "Width", $WW)
	$ret = ReplaceAttr($ret, "Height", $HH)
	Return $ret
EndFunc

Func flip_n_rot($str)
	if StringInStr($str, "Bounds") = 0 then Return $str
	$b = GetAttr($str, "Bounds")
	$arr = StringSplit($b, ",")
	if $arr[0] <> 4 Then Return $str & "~~~ERROR in BOUNDS~~~"
	$ret = ReplaceAttr($str, "Bounds", $WW - StringStripWS($arr[2], 8) - StringStripWS($arr[4], 8) + $dX & ", " & StringStripWS($arr[1], 8) + $dY & ", " & StringStripWS($arr[4], 8) & ", " & StringStripWS($arr[3], 8))
	Return $ret
EndFunc

Func left_top($str)
	$Align = GetAttr($str, "Align")
	$ret = $str
	Switch $Align
	Case ""
		Return $ret
	Case "Left"
		$ret = ReplaceAttr($ret, "Align", "Top")
	Case "Top"
		$ret = ReplaceAttr($ret, "Align", "Right")
	Case "Right"
		$ret = ReplaceAttr($ret, "Align", "Bottom")
	Case "Bottom"
		$ret = ReplaceAttr($ret, "Align", "Left")
	EndSwitch
	Return $str
EndFunc

Func ParseStr($str)
	$ret = $str
	if $flip Then
		$ret = Width_to_Height($ret)
		$ret = flip_n_rot($ret)
		$ret = left_top($ret)
	EndIf
	Return $ret
EndFunc

Func AddButtons()
	$btnFileName = FileOpenDialog("Select button file", @ScriptDir & "\", "XML Files (*.xml)", 1) ;
	if $btnFileName = "" Then Return
	$btnFileH = FileOpen($btnFileName, 0)
	do 
		$str = FileReadLine($btnFileH)
		$ee = (@error = -1)
		Write($str, True)
	Until $ee
	FileClose($btnFileH)

EndFunc

Func Read()
	$str = ""
	if Not $EOinF Then $str = FileReadLine($inFile)
	$EOinF = (@error = -1)
	Return $str
EndFunc

Func Write($newStr, $andInConsole)
	if $newStr <> "" Then
		FileWriteLine($outFile, $newStr)
		if $andInConsole then ConsoleWrite($newStr & @CRLF)
	EndIf
EndFunc


ProgInit()
Do
	$str = Read()
	$newStr = ParseStr($str)
	if (GetElementName($str) = "Level") and (GetTagType($str) = $cTT_End) Then AddButtons()
	Write($newStr, False)
Until $EOinF
Write("<!-- flipped -->", False)
ProgClose()



$wrkDr = IniRead(@ScriptDir & "\ini.ini", "main", "wrkdr", "/\NODIR/\")
if FileExists($wrkDr) Then
	if MsgBox(33, "Copy", 'Copy file ' & @CRLF & $outFileName & @CRLF & 'to ' & $wrkDr & '?') = 1 Then
		FileCopy($outFileName, $wrkDr, 1)
	EndIf
EndIf


