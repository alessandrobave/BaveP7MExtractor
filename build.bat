@echo off
rem Build portable single-file executable
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
echo Build completed. Check the ./publish folder for P7MExtractor.exe
pause
