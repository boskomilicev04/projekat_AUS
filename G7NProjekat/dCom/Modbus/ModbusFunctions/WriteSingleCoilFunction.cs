using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus write coil functions/requests.
    /// </summary>
    public class WriteSingleCoilFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WriteSingleCoilFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public WriteSingleCoilFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
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

            request[10] = (byte)(mPacket.Value == 1 ? 0xFF : 0x00);
            request[11] = 0x00;

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusWriteCommandParameters mPacket = CommandParameters as ModbusWriteCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            result.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, mPacket.OutputAddress), mPacket.Value);

            return result;
        }


    }
}