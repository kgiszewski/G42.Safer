"NugetHelper.exe" dist/lib/net471/G42.Safer.Xss.dll Package.nuspec
del package\*.* /F /Q
"nuget.exe" pack package.nuspec -OutputDirectory Package -BasePath dist
pause