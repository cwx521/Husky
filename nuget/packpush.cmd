dotnet pack ../ -c Release -o ../../nuget
nuget push *.nupkg 178079be-7c67-4f49-914c-e11c6f502336 -src https://www.nuget.org/
del *.nupkg /f /q
pause