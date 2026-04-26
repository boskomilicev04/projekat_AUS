using Common;
using Modbus.FunctionParameters;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Modbus.ModbusFunctions
{
    /// <summary>
    /// Class containing logic for parsing and packing modbus read holding registers functions/requests.
    /// </summary>
    public class ReadHoldingRegistersFunction : ModbusFunction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadHoldingRegistersFunction"/> class.
        /// </summary>
        /// <param name="commandParameters">The modbus command parameters.</param>
        public ReadHoldingRegistersFunction(ModbusCommandParameters commandParameters) : base(commandParameters)
        {
            CheckArguments(MethodBase.GetCurrentMethod(), typeof(ModbusReadCommandParameters));
        }

        /// <inheritdoc />
        public override byte[] PackRequest()
        {
            ModbusReadCommandParameters mPacket = CommandParameters as ModbusReadCommandParameters;
            byte[] request = new byte[12];

            // Transaction ID
            request[0] = (byte)(mPacket.TransactionId >> 8);
            request[1] = (byte)(mPacket.TransactionId & 0xFF);

            // Protocol ID (0 za Modbus)
            request[2] = 0x00;
            request[3] = 0x00;

            // Length (UnitID + FC + StartAddr + Quantity = 6 bajtova)
            request[4] = 0x00;
            request[5] = 0x06;

            // Unit ID (Slave adresa, npr. 158)
            request[6] = mPacket.UnitId;

            // Function Code (0x03 za Read Holding Registers)
            request[7] = mPacket.FunctionCode;

            // Start Address (Adresa za N1 je 1500)
            request[8] = (byte)(mPacket.StartAddress >> 8);
            request[9] = (byte)(mPacket.StartAddress & 0xFF);

            // Quantity (Broj registara za čitanje)
            request[10] = (byte)(mPacket.Quantity >> 8);
            request[11] = (byte)(mPacket.Quantity & 0xFF);

            return request;
        }

        /// <inheritdoc />
        public override Dictionary<Tuple<PointType, ushort>, ushort> ParseResponse(byte[] response)
        {
            ModbusReadCommandParameters mPacket = CommandParameters as ModbusReadCommandParameters;
            Dictionary<Tuple<PointType, ushort>, ushort> result = new Dictionary<Tuple<PointType, ushort>, ushort>();

            // Modbus TCP odgovor: Header(7b) + FC(1b) + ByteCount(1b) + Data...
            // ByteCount je na indeksu 8, podaci kreću od indeksa 9
            if (response.Length >= 9 + response[8])
            {
                ushort startAddr = mPacket.StartAddress;
                int byteCount = response[8];

                for (int i = 0; i < byteCount / 2; i++)
                {
                    // Svaki Holding Register su 2 bajta
                    ushort value = (ushort)((response[9 + i * 2] << 8) | response[10 + i * 2]);
                    result.Add(new Tuple<PointType, ushort>(PointType.ANALOG_OUTPUT, (ushort)(startAddr + i)), value);
                }
            }

            return result;
        }
    }
}