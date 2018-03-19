#addin "Cake.Npm"
#tool nuget:?package=NUnit.Runners&version=2.6.4

var solutionFile = GetFiles("./*.sln").First();
var testCsproj = GetFiles("./CakeXamarinTest/*.csproj").First();
var testDll = "./CakeXamarinTest/bin/Debug/CakeXamarinTest.dll";
var coreCsproj = GetFiles("./CakeXamarin/*.csproj").First();
var androidCsproj = GetFiles("./Droid/*.csproj").First();
var iOSCsproj = GetFiles("./iOS/*.csproj").First();
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
    .IsDependentOn("InstallAppcenter")
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



Task("BuildAndroid")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(androidCsproj);
});

Task("BuildCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild(solutionFile, conf =>
            { conf
                .SetConfiguration("Release")
                .WithTarget($@"CakeXamarin");
            });
});

Task("BuildIOS")
    .IsDependentOn("BuildCore")
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

Task("DeployIOS")
    .IsDependentOn("BuildIOS")
    .Does(() =>
{
     var uploadiOSResult = StartProcess("appcenter", $"distribute release --file {ipaPath} --app ThomasDhaen/CakeXamarinIOS --group Collaborators  --token {appCenterToken}");
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
    .IsDependentOn("DeployIOS");

// Task("BuildAndDeploy")
//     .IsDependentOn("InstallAppcenter")
//     .IsDependentOn("BuildAll")
//     .IsDependentOn("RunUnitTests")
//     .Does(() => 
//     {
//         try
//         {
//             var uploadiOSResult = StartProcess("appcenter", $"distribute release --file {ipaPath} --app {appCenteriOSApp} --group {appCenterGroup} --token {appCenterToken}");
//         }
//         catch (System.Exception exc)
//         {
//             LogExceptionToSlack($"Build {buildId} failed during deploy to App Center", exc);
//             throw;
//         }
//     });

RunTarget(target);