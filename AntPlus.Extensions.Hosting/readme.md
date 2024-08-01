## Small Earth Technology ANT+ Hosting Extensions
If you use dependency injection to compose your apps, check this extension out! With a few lines added to the DI container/host builder, the
complete library is ready to be used by the app. Here's a snippet from the WPF sample -
```csharp
    // dependency services
    _host = Host.CreateDefaultBuilder(Environment.GetCommandLineArgs()).
        UseSerilog().
        UseAntPlus().   // this adds all the required dependencies to use the ANT+ class library
        ConfigureServices(services =>
        {
            // add the implementation of IAntRadio to the host
            services.AddSingleton<IAntRadio, AntRadio>();
        }).
        Build();
        .
        .
        .
    // create the device collection - this starts scanning for devices
    AntDevices = _host.Services.GetRequiredService<AntCollection>();
```
MAUI apps can use `UseMauiAntPlus()` to add the dependencies to `MauiApp.CreateBuilder()`.

An ==important== benefit of using dependency injection is that you can now add ANT devices not supported by the current
ANT+ Class Library. A detailed guide on how to do this available in the documentation. You can also set the global
ANT device timeout from a variety of sources such as the command line, appsettings.json, etc. and let the DI container
inject them into the ANT+ Class Library.

##### Addtional Links
* [Documentation](https://stephenhidem.github.io/AntPlus/html/40537603-f0ed-f134-8e97-bd58d1d8b17d.htm)
* [Examples](https://github.com/StephenHidem/AntPlus/tree/master/Examples) - MAUI-gRPC and WpfUsbStickApp examples.
* [Issues](https://github.com/StephenHidem/AntPlus/issues) - Mention AntPlus class library in the issue title.
* [Discussions](https://github.com/StephenHidem/AntPlus/discussions) - Post questions and join discussions.

