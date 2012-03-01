using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using XkeyApp.Utils.Shared;

namespace XkeyApp.Utils.IsoGameInfo
{
    /// <summary>
    /// Enum contains 3 types of xbox media
    /// </summary>
    public enum IsoType : uint
    {
        GDF = 0xfd90000,
        XGD3 = 0x2080000,
        XSF = 0
    }

    /// <summary>
    /// Class contains info about iso file
    /// </summary>
    public class Iso : IDisposable
    {
        private string filePath;

        public string FilePath { get { return filePath; } }

        private FileStream file;

        public FileStream File { get { return file; } }

        private MyBinaryReader reader;

        public MyBinaryReader Reader { get { return reader; } }

        private IsoType isoType;

        public IsoType IsoType { get { return isoType; } }

        private IsoInfo isoInfo;

        public IsoInfo IsoInfo { get { return isoInfo; } }

        private DefaultXeX defaultXeX;

        public DefaultXeX DefaultXeX { get { return defaultXeX; } }

        public Iso(string path) : this(path, true) { }

        public Iso(string path, bool readData)
        {
            this.filePath = path;
            if(readData)
                ReadGameInfo();
        }

        /// <summary>
        /// Opens file, file system info, reads game info, extracts default.xex, closes file
        /// </summary>
        private void ReadGameInfo()
        {
            OpenIsoFile();
            ReadIsoData();
            CheckIfXbox360Iso();
            ReadDefaultXex();
            CloseFile();
        }

        /// <summary>
        /// Saves root folder byte array to disc
        /// </summary>
        /// <param name="path"></param>
        public void SaveRootFolderTree(string path)
        {
            OpenIsoFile();
            ReadIsoData();

            this.Reader.BaseStream.Seek((long)((this.IsoInfo.RootDirSector * this.IsoInfo.SectorSize) + this.IsoInfo.RootOffset), System.IO.SeekOrigin.Begin);

            byte[] rootDirBuffer = this.Reader.ReadBytes((int)this.IsoInfo.RootDirSize);

            System.IO.File.WriteAllBytes(path, rootDirBuffer);

            CloseFile();
        }


        /// <summary>
        /// Open Iso file
        /// </summary>
        private void OpenIsoFile()
        {
            if (CheckPath())
            {
                try
                {
                    this.file = new FileStream(this.filePath, FileMode.Open, FileAccess.Read);
                    this.reader = new MyBinaryReader(this.file);
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            else throw new Exception("Provided path is not correct");
        }

        /// <summary>
        /// Check if given path exists
        /// </summary>
        /// <returns></returns>
        private bool CheckPath()
        {
            if (!ISharedMethods.IsObjectEmptyOrNull(this.filePath))
                if (System.IO.File.Exists(this.filePath))
                    return true;

            return false;
        }

        /// <summary>
        /// Check if given iso is XBOX 360
        /// </summary>
        private void CheckIfXbox360Iso()
        {
            if (this.isoType == IsoType.XSF)
                throw new Exception("Provided iso is not XBOX360 iso");
        }

        /// <summary>
        /// Reads data from default.xex
        /// </summary>
        private void ReadDefaultXex()
        {
            this.defaultXeX = new DefaultXeX(this);
        }

        /// <summary>
        /// Reads file system info
        /// </summary>
        private void ReadIsoData()
        {
            try
            {
                this.isoInfo = new IsoInfo();
                this.isoInfo.SectorSize = 0x800;
                this.reader.BaseStream.Seek((long)(0x20 * this.isoInfo.SectorSize), SeekOrigin.Begin);
                if (Encoding.ASCII.GetString(this.reader.ReadBytes(20)) == "MICROSOFT*XBOX*MEDIA")
                {
                    this.isoType = IsoType.XSF;
                    this.isoInfo.RootOffset = (uint)this.isoType;
                }
                else
                {
                    this.file.Seek((long)((0x20 * this.isoInfo.SectorSize) + 0xfd90000), SeekOrigin.Begin);
                    if (Encoding.ASCII.GetString(this.reader.ReadBytes(20)) == "MICROSOFT*XBOX*MEDIA")
                    {
                        this.isoType = IsoType.GDF;
                        this.isoInfo.RootOffset = (uint)this.isoType;
                    }
                    else
                    {
                        this.isoType = IsoType.XGD3;
                        this.isoInfo.RootOffset = (uint)this.isoType;
                    }
                }
                this.reader.BaseStream.Seek((long)((0x20 * this.isoInfo.SectorSize) + this.isoInfo.RootOffset), SeekOrigin.Begin);
                this.isoInfo.Identifier = this.reader.ReadBytes(20);
                this.isoInfo.RootDirSector = this.reader.ReadUInt32();
                this.isoInfo.RootDirSize = this.reader.ReadUInt32();
                this.isoInfo.ImageCreationTime = this.reader.ReadBytes(8);
                this.isoInfo.VolumeSize = (ulong)(this.reader.BaseStream.Length - this.isoInfo.RootOffset);
                this.isoInfo.VolumeSectors = (uint)(this.isoInfo.VolumeSize / ((ulong)this.isoInfo.SectorSize));
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot read ISO info", ex);
            }
        }

        public void Dispose()
        {
            CloseFile();
            this.defaultXeX.Dispose();
            this.isoType = IsoType.GDF;
            this.isoInfo = new IsoInfo();
        }

        private void CloseFile()
        {
            this.filePath = String.Empty;
            this.reader.Close();
            this.file.Close();
            this.file.Dispose();
        }
    }

    public struct IsoInfo
    {
        public byte[] Identifier;
        public uint RootDirSector;
        public uint RootDirSize;
        public byte[] ImageCreationTime;
        public uint SectorSize;
        public uint RootOffset;
        public ulong VolumeSize;
        public uint VolumeSectors;
    }
}
