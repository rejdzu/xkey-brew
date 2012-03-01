using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XkeyApp.Utils.Shared
{
    public static class ISharedMethods
    {
        /// <summary>
        /// Convert byte array to HEX string
        /// </summary>
        /// <param name="value">byte array</param>
        /// <returns>HEX string</returns>
        public static string ConverByteToHex(byte[] value)
        {
            StringBuilder builder = new StringBuilder(value.Length * 2);
            foreach (byte b in value)
            {
                builder.Append(b.ToString("X02"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Check if object is null, DBNull or is empty string
        /// </summary>
        /// <param name="value">checked object</param>
        /// <returns>true - object is null, DBNull or empty string, false - not null, DBNull and empty</returns>
        public static bool IsObjectEmptyOrNull(object value)
        {
            return value == null || value == DBNull.Value || String.IsNullOrEmpty(value == null ? null : value.ToString());
        }
    }
}
