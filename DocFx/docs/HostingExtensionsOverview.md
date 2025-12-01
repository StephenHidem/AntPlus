## ANT+ Hosting Extensions Overview
The ANT+ Hosting Extensions simplify integrating the ANT+ Class Library into applications that use
dependency injection (DI) to compose the host application. MAUI applications use this pattern extensively
and other Windows applications can employ various host builders for DI.
## HostExtension Class
This class provides two extension methods to add the ANT+ classes to the host builder. UseAntPlus() is overloaded
to work with IHostBuilder implementations and MauiAppBuilder implementations.
Their purpose is identical; inject all the ANT+ devices and support infrastructure into the DI container.

An additional extension method, AddAntPlusServices(), is provided to add the ANT+ classes to an IServiceCollection when UseAntPlus()
is not suitable.

The TimeoutOptions class will also be added to the DI container and the configuration is bound by the container.
## Setting TimeoutOptions
The global timeout setting can come from a variety of sources such as the command line or appsettings.json.
Two options are available - **MissedMessages** and **Timeout**. Timeout is in milliseconds and MissedMessages is the
number of messages missed before signalling the ANT device is considered offline. If no timeout setting
is specified the timeout will default to 8 missed messages. Timeouts can be disabled by setting **Timeout = -1**. This is
equivalent to Timeout.Infinite.
>[!WARNING]
> Disabling timeouts is **NOT** recommended. It can lead to unintended consequences such as sending a
> message to a device that is no longer present. This can cause a radio channel to become blocked indefinitely.

>[!NOTE]
> Prefer MissedMessages as this will scale the timeout based on the broadcast rate of specific ANT devices.

Setting the option from the command line: **--TimeoutOptions:MissedMessages=8**.

Setting the option from appsetings.json: 
```json
{
  "TimeoutOptions": {
    "MissedMessages": 8,
    "Timeout": null
  }
}
```
## ISelectImplementation Interface
This is a simple interface that defines a single method - SelectImplementationType(). The purpose of
this method is to provide a mechanism for selecting one specific implementation from multiple implementations
of the same device class. Examples of this are bicycle power and fitness equipment sensors. A data page is
decoded and a specific implementation type is returned or null if the implementation could not be
determined from the data page.
>[!NOTE]
> You only need to implement this interface for ANT device profiles with multiple implementations.

Support for ANT+ devices not supported by the ANT+ Class Library can be added to the DI container and
device types that support multiple implementations will implement this interface and add it to the DI container.
It is added to the DI container with the AddKeyedSingleton() method with the device class value specified
as the key. For example -
```csharp
AddKeyedSingleton<ISelectImplementation, SelectBicyclePowerImplementation>(BicyclePower.DeviceClass);
```
## AntCollection Class
This class replaces the AntDeviceCollection class. It is a thread-safe ObservableCollection of AntDevices.
It exploits the benefits of dependency injection and handles the addition of custom AntDevice to the DI
container. It automatically brings in the TimeoutOptions, resolves any AntDevices present in the DI
container, and resolves any ISelectImplementations required to resolve an AntDevice type.
## See Also
- [How-To: Create and Add a Custom AntDevice to the Host Container]()