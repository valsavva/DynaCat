;SVG to level XML 0.3
;<Element Attribute="SubAttribute:1111;">Value</Element>

#include "Array.au3"
#include <Constants.au3>


Global $EOinF = False, $inFileName, $outFileName, $inFile, $outFile

;Tag Types
Const $cTT_Start = "Tag Type: Start"
Const $cTT_Single = "Tag Type: Single"
Const $cTT_End = "Tag Type: End"

Global $dictElems = ""
Global $passElems = ""
Global $GrfxTypes = "", $GrfxTypesArr
Global $FirstLayer = True
Global $cntObj = 0, $cntBlock = 0, $cntDesign = 0
Global $cDecimalplaces

Func ProgInit()
	
	if $CmdLine[0] <> 0 Then
		$inFileName = $CmdLine[1]
		if (not FileExists($inFileName)) or (GetExt($inFileName) <> "svg") Then 
			$inFileName = FileOpenDialog("Select input file", @ScriptDir & "\", "SVG Files (*.svg)", 1) ;
		EndIf
	Else
			$inFileName = FileOpenDialog("Select input file", @ScriptDir & "\", "SVG Files (*.svg)", 1) ;
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
		
	$arr = IniReadSectionNames(@ScriptDir & "\dict.ini")

	for $i = 1 to $arr[0]
		$dictElems = $dictElems & "|" & $arr[$i]
	Next
	
	$passElems = IniRead(@ScriptDir & "\ini.ini", "main", "passElems", "")
	$GrfxTypes = IniRead(@ScriptDir & "\ini.ini", "main", "GrfxTypes", "")
	$GrfxTypesArr = StringSplit($GrfxTypes, "|")
	$cDecimalplaces = IniRead(@ScriptDir & "\ini.ini", "main", "Decimalplaces", 2)
EndFunc


Func ProgClose()
	FileClose($inFile)
	FileClose($outFile)
EndFunc

Func VerifySVG()
Local $i = 1, $name, $ErrorList = "", $teleports = ""
	Do
		do 
			$passed = False
			$str = Read()
			$el = GetElementName($str)
			if ($el <> "") and (StringInStr($passElems, "|" & $el)) and (GetTagType($str) = $cTT_Start) then 
				;MsgBox(0, "", $el)
				PassElem($el)
				$passed = True
			EndIf
		Until not $passed
		
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
		
		$el = GetElementName($str)
		if $el = "g" then 
			if StringInStr(GetAttr($str, "id"), "g") Then $ErrorList = $ErrorList & 'Error in SVG file: some elements are grouped (line #' & $i & ') - its should be ungrouped' & @CRLF
		EndIf

		
		if StringInStr($str, "__Teleport__Id") > 0 Then
			;MsgBox(0, "", $str)
			$desc = '"' & GetAttr(GetValue($str), '__Teleport__Id') & '"'
			$desc = StringReplace($desc, '&quot;', '"')
			if (not StringInStr($teleports, $desc)) Then 
				$teleports = $teleports & "   " & $desc
			Else
				$teleports = StringReplace($teleports, $desc, "")
			EndIf
		EndIf
		
		$i = $i + 1
	Until $EOinF
	
	if not StringIsSpace($teleports) And not $teleports = "" Then $ErrorList = $ErrorList & "Teleports are not matched: " & $teleports & @CRLF
		
	if $ErrorList <> "" Then
		if MsgBox(49, 'Errors in SVG', $ErrorList & @CRLF & @CRLF & "Ignore and continue?" ) <> 1 Then Exit
	EndIf
	
	FileSetPos($inFile, 0, $FILE_BEGIN)

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
		if GetTagType($str) = $cTT_End then $offset = StringReplace($offset, "  ", "", 1)
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

Func WipeStr($str)
	Return StringStripWS($str, 7)
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

Func GetSubAttr($str, $attr, $subAttr)
	$attrStr = GetAttr($str, $attr)
	$arr = StringSplit($attrStr, ';')
	for $i = 0 to $arr[0]
		if StringLeft($arr[$i], 7) = $subAttr then 
			$arr2 = StringSplit($arr[$i], ':')
			if $arr2[0] = 1 then Return ""
			Return $arr2[2]
		EndIf
	next
	Return ""	
EndFunc

Func TranslateElementTag($str, $el, $tagType)
	if $el = "" then Return ""
	if $tagType = $cTT_End then Return "</" & IniRead(@ScriptDir & "\dict.ini", $el, "ElementName", "# ERROR TranslateElementTag 001 #") & ">"
		
	$newEl = IniRead(@ScriptDir & "\dict.ini", $el, "ElementName", "# ERROR TranslateElementTag 002 #")
	if $newEl = "Layer" and (GetTagType($str) = $cTT_Single) then Return "";delete empty layer (<Layer bla bla />)
		
	$retStr = "<" & $newEl
	
	$arr = IniReadSection(@ScriptDir & "\dict.ini", $el)
	if $arr[0][0] > 1 then 
		for $i = 1 to $arr[0][0]
			$attr = GetAttr($str, $arr[$i][0])
			if $arr[$i][0] = "width" or $arr[$i][0] = "height" then $attr = Round($attr)
			if $attr <> "" Then
				$retStr = $retStr & ' ' & $arr[$i][1] & '="' & $attr & '"'
			EndIf
		Next
	EndIf

	if $newEl = "Layer" Then
		;Write(" ")
		if $FirstLayer then  
			$retStr = $retStr & ' ' & 'BackColor="' & IniRead(@ScriptDir & "\ini.ini", "main", "FirstLayerBckColor", "#FFFFFFFF") & '"'
			$FirstLayer = False
		Else
			$retStr = $retStr & ' ' & 'BackColor="' & IniRead(@ScriptDir & "\ini.ini", "main", "OtherLayerBckColor", "#00000000") & '"'
		EndIf
	EndIf
	
	$retStr = $retStr & '  SvgId="' & GetAttr($str, "id") & '"'
	
	if $tagType = $cTT_Single then $retStr = $retStr & "/"
	$retStr = $retStr & ">"
	
	Return $retStr
EndFunc

Func GetObjElemNameFromPath($fullPath)
	$i = StringInStr($fullPath, "Obj/")
	if $i = 0 then $i = StringInStr($fullPath, "Obj\")
	if $i = 0 then Return ""
	Return StringUpper(StringMid($fullPath, $i + 4, 1)) & StringMid($fullPath, $i + 5, StringLen($fullPath) - 8 - $i)
EndFunc

#cs
Func GetParentDir($fullPath)
	$arr = StringSplit($fullPath, "/")
	if $arr[0] = 1 then $arr = StringSplit($fullPath, "\")
	if $arr[0] = 1 Then Return ""
	Return $arr[$arr[0]-1]
EndFunc
#ce

Func GetGrfxType($fullPath)
	$arr = StringSplit($fullPath, "/")
	if $arr[0] = 1 then $arr = StringSplit($fullPath, "\")
	if $arr[0] = 1 Then Return ""
	
	For $i = 1 to $GrfxTypesArr[0]
		if StringInStr($fullPath, "/" & $GrfxTypesArr[$i]) Then Return $GrfxTypesArr[$i]
	Next
	
	Return ""
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

Func GetDescriptions()
	$ret = ""
	Do
		$str = Read()
		if GetElementName($str) = "desc" Then $ret = GetValue($str)
	Until $EOinF or $ret <> ""
	
	$ret = StringReplace($ret, '&quot;', '"')
	
	PassElem("image")
	
	Return $ret
EndFunc

Func PassElem($el)
	Do
		$str = Read()
	Until $EOinF or ((GetElementName($str) = $el) and (GetTagType($str) = $cTT_End))
EndFunc

Func translateBlockChars($ch, $align, $defEdge, $EdgeType, $desc)
	if $ch = StringLeft($defEdge, 1) Then Return ""
	$ret = ""
	$el = ""
	if $ch = StringLeft($EdgeType, 1) then 
		$el = "Edge"
		$ret = '<Edge Type="' & $EdgeType & '"'
	EndIf
	if $ch = 'T' then 
		$el = "Teleport"
		$ret = "<Teleport"
	EndIf
		
	$ret = $ret & ' Align="' & $align & '"'
	$childDesc = "__" & $el & "__"
	if StringLeft($desc, stringlen($childDesc)) = $childDesc Then $ret = $ret & " " & StringRight($desc, StringLen($desc) - stringlen($childDesc))
	$ret = $ret & "/>"
	Return $ret
	
EndFunc

Func TranslateImageTag($str, $tagType)
	$fullPath = GetAttr($str, "xlink:href")
	$tagType = ""
	$pf = GetGrfxType($fullPath)
	$fn = GetFileName($fullPath)
	$childs = ""
	
	$el = ""
	if $pf = "Block" then
		$cntBlock = $cntBlock + 1
		$el = "Block"
	EndIf
	if $pf = "Design" or $pf = "Texture" then 
		$cntDesign = $cntDesign + 1
		$el = "Image"
	EndIf
	if $pf= "Obj" then 
		$cntObj = $cntObj + 1
		$el = GetObjElemNameFromPath($fullPath)
	EndIf
	
	$ret = "<" & $el & ' Bounds="' & Round(GetAttr($str, "x")) & ', ' & Round(GetAttr($str, "y")) & ', ' & Round(GetAttr($str, "width")) & ', ' & Round(GetAttr($str, "height")) & '"'
	
	$desc = ""
	if (GetTagType($str) = $cTT_Start) Then
		$desc = GetDescriptions()
		if StringLeft($desc, 2) <> "__" Then
			$ret = $ret & " " & $desc
		EndIf
	EndIf
	
	if $desc = "" Then
		$desc = IniRead(@ScriptDir & "\ini.ini", "default_attributes", $el, "")
		if $desc <> "" then $ret = $ret & " " & $desc
	EndIf
	
	Switch $pf
	Case "Block"
		StringReplace($fn, "S", "X")
		$cntS = @extended
		StringReplace($fn, "B", "X")
		$cntB = @extended
		if $cntS > $cntB Then 
			$defEdge = "Stick"
			$EdgeType = "Bounce"
		Else
			$defEdge = "Bounce"
			$EdgeType = "Stick"			
		EndIf
		$ret = $ret & ' DefaultEdge="' & $defEdge & '"'
		$chl = translateBlockChars(StringMid($fn, 1, 1), "Top", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
		$chl = translateBlockChars(StringMid($fn, 2, 1), "Right", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
		$chl = translateBlockChars(StringMid($fn, 3, 1), "Bottom", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
		$chl = translateBlockChars(StringMid($fn, 4, 1), "Left", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
	Case "Obj"
	Case "Design" or "Texture"
		$opacity = Round(GetSubAttr($str, "style", "opacity"), $cDecimalplaces)
		If $opacity = "" Then $opacity = 1
		$ret = $ret & ' Source="' & $pf & "\" & $fn & '.' & GetExt($fullPath) & '" Opacity="' & $opacity & '"'
	EndSwitch
	
	$ret = $ret & '  SvgId="' & GetAttr($str, "id") & '"'
	
	if $childs = "" Then 
		$ret = $ret & "/>"
	Else
		$ret = $ret & ">" & $childs & @CRLF & "</" & $el & ">"
	EndIf
	
	Return $ret
EndFunc

Func ParseStr($str)
	$el = GetElementName($str)
	$tagType = GetTagType($str)
	if ($el <> "") and (StringInStr($passElems, "|" & $el)) and ($tagType = $cTT_Start) then 
		PassElem($el)
		Return ""
	EndIf
	If StringInStr($dictElems, "|" & $el) then Return TranslateElementTag($str, $el, $tagType)
	If $el = "image" then Return TranslateImageTag($str, $tagType)
	Return ""
EndFunc


Func Read()
	$str = ""
	if Not $EOinF Then $str = FileReadLine($inFile)
	$EOinF = (@error = -1)
	$str = WipeStr($str)
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
Do
	$str = Read()
	$newStr = ParseStr($str)
	Write($newStr, False)
Until $EOinF
Write("<!-- Action objects:" & $cntObj & "; Graphx design elements:" & $cntDesign & "; Blocks:" & $cntBlock & " -->", False)
ProgClose()
FormatFile($outFileName)

$wrkDr = IniRead(@ScriptDir & "\ini.ini", "main", "wrkdr", "/\NODIR/\")
if FileExists($wrkDr) Then
	if MsgBox(33, "Copy", 'Copy file ' & @CRLF & $outFileName & @CRLF & 'to ' & $wrkDr & '?') = 1 Then
		FileCopy($outFileName, $wrkDr, 1)
	EndIf
EndIf


