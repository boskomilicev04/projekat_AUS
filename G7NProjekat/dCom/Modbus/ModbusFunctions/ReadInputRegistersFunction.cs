using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read input registers functions/requests.
    /// </summary>
    public class ReadInputRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadInputRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadInputRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            var mPacket = CommandParameters as ModbusReadCommandParameters;
            byte[] request = new byte[12];

            request[5] = 0x06;
            request[6] = mPacket.UnitId;
            request[7] = mPacket.FunctionCode;
            request[8] = (byte)(mPacket.StartAddress >> 8);
            request[9] = (byte)(mPacket.StartAddress & 0xFF);
            request[10] = (byte)(mPacket.Quantity >> 8);
            request[11] = (byte)(mPacket.Quantity & 0xFF);
            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            var mPacket = CommandParameters as ModbusReadCommandParameters;
            var result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response.Length >= 9 + response[8])
            {
                ushort value = (ushort)((response[9] << 8) | response[10]);
                result.Add(new Tuple<PointType, ushort>(PointType.ANALOG_INPUT, mPacket.StartAddress), value);
            }
            return result;
        }
    }
}