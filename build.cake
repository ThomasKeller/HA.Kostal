var target = Argument("target", "Build");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./";
var outputFolder = "./artifacts";

Task("Clean")
    .Does(() => {
        CleanDirectory("./build");
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
        DotNetPublish(solutionFolder, new DotNetPublishSettings {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
			OutputDirectory = outputFolder
        });
    });



RunTarget(target);