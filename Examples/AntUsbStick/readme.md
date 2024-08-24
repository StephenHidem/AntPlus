## Small Earth Technology ANT+ USB Stick Class Library
This package implements the Small Earth Technology ANT radio interface. It supports Garmin/Dynastream ANT USB sticks and some development
boards with USB interfaces.
### Getting Started
> **Important:** Projects that consume this package must set the platform target to x86. This is due to the native DLLs that are a part of the package.

The easiest way to make use of the package is to add it to your DI container.
```csharp
// dependency services
_host = Host.CreateDefaultBuilder().
    ConfigureServices(s =>
    {
        ...
        s.AddSingleton<IAntRadio, AntRadio>();
        s.AddSingleton<AntDeviceCollection>();
        ...
    }).
    Build();
```
This snippet makes use of the Small Earth Technology ANT+ Class Library AntDeviceCollection class and DI container constructor injection. The DI container
will provide logging services and the IAntRadio implementation provided by the ANT+ USB Stick Class Library to the AntDeviceCollection.
### Addtional Links
* [Documentation](https://stephenhidem.github.io/AntPlus/html/5e5a5e1c-a0e6-4ef0-a8f5-12f9394450c4.htm)
* [Examples Overview](https://stephenhidem.github.io/AntPlus/html/27d74052-f564-4aaa-97a0-5f166ffd5ce3.htm)
* [Issues](https://github.com/StephenHidem/AntPlus/issues) - Mention AntUsbStick in the issue title.
* [Discussions](https://github.com/StephenHidem/AntPlus/discussions) - Post questions and join discussions.
