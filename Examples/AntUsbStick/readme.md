## Small Earth Technology ANT+ USB Stick Class Library
This package implements the Small Earth Technology ANT radio interface. It supports Garmin/Dynastream ANT USB sticks and some development
boards with USB interfaces.
##### Getting Started
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
##### Addtional Links
* [Documentation](https://stephenhidem.github.io/AntPlus/html/bf8a5d40-6d1a-6d79-a57f-fd17688d7682.htm)
* [Examples](https://github.com/StephenHidem/AntPlus/tree/master/Examples) - Includes AntUsbStick source code
* [Issues](https://github.com/StephenHidem/AntPlus/issues) - Mention AntUsbStick in the issue title.
* [Discussions](https://github.com/StephenHidem/AntPlus/discussions) - Post questions and join discussions.
