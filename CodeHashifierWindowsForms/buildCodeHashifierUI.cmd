@echo off
echo static internal class TimeStamp {public static readonly string BuildTime = "%DATE% %TIME%".Substring(0, 19); public static readonly string BuildYear = "%DATE%".Substring(6);}> TimeStamp.cs
type CodeHashifierUI.cs TimeStamp.cs >BuildObject.cs 2>nul
call "%PROGRAMFILES%\Mono\bin\mcs" BuildObject.cs -out:CodeHashifierUI.exe -target:winexe -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:System.Data.dll -win32icon:CodeHashifier.ico
@echo off
del /S /Q TimeStamp.cs >nul 2>&1
del /S /Q BuildObject.cs >nul 2>&1