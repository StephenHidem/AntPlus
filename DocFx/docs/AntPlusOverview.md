# AntPlus Overview
The AntPlus class library supports a number of ANT+ device profiles as defined by Garmin/Dynastream. All device profiles
are based on the abstract class AntDevice. Common data pages and timeout options are provided to support the device profiles.

The following device profiles are supported -
- Asset Tracker
- Bicycle Power
    - Crank Torque Frequency Sensor
    - Standard Power Sensor
    - Standard Crank Torque Sensor
    - Standard Wheel Torque Sensor
- Bike Speed and Cadence
    - Bike Speed Sensor
    - Bike Cadence Sensor
    - Combined Speed and Cadence Sensor
- Fitness Equipment
    - Climber
    - Elliptical
    - Nordic Skier
    - Rower
    - Trainer/Stationary Bike
    - Treadmill
- Geocache
- Heartrate Monitor
- Muscle Oxygen
- Stride Based Speed and Distance Monitor

## AntDevice
This abstract class provides properties and methods common to all ANT devices. The virtual Parse method handles resetting the
device timeout each time a message is received. The RequestDataPage method sends a data page request to the ANT device.
The SendExtAcknowledgedMessage method sends commands/requests to the ANT device.
## CommonDataPages
CommonDataPages are typically added to derived AntDevice classes as a read-only property if the ANT device supports common data pages.
The AntDevice constructor will create and assign the CommonDataPages class. Here's an example from the BikeRadar class -
```csharp
public new CommonDataPages CommonDataPages { get; }

public BikeRadar(AntChannel channel) : base(channel)
{
    CommonDataPages = new CommonDataPages(this);
}
```
## AntDeviceCollection
This is a thread-safe observable collection of ANT devices. The constructor will initialize the ANT radio for continuous scan
mode and direct all messages received from the ANT radio to a private handler. This handler will select an ANT device from
the collection or create a new ANT device and add it to the collection. The message is then passed to the ANT device parser
for handling by the device.
>[!NOTE]
> Prefer AntCollection and the hosting extensions if your application uses dependency injection. See [HostingExtensions](HostingExtensionsOverview.md) for details.

[!code-csharp[](../../Examples/MAUI-gRPC/MauiAntGrpcClient/CustomAntDevice/BikeRadar.cs#Ctor)]
