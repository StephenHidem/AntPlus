---
_layout: landing
---

# Welcome to the Small Earth Technology ANT+ Class Libraries.
<p align="center">
    <img src="images/SmallEarthTech.png" width="200"/>
    <br/>
    <em>Whitespace is the programmers punctuation mark</em>
</p>

## Overview
Welcome to the Small Earth Technology ANT+ class libraries and examples. My goal is to provide a frictionless
as possible implementation to enable applications to interface and acquire data from a variety of ANT+
sensor sources. The primary class is AntPlus and it contains device profiles of ANT+ devices and common data pages.
The AntRadioInterface class permits different interfaces to the hardware being used and lends itself to
dependency injection.

The libraries are largely intended to support applications interfacing to ANT devices.
This could be a group app, mobile app, push data to the cloud, or some other purpose.

The primary projects are **AntPlus** and **AntRadioInterface**.
AntPlus defines the device profiles derived from the AntDevice class. The AntRadioInterface defines an interface
to interact with an ANT radio to send and receive from an ANT device. Implementors of the AntRadioInterface supply a
concrete implementation.

The projects in the Examples folder illustrate usage of the libraries. Of particular note is the AntUsbStick
project. It provides a concrete implementation of the AntRadioInterface using the Dynastream/Garmin ANT USB
sticks. This project is referenced by the examples.

### Getting Started
Become an ANT+ Adopter! There is no membership fee. Create a login and go to [Become an Adopter](https://www.thisisant.com/my-ant/join-adopter) to sign up.
This provides you with access to device profiles, SDK's, and useful software tools.

### Prerequisites
- **Visual Studio 2022 Community Edition** or later
- Clone or fork this repository.
- OPTIONAL: ANT USB stick hardware and device drivers. I use two sticks for testing and some example projects require it.
You can get them from DigiKey for around $45 for two.
- OPTIONAL: Only needed if you intend to modify the libraries I've provided in the AntUsbStick project.
Download the [ANT PC SDK zip file](https://www.thisisant.com/resources/ant-windows-library-package-with-source-code/),
unblock, and install.
- OPTIONAL: To build the documentation, install the Sandcastle Help File Builder (SHFB) from the
[Sandcastle Help File Builder project](https://github.com/EWSoftware/SHFB)
- OPTIONAL: To build the AntGrpcServicePackage installer, install the [WIX toolset and Visual Studio integration](https://www.firegiant.com/wixtoolset/).

## Examples
- The **AntUsbStick** class library is derived from AntRadioInterface and implements a concrete implementation
that supports commercially available ANT USB sticks from Dynastream/Garmin. This project is essential to the other examples.
It's now available as a NuGet package.
- The **WpfUsbStickApp** WPF application demonstrates usage of the ANT class library. It depends on the
AntUsbStick project and the hosting extensions.
- The **MauiAntGrpcClient** This example illustrates using .NET MAUI and gRPC. You can find it in the Examples folder
under MAUI-gRPC. It consists of four projects - a shared gRPC library, the gRPC service, the service installer, and the client application.
The client application runs under Windows, Android emulators, and Android mobile devices. I have not tested it on any of the other platforms
MAUI apps can run on.

See the [Examples Overview](docs/Examples/ExamplesOverview.md) for more information on the examples.

## Other Resources
- [AntPlus Source Code Repository](https://github.com/StephenHidem/AntPlus)
- [Version History](docs/VersionHistory/VersionHistory.md)
