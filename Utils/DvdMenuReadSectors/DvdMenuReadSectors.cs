using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace XkeyApp.Utils.DvdMenuReadSectors
{
    public class DvdMenuReadSectors
    {
        private FileStream fs = null;
        private BinaryReader br = null;
        private int sectorSize = 2048;
        private int pathOffset = 0x84;

        /// <summary>
        /// DvdMenuReadSectors constructor
        /// </summary>
        /// <param name="path">path to iso with dvd-video</param>
        public DvdMenuReadSectors(string path)
        {
            try
            {
                this.fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                this.br = new BinaryReader(this.fs);
            }
            catch (Exception ex)
            {
                throw new Exception("Error opening iso file", ex);
            }
        }

        /// <summary>
        /// Returns dictionary list with filename and sectors
        /// </summary>
        /// <returns>dictionary list key=file name as string; value=sector as byte[4]</returns>
        public Dictionary<string, byte[]> GetFilesWithSectors()
        {
            return GetFilesWithSectors(false);
        }

        /// <summary>
        /// Returns dictionary list with filename and sectors
        /// </summary>
        /// <param name="showLog">show form with log: true/false</param>
        /// <returns>dictionary list key=file name as string; value=sector as byte[4]</returns>
        public Dictionary<string, byte[]> GetFilesWithSectors(bool showLog)
        {
            Dictionary<string, byte[]> videoTSStructure = null;

            int videoTSLba = GetLBAOfVideoTS();
            if (videoTSLba > 0)
            {
                videoTSStructure = GetFilesFromVideoTS(videoTSLba);

                if (showLog)
                {
                    Form fm = new Form();
                    fm.Size = new System.Drawing.Size(800, 600);
                    TextBox tb = new TextBox();
                    tb.Multiline = true;
                    tb.Dock = DockStyle.Fill;
                    tb.ReadOnly = true;
                    tb.Text = PrepareLog(videoTSStructure);
                    tb.Font = new System.Drawing.Font("Courier New", 9f);
                    fm.Controls.Add(tb);
                    fm.Show();
                }
            }

            return videoTSStructure;
        }

        /// <summary>
        /// Prepare log from dictionary list with filename and sectors
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string PrepareLog(Dictionary<string, byte[]> input)
        {
            string log = "";

            log += "File name".PadRight(20);
            log += "Sector".PadRight(20);
            log += "\r\n";

            foreach (KeyValuePair<string, byte[]> kvp in input)
            {
                log += kvp.Key.PadRight(20);
                log += BitConverter.ToInt32(kvp.Value, 0).ToString().PadRight(20);
                log += "\r\n";
            }

            return log;
        }

        /// <summary>
        /// Get LBA sector of VIDEO_TS directory
        /// </summary>
        /// <returns>sector of VIDEO_TS directory</returns>
        private int GetLBAOfVideoTS()
        {
            int videoTSLBA = 0;

            try
            {
                this.br.BaseStream.Seek((16 * this.sectorSize) + this.pathOffset, SeekOrigin.Begin);

                int pathTableSize = br.ReadInt32();
                br.BaseStream.Seek(4L, SeekOrigin.Current);
                int pathTableLittleEndian = br.ReadInt32();

                long pathSector = (long)(sectorSize * (long)pathTableLittleEndian);

                this.br.BaseStream.Seek(pathSector, SeekOrigin.Begin);

                while (this.br.BaseStream.Position < pathSector + (long)pathTableSize)
                {
                    byte dirNameLength = this.br.ReadByte();
                    this.br.BaseStream.Seek(1L, SeekOrigin.Current);
                    int lba = this.br.ReadInt32();
                    short indexOfParent = this.br.ReadInt16();
                    string dirName = Encoding.ASCII.GetString(this.br.ReadBytes(dirNameLength));

                    if (dirNameLength % 2 == 1)
                        this.br.BaseStream.Seek(1L, SeekOrigin.Current);

                    if (dirName.ToUpper() == "VIDEO_TS")
                    {
                        videoTSLBA = lba;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching for VIDEO_TS", ex);
            }

            return videoTSLBA;
        }

        /// <summary>
        /// Gets dictionary list with filename and sectors of all files in VIDEO_TS folder of DVD-VIDEO
        /// </summary>
        /// <param name="videoTSLBA">VIDEO_TS LBA sector</param>
        /// <returns>dictionary list key=file name as string; value=sector as byte[4]</returns>
        private Dictionary<string, byte[]> GetFilesFromVideoTS(int videoTSLBA)
        {
            Dictionary<string, byte[]> videoTSStructure = new Dictionary<string, byte[]>();

            try
            {
                long videoTSSector = (long)(this.sectorSize * (long)videoTSLBA);

                this.br.BaseStream.Seek(videoTSSector, SeekOrigin.Begin);

                while (this.br.BaseStream.Position < videoTSSector + this.sectorSize)
                {
                    byte lengthDirectoryRecord = this.br.ReadByte();
                    if (lengthDirectoryRecord > 0)
                    {
                        byte extendedAtributeRecordLength = this.br.ReadByte();
                        byte[] sector = this.br.ReadBytes(4);
                        int locationOfExtent = BitConverter.ToInt32(sector, 0);
                        this.br.BaseStream.Seek(4L, SeekOrigin.Current);
                        int sizeOfExtent = this.br.ReadInt32();
                        this.br.BaseStream.Seek(4L, SeekOrigin.Current);
                        string dateTime = Encoding.ASCII.GetString(this.br.ReadBytes(7));
                        byte fileFlag = this.br.ReadByte();
                        byte fileUnitSizeInterleavedMode = this.br.ReadByte();
                        byte interleaveGapSize = this.br.ReadByte();
                        short volumeSequenceNumber = this.br.ReadInt16();
                        this.br.BaseStream.Seek(2L, SeekOrigin.Current);
                        byte lenghtOfFileName = this.br.ReadByte();
                        byte[] tmp = this.br.ReadBytes(lenghtOfFileName);

                        string fileName = Encoding.ASCII.GetString(tmp);

                        if (lenghtOfFileName == 1 && tmp[0] == 0)
                            fileName = ".";
                        else if (lenghtOfFileName == 1 && tmp[0] == 1)
                            fileName = "..";

                        if (lenghtOfFileName % 2 == 0)
                            this.br.BaseStream.Seek(1L, SeekOrigin.Current);

                        videoTSStructure.Add(fileName.Replace(";1", ""), sector);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error listing VIDEO_TS", ex);
            }

            return videoTSStructure;
        }
    }
}
