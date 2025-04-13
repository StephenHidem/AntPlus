[![ANT+ .NET Test](https://github.com/StephenHidem/AntPlus/actions/workflows/dotnet.yml/badge.svg)](https://github.com/StephenHidem/AntPlus/actions/workflows/dotnet.yml)
# Small Earth Technology ANT+ Class Library
Welcome to the Small Earth Technology ANT+ class library and examples. My goal is to provide a frictionless
as possible implementation to enable applications to interface and acquire data from a variety of ANT+
sensor sources. The primary class is AntPlus and it contains device profiles of ANT+ devices and common data pages.
The AntRadioInterface project provides interfaces to implement depending on the hardware being used and lends itself to
dependency injection.

The class library is a .NET standard 2.0 compliant assembly in order to provide the broadest application reach,
from desktop applications to mobile platforms.
### Prerequisites
Become an ANT+ Adopter! There is no membership fee. 
Create a login and go to [Become an Adopter](https://www.thisisant.com/my-ant/join-adopter). This provides you with access to
device profiles, SDK's, and useful software tools.
- Visual Studio 2022 Community Edition or higher.
- OPTIONAL: ANT USB stick hardware and device drivers. I use two sticks, one for simulating devices, and the other for
 testing. Some example projects require it.
You can get them from DigiKey for around $45 for two.
- OPTIONAL: Only needed if you intend to modify the libraries I've provided in the AntUsbStick project.
Download the [ANT PC SDK zip file](https://www.thisisant.com/resources/ant-windows-library-package-with-source-code/), unblock, and install.
### Project Documentation
* [ANT+ Class Library Help](http://stephenhidem.github.io/AntPlus)