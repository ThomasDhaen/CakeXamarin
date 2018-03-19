#addin "Cake.Npm"
#tool nuget:?package=NUnit.Runners&version=2.6.4

var solutionFile = GetFiles("./*.sln").First();
var testCsproj = GetFiles("./CakeXamarinTest/*.csproj").First();
var testDll = "./CakeXamarinTest/bin/Debug/CakeXamarinTest.dll";
var target = Argument("target", "Default");
var appCenterToken = Argument("appCenterToken", EnvironmentVariable("APP_CENTER_TOKEN") ?? string.Empty);

var artifactsPath = MakeAbsolute(Directory("./artifacts"));
string ipaPath = $"{artifactsPath}/CakeXamarin.iOS.ipa";

Task("InstallAppcenter")
    .Does(() => 
        {
        var settings = new NpmInstallSettings
        {
        Global = true
        };
        settings.AddPackage("appcenter-cli");
        NpmInstall(settings);
});

Task("CleanObjBin")
    .Does(() =>
{
    CleanDirectory(artifactsPath);
    CleanDirectories("./**/bin");
    CleanDirectories("./**/obj");
    Information("Finished cleaning bin and obj folders.");
});

Task("Restore")
    .Does(() =>
{
    Information($"Restoring NuGet packages for {solutionFile}");
    NuGetRestore(solutionFile);
});

Task("BuildTest")
    .Does(() =>
{
    MSBuild(testCsproj);
});

Task("RunTest")
    .Does(() =>
{
    NUnit(testDll);
});


Task("BuildCore")
    .Does(() =>
{
    MSBuild(solutionFile, conf =>
            { conf
                .SetConfiguration("Release")
                .WithTarget($@"CakeXamarin");
            });
});

Task("BuildIOS")
    .Does(() =>
{
    EnsureDirectoryExists(artifactsPath);
    MSBuild(solutionFile, conf =>
            { conf
                .SetConfiguration("Release")
                .WithProperty("Platform", "iPhone")
                .WithProperty("IpaPackageDir", @"..\\artifacts")
                .WithProperty("BuildIpa", "true")
                .WithTarget($@"CakeXamarin_iOS");
            });   
});

Task("Deploy")
    .Does(() =>
{
     var uploadiOSResult = StartProcess("appcenter", $"distribute release --file {ipaPath} --app ThomasDhaen/CakeXamarinIOS --group Collaborators  --token {appCenterToken}");
});

Task("Clean")
    .IsDependentOn("InstallAppcenter")
    .IsDependentOn("CleanObjBin")
    .IsDependentOn("Restore");

Task("BuildAll")
    .IsDependentOn("BuildCore")
    .IsDependentOn("BuildIOS");  

Task("Test")
    .IsDependentOn("BuildTest")
    .IsDependentOn("RunTest");     

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("BuildAll")
    .IsDependentOn("Test")
    .IsDependentOn("Deploy");

RunTarget(target);