namespace Xrcise.LifeSpan
{
    using System;
    using System.IO;
    using System.Linq;

    public class LifeSpanTreadmill
    {
        readonly Stream toDevice, fromDevice;
        readonly byte[] responseBuffer = new byte[8];
        readonly BinaryWriter writer;

        public LifeSpanTreadmill(Stream toDevice, Stream fromDevice) {
            this.toDevice = toDevice ?? throw new ArgumentNullException(nameof(toDevice));
            this.fromDevice = fromDevice ?? throw new ArgumentNullException(nameof(fromDevice));
            this.writer = new BinaryWriter(this.toDevice);
            this.Init();
        }

        public LifeSpanTreadmill(Stream deviceStream) {
            this.toDevice = this.fromDevice = deviceStream ?? throw new ArgumentNullException(nameof(deviceStream));
            this.writer = new BinaryWriter(this.toDevice);
            this.Init();
        }

        void Init() {
            long[] resposes = new uint[] {
                0x20000000,
                0xC2000000,
                0xE9FF0000,
                0xE400F400,
            }.Select(this.SendCommand).ToArray();
        }

        long SendCommand(Command command) => this.SendCommand((uint)command);

        long SendCommand(uint command) {
            this.writer.Write(command);
            int read = this.fromDevice.Read(this.responseBuffer, 0, 6);
            if (read < 6)
                throw new NotSupportedException();
            return BitConverter.ToInt64(this.responseBuffer, 0);
        }

        static decimal ToDecimal(long response) => response & 0xFF;

        enum Command: uint
        {
            Start = 0xE1000000,
            Stop = 0xE0000000,
            GetSteps = 0xA1880000,
            GetCalories = 0xA1870000,
            GetDistance = 0xA1850000,
            GetTime = 0xA1890000,
            GetSpeed = 0xA1820000,
        }
    }
}
