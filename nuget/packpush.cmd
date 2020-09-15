dotnet pack ../ -c Release -o ../nuget
pause
nuget push *.nupkg oy2d5crn3nw5t5gu2w7q6dekmuq65ktehmje54q4oif264 -src https://www.nuget.org/
del *.nupkg /f /q
pause