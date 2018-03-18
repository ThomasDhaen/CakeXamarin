#tool nuget:?package=NUnit.Runners&version=2.6.4

var solutionFile = GetFiles("./*.sln").First();
var testCsproj = GetFiles("./CakeXamarinTest/*.csproj").First();
var testDll = "./CakeXamarinTest/bin/Debug/CakeXamarinTest.dll";
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

Task("Clean")
    .IsDependentOn("CleanObjBin")
    .IsDependentOn("Restore");

Task("Default")
    .IsDependentOn("RunTest");

RunTarget(target);