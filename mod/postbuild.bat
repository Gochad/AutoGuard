@echo off
Xcopy /E /C /H /I /Y /EXCLUDE:postbuildmoveignore.txt %1 %2
