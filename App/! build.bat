@echo off
echo Wait for build...
dotnet publish
del bin\Debug\net6.0\win-x86\publish\Splash.pdb
start explorer.exe bin\Debug\net6.0\win-x86\publish