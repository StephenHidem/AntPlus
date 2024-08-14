using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmallEarthTech.AntPlus.DeviceProfiles;
using SmallEarthTech.AntRadioInterface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SmallEarthTech.AntPlus.Extensions.Hosting
{
    /// <summary>
    /// This class largely mirrors <see cref="AntDeviceCollection"/> but is designed specifically to be integrated into
    /// an application via dependency injection.
    /// </summary>
    public partial class AntCollection : ObservableCollection<AntDevice>, IDisposable
    {
        /// <summary>
        /// The collection lock typically used by WPF applications to synchronize UI updates when devices are added or
        /// removed from the collection. For example, in the code behind for a window that is using this collection
        /// would include the following line in its constructor -
        /// <code>BindingOperations.EnableCollectionSynchronization(viewModel.AntDevices, viewModel.AntDevices.CollectionLock);</code>
        /// </summary>
        public object CollectionLock = new object();

        private readonly IServiceProvider _services;
        private readonly IAntRadio _antRadio;
        private readonly ILogger<AntCollection> _logger;
        private readonly TimeoutOptions _timeout;

        private IAntChannel[]? _channels;
        private SendMessageChannel? _channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AntCollection"/> class. The ANT radio is initialized for continuous scan mode.
        /// </summary>
        /// <param name="services">The service provider.</param>
        /// <param name="antRadio">The ANT radio.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The ANT device timeout options.</param>
        public AntCollection(IServiceProvider services, IAntRadio antRadio, ILogger<AntCollection> logger, IOptions<TimeoutOptions> options)
        {
            _services = services;
            _antRadio = antRadio;
            _logger = logger;
            _timeout = options.Value;
            _logger.LogInformation("Created AntCollection");
        }

        /// <summary>
        /// Initializes the ANT radio for continuous scan mode and setup ANT channels for use by this collection.
        /// </summary>
        /// <returns>Task of type void</returns>
        public async Task StartScanning()
        {
            _channels = await _antRadio.InitializeContinuousScanMode();
            _channel = new SendMessageChannel(_channels.Skip(1).ToArray(), _logger);
            _channels[0].ChannelResponse += MessageHandler;
        }

        /// <summary>
        /// The collection message handler. All messages received from the ANT radio pass through here. New devices are added to the
        /// collection and the message is dispatched to the device for processing.
        /// </summary>
        /// <param name="sender">The ANT radio.</param>
        /// <param name="e">The ANT response.</param>
        private void MessageHandler(object? sender, AntResponse e)
        {
            if (e.ChannelId != null && e.Payload != null)
            {
                // see if the device is in the collection
                AntDevice? device;
                device = this.FirstOrDefault(ant => ant.ChannelId.Id == e.ChannelId.Id);

                // create the device if not in the collection
                if (device == null)
                {
                    // get the device type
                    Type? deviceType = GetAntDeviceType(e.ChannelId, e.Payload!);
                    if (deviceType == null) return;     // exit if unable to determine device type

                    // create ant device from service provider and apply timeout options
                    // the ActivatorUtilities allow us to inject ctor parameters into the requested service
                    device = (AntDevice?)ActivatorUtilities.CreateInstance(_services, deviceType, e.ChannelId, _channel!, _timeout);

                    if (device == null) return;     // exit if unable to create the device
                    Add(device);
                    device.DeviceWentOffline += DeviceOffline;
                }

                // dispatch the message to the device
                device.Parse(e.Payload);
            }
            else
            {
                _logger.LogCritical("ChannelId is null. Channel # = {ChannelNum}, Response ID = {Response}, Payload = {Payload}.",
                    e.ChannelNumber,
                    (MessageId)e.ResponseId,
                    e.Payload != null ? BitConverter.ToString(e.Payload) : "Null");
                Dispose();  // clear the channels
            }
        }

        private void DeviceOffline(object? sender, EventArgs e)
        {
            AntDevice device = (AntDevice)sender!;
            device.DeviceWentOffline -= DeviceOffline;
            _ = Remove(device);
        }

        /// <summary>Adds the specified <see cref="AntDevice"/> to the end of the collection.</summary>
        /// <param name="item">The <see cref="AntDevice"/>.</param>
        public new void Add(AntDevice item)
        {
            lock (CollectionLock)
            {
                base.Add(item);
                _logger.LogDebug("{Device} added.", item);
            }
        }

        /// <summary>Removes the specified <see cref="AntDevice"/> from the collection.</summary>
        /// <param name="item">The <see cref="AntDevice"/>.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the original collection.</returns>
        public new bool Remove(AntDevice item)
        {
            lock (CollectionLock)
            {
                bool result = base.Remove(item);
                _logger.LogDebug("{Device} remove. Result = {Result}", item, result);
                return result;
            }
        }

        /// <summary>
        /// Gets the type of the ANT device.
        /// </summary>
        /// <param name="channelId">The channel identifier.</param>
        /// <param name="page">The page received from the device.</param>
        /// <returns>The type of the device.</returns>
        private Type? GetAntDeviceType(ChannelId channelId, byte[] page)
        {
            // check if the ServiceCollection is null
            if (HostExtensions.ServiceCollection == null) throw new InvalidOperationException("The ServiceCollection is null. UseAntPlus() or UseMauiAntPlus() must be invoked when building app services.");

            ISelectImplementation? selector = _services.GetKeyedService<ISelectImplementation>(channelId.DeviceType);
            if (selector != null) { return selector.SelectImplementationType(page); }
            else
            {
                return HostExtensions.ServiceCollection.
                        FirstOrDefault(d =>
                        d.ServiceType == typeof(AntDevice) &&
                        d.IsKeyedService &&
                        d.ServiceKey is byte key &&
                        key == channelId.DeviceType)?.
                        KeyedImplementationType ?? typeof(UnknownDevice);
            }
        }

        /// <summary>
        /// Release the ANT channels.
        /// </summary>
        public void Dispose()
        {
            if (_channels != null)
            {
                _logger.LogDebug("AntCollection: Dispose");
                _channels[0].ChannelResponse -= MessageHandler;
                foreach (IAntChannel item in _channels)
                {
                    item.Dispose();
                }
                _channels = null;
            }
        }
    }
}
