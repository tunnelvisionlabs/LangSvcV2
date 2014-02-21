$BuildConfig = 'Release'
$Version = '1.0.0-alpha001'

if (!(Test-Path .\bin\nuget)) {
	mkdir .\bin\nuget
}

..\.nuget\NuGet.exe pack .\Tvl.Java.nuspec -NoDefaultExcludes -OutputDirectory bin\nuget -Prop Configuration=$BuildConfig -Version $Version
