namespace ZConnect.Models
{
    public class SerialFrameModel
    {
        /// <summary>
        /// BuildFrame：Command + Data + CRC16
        /// [command][data][crcLow][crcHight]
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="data">Data</param>
        /// <returns>Complete frame byte array</returns>
        byte[] BuildFrame(byte command, byte[] data)
        {
            byte[] frame = new byte[] { command }.Concat(data).ToArray();   // command + data
            ushort crc = CRC16.Compute(frame);  // calc crc16
            byte crcLow = (byte)(crc & 0xFF);   // Split CRC16 into two bytes, 0xFF = 11111111
            byte crcHight = (byte)((crc >> 8) & 0xFF);

            return frame.Concat(new byte[] { crcLow, crcHight }).ToArray(); // command + data + crcLow + crcHight
        }

        /// <summary>
        /// VerifyFrame：Verify received complete frame
        /// </summary>
        /// <param name="frame">Complete frame with CRC</param>
        /// <returns>true = CRC is correct, false = CRC is incorrect</returns>
        bool VerifyFrame(byte[] frame)
        {
            if (frame == null || frame.Length < 3)
                return false;

            ushort receivedCrc = (ushort)(frame[^2] | (frame[^1] << 8));    // frame[^2] -> crcLow, frame[^1] -> crcHight, Combined into ushort CRC16
            ushort calculateCrc = CRC16.Compute(frame.Take(frame.Length - 2).ToArray());

            return receivedCrc == calculateCrc;
        }

        /// <summary>
        /// CRC16
        /// </summary>
        public static class CRC16
        {
            public static ushort Compute(byte[] data)
            {
                ushort crc = 0xFFFF;

                foreach (byte b in data)
                {
                    crc ^= b;   // XOR
                    for (int i = 0; i < 8; i++)
                    {
                        if ((crc & 0x0001) != 0)
                            crc = (ushort)((crc >> 1) ^ 0xA001);
                        else
                            crc >>= 1;
                    }
                }

                return crc;
            }
        }
    }
}
