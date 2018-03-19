#tool nuget:?package=NUnit.Runners&version=2.6.4

var solutionFile = GetFiles("./*.sln").First();
var testCsproj = GetFiles("./CakeXamarinTest/*.csproj").First();
var testDll = "./CakeXamarinTest/bin/Debug/CakeXamarinTest.dll";
var coreCsproj = GetFiles("./CakeXamarin/*.csproj").First();
var androidCsproj = GetFiles("./Droid/*.csproj").First();
var iOSCsproj = GetFiles("./iOS/*.csproj").First();
var target = Argument("target", "Default");

Task("CleanObjBin")
    .Does(() =>
{
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    Information("Finished cleaning bin and obj folders.");
});

Task("Restore")
    .Does(() =>
{
    Information("Restoring NuGet packages for {0}", solutionFile);
    NuGetRestore(solutionFile);
});

Task("BuildTest")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(testCsproj);
});

Task("RunTest")
    .IsDependentOn("BuildTest")
    .Does(() =>
{
    NUnit(testDll);
});

Task("BuildCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(coreCsproj);
});

Task("BuildAndroid")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(androidCsproj);
});

Task("BuildIOS")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(iOSCsproj);
});

Task("BuildAll")
    .IsDependentOn("BuildTest")
    .IsDependentOn("BuildCore")
    .IsDependentOn("BuildAndroid")
    .IsDependentOn("BuildIOS");

Task("Clean")
    .IsDependentOn("CleanObjBin")
    .IsDependentOn("Restore");

Task("Default")
    .IsDependentOn("BuildAll");

RunTarget(target);