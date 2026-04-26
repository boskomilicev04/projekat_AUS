using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read coil functions/requests.
    /// </summary>
    public class ReadCoilsFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadCoilsFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
		public ReadCoilsFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc/>
        public override byte[] PackRequest()
        {
            var mPacket = CommandParameters as ModbusReadCommandParameters;
            byte[] request = new byte[12];

            request[0] = (byte)(mPacket.TransactionId >> 8);
            request[1] = (byte)(mPacket.TransactionId & 0xFF);
            request[2] = 0x00;
            request[3] = 0x00;
            request[4] = 0x00;
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

            if (response.Length >= 9)
            {
                ushort value = response[9] > 0 ? (ushort)1 : (ushort)0;
                result.Add(new Tuple<PointType, ushort>(PointType.DIGITAL_OUTPUT, mPacket.StartAddress), value);
            }

            return result;
        }
    }
}