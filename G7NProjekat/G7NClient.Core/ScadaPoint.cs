namespace G7NClient.Core
{
    public enum PointType { AnalogInput, AnalogOutput, DigitalOutput }
    public class ScadaPoint
    {
        public required string Name { get; set; }
        public PointType Type { get; set; }
        public ushort Address { get; set; }
        public double Value { get; set; }
    }
}
