// ARGUMENTS
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");

// GLOBAL VARIABLES
var sourcePath  = Directory("./src");

// TASKS
Task("Restore")
.Does(() =>
{
	var projects = GetFiles("./**/project.json");

	foreach(var project in projects)
	{
        var settings = new DotNetCoreRestoreSettings
        {
            Sources = new [] { "https://api.nuget.org/v3/index.json" }
        };
        
	    DotNetCoreRestore(project.GetDirectory().FullPath, settings);
    }
});

Task("Build")
.IsDependentOn("Restore")
.Does(() =>
{
	var projects = GetFiles("./**/project.json");

	foreach(var project in projects)
	{
        var settings = new DotNetCoreBuildSettings 
        {
            Configuration = configuration
        };

	    DotNetCoreBuild(project.GetDirectory().FullPath, settings);
    }
});

Task("Unit-Tests")
.Does(() =>
{
    var projects = GetFiles("./src/*.*Tests/**/project.json");

    foreach(var project in projects)
	{
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration
        };

        Information(project.FullPath);

        DotNetCoreTest(project.GetDirectory().FullPath, settings);
    }
});

// DEPENDENCIES
Task("Default")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.IsDependentOn("Unit-Tests");

// EXECUTE
RunTarget(target);