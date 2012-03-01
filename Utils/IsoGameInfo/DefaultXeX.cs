using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace XkeyBrew.Utils.IsoGameInfo
{
    /// <summary>
    /// Class contains deafault.xex file with properties like, file size, sector of file or XeXHeader
    /// </summary>
    public class DefaultXeX : IDisposable
    {
        private Iso iso;

        private byte[] file;

        public byte[] File { get { return file; } }

        private string fileName = "default.xex";

        public string FileName { get { return fileName; } }

        private byte fileNameLength;

        public byte FileNameLength { get { return fileNameLength; } }

        private uint fileSize;

        public uint FileSize { get { return fileSize; } }

        private uint startingSectorOfFile;

        public uint StartingSectorOfFile { get { return startingSectorOfFile; } }

        private ushort offsetLeftSubTree;

        public ushort OffsetLeftSubTree { get { return offsetLeftSubTree; } }

        private ushort offsetRightSubTree;

        public ushort OffsetRightSubTree { get { return offsetRightSubTree; } }

        private XeXHeader xexHeader;

        public XeXHeader XeXHeader { get { return xexHeader; } }

        public DefaultXeX(Iso iso)
        {
            this.iso = iso;
            ExtractInfo();
        }

        /// <summary>
        /// Extracts info about default.xex from iso
        /// </summary>
        private void ExtractInfo()
        {
            if (SearchForDefaultXeX())
            {
                try
                {
                    MemoryStream ms = new MemoryStream(this.file);

                    this.xexHeader = new XeXHeader(ms);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error while reading XEX header", ex);
                }
            }
            else
            {
                throw new Exception("Default.xex was not found");
            }
        }

        /// <summary>
        /// Search for default.xex
        /// </summary>
        /// <returns>true if found; false if not found</returns>
        private bool SearchForDefaultXeX()
        {
            try
            {
                this.iso.Reader.BaseStream.Seek((long)(((long)this.iso.IsoInfo.RootDirSector * (long)this.iso.IsoInfo.SectorSize) + (long)this.iso.IsoInfo.RootOffset), System.IO.SeekOrigin.Begin);

                byte[] rootDirBuffer = this.iso.Reader.ReadBytes((int)this.iso.IsoInfo.RootDirSize);

                MemoryStream ms = new MemoryStream(rootDirBuffer);
                MyBinaryReader reader = new MyBinaryReader(ms);

                while (reader.BaseStream.Position < this.iso.IsoInfo.RootDirSize)
                {
                    ushort offsetLeftSubTreeTmp = reader.ReadUInt16();
                    ushort offsetRightSubTreeTmp = reader.ReadUInt16();

                    if (offsetLeftSubTreeTmp != 0xffff && offsetRightSubTreeTmp != 0xffff)
                    {
                        uint startingSectorOfFileTmp = reader.ReadUInt32();
                        uint fileSizeTmp = reader.ReadUInt32();
                        uint fileAttribute = (uint)reader.ReadByte();
                        byte fileNameLengthTmp = reader.ReadByte();
                        string fileNameTmp = Encoding.ASCII.GetString(rootDirBuffer, (int)reader.BaseStream.Position, fileNameLengthTmp);
                        reader.BaseStream.Seek((long)fileNameLengthTmp, SeekOrigin.Current);
                        long num1 = reader.BaseStream.Position % 4L;
                        if ((reader.BaseStream.Position % 4L) != 0L)
                        {
                            reader.BaseStream.Seek(4L - (reader.BaseStream.Position % 4L), SeekOrigin.Current);
                        }

                        if (fileNameTmp.ToLower() == this.fileName.ToLower())
                        {
                            this.offsetLeftSubTree = offsetLeftSubTreeTmp;
                            this.offsetRightSubTree = offsetRightSubTreeTmp;
                            this.startingSectorOfFile = startingSectorOfFileTmp;
                            this.fileSize = fileSizeTmp;
                            this.fileNameLength = fileNameLengthTmp;
                            //this.iso.Reader.Read(this.file, (int)(this.iso.IsoInfo.RootOffset + (this.startingSectorOfFile * this.iso.IsoInfo.SectorSize)), (int)this.fileSize);
                            this.iso.Reader.BaseStream.Seek((long)((long)this.iso.IsoInfo.RootOffset + ((long)this.startingSectorOfFile * (long)this.iso.IsoInfo.SectorSize)), SeekOrigin.Begin);
                            this.file = this.iso.Reader.ReadBytes((int)this.fileSize);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error browsing ISO for default.xex", ex);
            }

            return false;
        }

        public void Dispose()
        {
            this.xexHeader.Dispose();
            this.file = null;
            this.fileNameLength = 0;
            this.fileSize = 0;
            this.startingSectorOfFile = 0;
            this.offsetLeftSubTree = 0;
            this.offsetRightSubTree = 0;
        }
    }
}
