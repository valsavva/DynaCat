Global $m_LogFileName = "log.log"
Global $m_LogFileNameH = -1
Global Const $m_ReadMode = 0
Global Const $m_AppendMode = 1
Global Const $m_ReWriteMode = 2

Func m_LogStart($newName, $mode)
Local $err
	if $newName <> "" then $m_LogFileName = $newName
	$m_LogFileNameH = FileOpen(@ScriptDir & "\" & $m_LogFileName, $mode)
	if $m_LogFileNameH <> -1 Then
		$err = FileWriteLine($m_LogFileNameH, "-----------------------")
		$err = FileWriteLine($m_LogFileNameH, "Log started at " & @MDAY & "/" & @MON & " " & @YEAR & "   " & @HOUR & ":" & @MIN & ":" & @SEC)
		$err = FileWriteLine($m_LogFileNameH, "-----------------------")
	EndIf
	if $err = 0 then $m_LogFileNameH = -1
	Return $m_LogFileNameH ; -1 if error
EndFunc

Func m_LogAdd($newLine)
Local $err
	if $newLine = "" then return -1
	if $m_LogFileNameH = -1 then return -1
	$err = FileWriteLine($m_LogFileNameH, ">" & @HOUR & ":" & @MIN & ":" & @SEC & " " & ">" & " " & $newLine)
	if $err = 0 then $err = -1
	Return $err ; -1 if error
EndFunc

Func m_LogAddAndToolTip($newLine)
Local $err
	if $newLine = "" then return -1
	if $m_LogFileNameH = -1 then return -1
	ToolTip($newLine, 20, 0)
	$err = FileWriteLine($m_LogFileNameH, ">" & @HOUR & ":" & @MIN & ":" & @SEC & " " & ">" & " " & $newLine)
	if $err = 0 then $err = -1
	Return $err ; -1 if error
EndFunc

Func m_LogClose()
Local $err
	if $m_LogFileNameH = -1 then return -1
	$err = FileWriteLine($m_LogFileNameH, "-----------------------")
	$err = FileWriteLine($m_LogFileNameH, "Log ended at " & @MDAY & "/" & @MON & " " & @YEAR & "   " & @HOUR & ":" & @MIN & ":" & @SEC)
	$err = FileWriteLine($m_LogFileNameH, "-----------------------")
	$err = FileClose($m_LogFileNameH)
	if $err = 0 then $err = -1
	Return $err ; -1 if error
EndFunc
