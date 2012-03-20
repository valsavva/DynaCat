#include "Array.au3"
#include <Constants.au3>


Global $EOinF = False, $inFileName, $outFileName, $inFile, $outFile
Const $cClosedTag = "ClosedTag"
Const $cClosedElem = "ClosedElem"
Global $cKeys = ""
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
		$cKeys = $cKeys & "|" & $arr[$i]
	Next
	
	$cDecimalplaces = IniRead(@ScriptDir & "\ini.ini", "main", "Decimalplaces", 2)
EndFunc


Func ProgClose()
	FileClose($inFile)
	FileClose($outFile)
EndFunc

Func VerifySVG()
Local $tt, $i = 1, $name, $ErrorList = "", $teleports = ""
	Do
		$str = Read()
		if GetAttr($str, "transform") <> "" then 
			$el = GetElementName($str, $tt)
			if $el = "g" then 
				$el = "layer(group)"
				$name = GetAttr($str, "inkscape:label")
				if $name = "" Then $name = GetAttr($str, "id")
			Else
				$name = "id=" & GetAttr($str, "id")
			EndIf
			$ErrorList = $ErrorList & 'Error in SVG file: ' & $el & ' "' & $name & '" has been transformed (line #' & $i & ')' & @CRLF
		EndIf
		
		$el = GetElementName($str, $tt)
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
		;if GetElementName($str, $tt) = "Layer" and Not StrIsClosedTag($str) Then Write(@CRLF)
		if StrIsClosedTag($str) then 
			$offset = StringReplace($offset, "  ", "", 1)
		Else
			if not ElemIsClosed($str) Then 
				$cur = "  "
			EndIf
		EndIf
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

Func StrIsClosedTag($str)
	Return (StringMid($str, 2, 1) = "/")
EndFunc

Func ElemIsClosed($str)
	Return (StringMid($str, StringLen($str)-1, 1) = "/")
EndFunc

Func GetElementName($str, ByRef $tagType)
	if StrIsClosedTag($str) then $tagType = $cClosedTag
	if ElemIsClosed($str) then $tagType = $cClosedElem
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
	if $tagType = $cClosedTag then Return "</" & IniRead(@ScriptDir & "\dict.ini", $el, "ElementName", "# ERROR TranslateElementTag 001 #") & ">"
		
	$newEl = IniRead(@ScriptDir & "\dict.ini", $el, "ElementName", "# ERROR TranslateElementTag 002 #")
	if $newEl = "Layer" and ElemIsClosed($str) then Return "";delete empty layer (<Layer bla bla />)
		
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
			$retStr = $retStr & ' ' & 'BackColor="#FFFFFFFF"'
			$FirstLayer = False
		Else
			$retStr = $retStr & ' ' & 'BackColor="#00000000"'
		EndIf
	EndIf
	
	if $tagType = $cClosedElem then $retStr = $retStr & "/"
	$retStr = $retStr & ">"
	
	Return $retStr
EndFunc

Func GetObjElemNameFromPath($fullPath)
	$i = StringInStr($fullPath, "Obj/")
	if $i = 0 then $i = StringInStr($fullPath, "Obj\")
	if $i = 0 then Return ""
	Return StringUpper(StringMid($fullPath, $i + 4, 1)) & StringMid($fullPath, $i + 5, StringLen($fullPath) - 8 - $i)
EndFunc

Func GetParentDir($fullPath)
	$arr = StringSplit($fullPath, "/")
	if $arr[0] = 1 then $arr = StringSplit($fullPath, "\")
	if $arr[0] = 1 Then Return ""
	Return $arr[$arr[0]-1]
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
	$tt = ""
	Do
		$str = Read()
		if GetElementName($str, $tt) = "desc" Then $ret = GetValue($str)
	Until $EOinF or $ret <> ""
	
	$ret = StringReplace($ret, '&quot;', '"')
	
	PassClosedElem("image")
	
	Return $ret
EndFunc

Func PassClosedElem($el)
	$tt = ""
	Do
		$str = Read()
	Until $EOinF or ((GetElementName($str, $tt) = $el) and (StrIsClosedTag($str)))
EndFunc

Func translateChar($ch, $align, $defEdge, $EdgeType, $desc)
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
	$pf = GetParentDir($fullPath)
	$fn = GetFileName($fullPath)
	$childs = ""
	
	$el = ""
	if $pf = "Block" then
		$cntBlock = $cntBlock + 1
		$el = "Block"
	EndIf
	if $pf = "Design" then 
		$cntDesign = $cntDesign + 1
		$el = "Image"
	EndIf
	if $pf= "Obj" then 
		$cntObj = $cntObj + 1
		$el = GetObjElemNameFromPath($fullPath)
	EndIf
	
	$ret = "<" & $el & ' Bounds="' & Round(GetAttr($str, "x")) & ', ' & Round(GetAttr($str, "y")) & ', ' & Round(GetAttr($str, "width")) & ', ' & Round(GetAttr($str, "height")) & '"'
	
	$desc = ""
	if (not StrIsClosedTag($str)) and (not ElemIsClosed($str)) Then
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
		if @extended > 2 Then 
			$defEdge = "Stick"
			$EdgeType = "Bounce"
		Else
			$defEdge = "Bounce"
			$EdgeType = "Stick"			
		EndIf
		;Write("<!--" & $fn & "-->" & @CRLF)
		$ret = $ret & ' DefaultEdge="' & $defEdge & '"'
		$chl = translateChar(StringMid($fn, 1, 1), "Top", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
		$chl = translateChar(StringMid($fn, 2, 1), "Right", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
		$chl = translateChar(StringMid($fn, 3, 1), "Bottom", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
		$chl = translateChar(StringMid($fn, 4, 1), "Left", $defEdge, $EdgeType, $desc)
		if $chl <> "" then $childs = $childs & @CRLF & $chl
	Case "Obj"
	Case "Design"
		$opa = Round(GetSubAttr($str, "style", "opacity"), $cDecimalplaces)
		If $opa = "" Then $opa = 1
		$ret = $ret & ' Source="' & $pf & "\" & $fn & '.' & GetExt($fullPath) & '" Opacity="' & $opa & '"'
		
	EndSwitch
	
	if $childs = "" Then 
		$ret = $ret & "/>"
	Else
		$ret = $ret & ">" & $childs & @CRLF & "</" & $el & ">"
	EndIf
	
	Return $ret
EndFunc

Func ParseStr($str)
	$tagType = ""
	$el = GetElementName($str, $tagType)
	If StringInStr($cKeys, "|" & $el) then Return TranslateElementTag($str, $el, $tagType)
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

if MsgBox(33, "Copy", "Copy file to work dir?") = 1 Then
	$wrkDr = IniRead(@ScriptDir & "\ini.ini", "main", "wrkdr", "/\NODIR/\")
	if FileExists($wrkDr) Then
		FileCopy($outFileName, $wrkDr, 1)
	EndIf
EndIf