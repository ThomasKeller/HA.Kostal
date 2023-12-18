var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./";
var serviceFolder = "./src/HA.Kostal.Service";
var publishFolder = "../../Releases/nuget"
var outputFolder = "./artifacts";

Task("Clean")
    .Does(() => {
        CleanDirectory(outputFolder);
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() => {
        DotNetRestore(solutionFolder);
  });

Task("Build") 
    .IsDependentOn("Restore")
    .Does(() => {
        DotNetBuild(solutionFolder, new DotNetBuildSettings {
            NoRestore = true,
            Configuration = configuration
        });
    });

Task("Test") 
    .IsDependentOn("Build")
    .Does(() => {
        DotNetTest(solutionFolder, new DotNetTestSettings {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration
        });
    });

Task("Publish") 
    .IsDependentOn("Build")
    .Does(() => {
        DotNetPublish(serviceFolder, new DotNetPublishSettings {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
			OutputDirectory = publishFolder
        });
    });

Task("Pack") 
    .IsDependentOn("Build")
    .Does(() => {
        DotNetPack(serviceFolder, new DotNetPackSettings {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
			OutputDirectory = outputFolder
        });
    });


RunTarget(target);