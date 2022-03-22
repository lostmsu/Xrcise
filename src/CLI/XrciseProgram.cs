namespace Xrcise
{
    using System;
    using System.IO;
    using Windows.Devices.Bluetooth.Rfcomm;
    using Windows.Devices.Enumeration;
    using Windows.Networking.Sockets;
    using Xrcise.LifeSpan;

    class XrciseProgram
    {
        static void Main(string[] args)
        {
            string bluetoothSerialPort = RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort);
            var bluetoothDevices = DeviceInformation.FindAllAsync(bluetoothSerialPort).AsTask().Result;
            foreach(var device in bluetoothDevices) {
                var service = RfcommDeviceService.FromIdAsync(device.Id).AsTask().Result;
                using (var socket = new StreamSocket()) {
                    socket.ConnectAsync(service.ConnectionHostName, service.ConnectionServiceName, service.ProtectionLevel).AsTask().Wait();
                    using (var fromDevice = socket.InputStream.AsStreamForRead())
                    using (var toDevice = socket.OutputStream.AsStreamForWrite()) {
                        var treadmill = new LifeSpanTreadmill(toDevice, fromDevice);
                    }
                }
                Console.WriteLine(device.Name);
            }
        }
    }
}
