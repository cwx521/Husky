dotnet pack ../ -c Release -o ../nuget
pause
nuget push *.nupkg [key] -src https://www.nuget.org/
del *.nupkg /f /q
pause