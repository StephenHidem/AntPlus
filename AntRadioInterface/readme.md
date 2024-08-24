## Small Earth Technology ANT+ Radio Interface Library
Derive concrete class implementations from the AntRadioInterface class to support the hardware being used. This largely
follows Dynastream's .NET and native implementations of their ANT USB stick. Multiple interfaces are defined along with
classes to derive from (AntResponse and DeviceCapabilities). These classes and interfaces are used by the ANT+ class library.

See AntUsbStick in the [repository](https://github.com/StephenHidem/AntPlus/tree/master/Examples/AntUsbStick) for an example
that supports the Dynastream/Garmin ANT USB stick.
One important thing to note is this example uses the .NET and native DLLs provided in Dynastream's PC SDK.
### Additional Links
* [Documentation](https://stephenhidem.github.io/AntPlus/html/5e5a5e1c-a0e6-4ef0-a8f5-12f9394450c4.htm)
* [Examples Overview](https://stephenhidem.github.io/AntPlus/html/27d74052-f564-4aaa-97a0-5f166ffd5ce3.htm)
* [Issues](https://github.com/StephenHidem/AntPlus/issues) - Mention AntRadioInterface in the issue title.
* [Discussions](https://github.com/StephenHidem/AntPlus/discussions) - Post questions and join discussions.
