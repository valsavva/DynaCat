@echo off
rem %1 - file mask
rem %2 - output name

echo building %2

dir %1 /B /S > dir.tmp
..\..\ThirtParty\SpriteSheetPacker\sspack.exe /image:%2.png /map:%2.txt /il:dir.tmp /pad:0 /mw:1024 /mh:1024
del dir.tmp