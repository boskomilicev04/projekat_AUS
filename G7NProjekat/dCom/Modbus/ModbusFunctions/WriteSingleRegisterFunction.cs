using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write single register functions/requests.
    /// </summary>
    public class WriteSingleRegisterFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleRegisterFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleRegisterFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusWriteCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusWriteCommandParameters mPacket = CommandParameters as ModbusWriteCommandParameters;
            byte[] request = new byte[12];

            request[0] = (byte)(mPacket.TransactionId >> 8);
            request[1] = (byte)(mPacket.TransactionId & 0xFF);

            request[2] = 0x00;
            request[3] = 0x00;

            request[4] = 0x00;
            request[5] = 0x06;

            request[6] = mPacket.UnitId;

            request[7] = mPacket.FunctionCode;

            request[8] = (byte)(mPacket.OutputAddress >> 8);
            request[9] = (byte)(mPacket.OutputAddress & 0xFF);

            request[10] = (byte)(mPacket.Value >> 8);
            request[11] = (byte)(mPacket.Value & 0xFF);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusWriteCommandParameters mPacket = CommandParameters as ModbusWriteCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            if (response.Length >= 12)
            {
                ushort value = (ushort)((response[10] << 8) | response[11]);
                result.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, mPacket.OutputAddress), value);
            }

            return result;
        }
    }
}