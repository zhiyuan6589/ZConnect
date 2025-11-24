using System;
using System.Collections.Generic;
using System.Text;

namespace ZConnect.Utils
{
    public class FormatConverter
    {
        public static byte[] HexToBytes(string hex)
        {
            hex = hex.Replace(" ", "")
                     .Replace("-", "")
                     .Trim();   // Remove leading and trailing whitespace characters. "AA BB CC" or "AA-BB-CC" -> "AABBCC"

            if (hex.Length % 2 == 1)    // If the Hex length is an odd number, add leading 0. "ABC" -> "0ABC"
                hex = "0" + hex;

            int len = hex.Length / 2;   // Calculate the number of bytes required. Hex * 2 = 1 Byte.
            byte[] bytes = new byte[len];

            for (int i = 0; i < len; i++)
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16); // Grouping converts hex data into bytes.

            return bytes;
        }

        public static string BytesToHex(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", " ");
        }

        public static string BytesToAscii(byte[] data)
        {
            var chars = new StringBuilder();

            foreach(var b in data)
            {
                char c = (char)b;   // byte -> ascii char
                chars.Append(char.IsControl(c) ? '.' : c);  // Control char (NULL, BEL, LF, CR, ESC...) -> .
            }

            return chars.ToString();
        }
    }
}
