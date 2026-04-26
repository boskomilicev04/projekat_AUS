namespace G7NClient.Core
{
    public enum PointType { AnalogInput, AnalogOutput, DigitalOutput }
    public class ScadaPoint
    {
        public required string Name { get; set; }
        public PointType Type { get; set; }
        public ushort Address { get; set; }
        public double Value { get; set; }

        public ushort RawValue { get; set; }

        public ScadaPoint() { }

        public ScadaPoint(string name, PointType type, ushort address)
        {
            Name = name;
            Type = type;
            Address = address;
            Value = 0;
            RawValue = 0;
        }
        public byte[] GetValueBytes()
        {
            if (Address == 3100 || Address == 1500)
            {
                ushort val = (ushort)Value;
                return new byte[] { (byte)(val >> 8), (byte)(val & 0xFF) };
            }
            else
            {
                byte digitalVal = (byte)(Value > 0 ? 0x01 : 0x00);
                return new byte[] { digitalVal };
            }
        }
    }
}
