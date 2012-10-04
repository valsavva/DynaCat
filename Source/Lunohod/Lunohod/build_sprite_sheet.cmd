@echo off
rem %1 - file mask
rem %2 - output name

echo building %2


"C:\Program Files (x86)\CodeAndWeb\TexturePacker\bin\TexturePacker.exe" --allow-free-size --max-size 1024 --data %2.xml --format xml --algorithm MaxRects --maxrects-heuristics area --no-trim --disable-rotation --sheet %2.png %1
