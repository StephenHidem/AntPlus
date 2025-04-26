# Small Earth Technology ANT+ Hosting Extensions
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
MAUI apps can also invoke `UseAntPlus()` to add the dependencies to `MauiApp.CreateBuilder()`.

An ==important== benefit of using dependency injection is that you can now add ANT devices not supported by the current
ANT+ Class Library. A detailed guide on how to do this is available in the [documentation](https://stephenhidem.github.io/AntPlus/html/0e424769-d4c6-4a98-9384-a810421ad5e1.htm). You can also set the global
ANT device timeout from a variety of sources such as the command line, appsettings.json, etc. and let the DI container
inject them into the ANT+ Class Library.
## Open Source Maintenance Fee
This project requires an [Open Source Maintenance
Fee][osmf]. While the source code is
freely available under the terms of the [LICENSE][license], all other aspects of
the project--including participating in
discussions and downloading releases--require [adherence to the
Maintenance Fee][eula].

In short, if you use this project to generate revenue, the [Maintenance
Fee is required][eula]. Note that a portion of the fee sponsors other open source projects that this project uses.

To pay the Maintenance Fee, [become a Sponsor](https://github.com/sponsors/StephenHidem).
## Addtional Links
* [Documentation](https://stephenhidem.github.io/AntPlus/html/5e5a5e1c-a0e6-4ef0-a8f5-12f9394450c4.htm)
* [Examples Overview](https://stephenhidem.github.io/AntPlus/html/27d74052-f564-4aaa-97a0-5f166ffd5ce3.htm)
* [Issues](https://github.com/StephenHidem/AntPlus/issues) - Mention hosting extensions in the issue title.
* [Discussions](https://github.com/StephenHidem/AntPlus/discussions) - Post questions and join discussions.

[osmf]: https://opensourcemaintenancefee.org/
[license]: https://github.com/StephenHidem/AntPlus/blob/master/LICENSE.txt
[eula]: https://github.com/StephenHidem/AntPlus/blob/master/OSMFEULA.txt