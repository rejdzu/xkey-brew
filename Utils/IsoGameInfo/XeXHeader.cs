using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using XkeyApp.Utils.Shared;

namespace XkeyApp.Utils.IsoGameInfo
{
    /// <summary>
    /// Class contains info from XeXHeader of deafult.xex file
    /// </summary>
    [Serializable]
    public class XeXHeader : IDisposable
    {
        private byte[] mediaId;

        public string MediaId { get { return ISharedMethods.ConverByteToHex(mediaId); } }

        private uint version;

        public uint Version { get { return version; } }

        private uint baseVersion;

        public uint BaseVersion { get { return baseVersion; } }

        private byte[] titleId;

        public string TitleId { get { return ISharedMethods.ConverByteToHex(titleId); } }

        private byte platform;

        public string Platform { get { return platform.ToString(); } }

        private byte executableType;

        public string ExecutableType { get { return executableType.ToString(); } }

        private byte discNumber;

        public int DiscNumber { get { return (int)discNumber; } }

        private byte discCount;

        public int DiscCount { get { return (int)discCount; } }

        public XeXHeader(MemoryStream file)
        {
            MyBinaryReader reader;

            try
            {
                reader = new MyBinaryReader(file, EndianType.BigEndian);

                reader.BaseStream.Seek(0L, SeekOrigin.Begin);
                if (Encoding.ASCII.GetString(reader.ReadBytes(4)) == "XEX2")
                {
                    reader.BaseStream.Seek(20L, SeekOrigin.Begin);

                    uint headerCount = reader.ReadUInt32();
                    byte[] infoArray = new byte[] { 0, 4, 0, 6 };

                    for (int i = 0; i < headerCount; i++)
                    {
                        byte[] headerIdArray = reader.ReadBytes(4);

                        uint headerId = BitConverter.ToUInt32(headerIdArray, 0);

                        if (headerId == BitConverter.ToUInt32(infoArray, 0))
                        {
                            uint dataAddress = reader.ReadUInt32();

                            reader.BaseStream.Seek((long)dataAddress, SeekOrigin.Begin);
                            this.mediaId = reader.ReadBytes(4);
                            this.version = reader.ReadUInt32();
                            this.baseVersion = reader.ReadUInt32();
                            this.titleId = reader.ReadBytes(4);
                            this.platform = reader.ReadByte();
                            this.executableType = reader.ReadByte();
                            this.discNumber = reader.ReadByte();
                            this.discCount = reader.ReadByte();
                            break;
                        }
                        else
                        {
                            reader.ReadUInt32();
                        }
                    }
                }
                else throw new Exception("Extracted file is not XEX file");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            reader.Close();
        }

        /// <summary>
        /// Converts object to string
        /// </summary>
        /// <returns>Property name and value separated by new line</returns>
        public string ToString()
        {
            StringBuilder result = new StringBuilder();

            PropertyInfo[] propertyInfos = typeof(XeXHeader).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo pi in propertyInfos)
            {
                result.AppendLine(pi.Name + ": " + pi.GetValue(this, null));
            }

            return result.ToString();
        }

        public void Dispose()
        {
            this.mediaId = null;
            this.version = 0;
            this.baseVersion = 0;
            this.titleId = null;
            this.platform = 0;
            this.executableType = 0;
            this.discNumber = 0;
            this.discCount = 0;
        }
    }
}
