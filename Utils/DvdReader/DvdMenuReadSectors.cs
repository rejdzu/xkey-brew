using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using XkeyBrew.Utils.BLBinaryReader;
using XkeyApp.Utils.DvdReader;

namespace XkeyBrew.Utils.DvdReader
{
    public class DvdMenuReadSectors
    {
        private FileStream fs = null;
        private MyBinaryReader br = null;
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
                this.br = new MyBinaryReader(this.fs);
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

        public List<byte[]> FillListWithMenuSectors()
        {
            List<byte[]> result = new List<byte[]>();
            Dictionary<string, byte[]> files = GetFilesWithSectors();

            foreach (KeyValuePair<string, byte[]> kvp in files)
            {
                if (kvp.Key.ToUpper().Contains("_0.IFO"))
                {
                    //sector of ifo file
                    int sector = BitConverter.ToInt32(kvp.Value, 0);
                    long address = (long)sector * this.sectorSize;

                    //sector of vob file
                    int sectorVob = GetSectorOfVtsVob(kvp.Key);

                    if (sector > 0 && sectorVob > 0)
                    {
                        // jumt to 0x0D to read start offset of VTSM_PGCI_UT
                        br.BaseStream.Seek(address + 0xD0, SeekOrigin.Begin);

                        // get sector
                        int startOffsetVTSM_PGCI_UT = br.ReadInt32B();
                        long addressVTSM_PGCI_UT = address + ((long)startOffsetVTSM_PGCI_UT * this.sectorSize);

                        // jump to VTSM_PGCI_UT
                        br.BaseStream.Seek(addressVTSM_PGCI_UT, SeekOrigin.Begin);
                        br.ReadInt32B();
                        int endByteVTSM_PGCI_UT = br.ReadInt32B();
                        long addressEndVTSM_PGCI_UT = addressVTSM_PGCI_UT + endByteVTSM_PGCI_UT;

                        br.BaseStream.Seek(addressVTSM_PGCI_UT, SeekOrigin.Begin);
                        VTSM_PGCI_UT titleSetPGCI = new VTSM_PGCI_UT(br.ReadBytes(endByteVTSM_PGCI_UT));

                        for (int i = 0; i < titleSetPGCI.Menus.Count; i++)
                        {
                            if (i % 2 == 1 && titleSetPGCI.Menus[i].IsGameMenu)
                            {
                                int gameSector = sectorVob + titleSetPGCI.Menus[i].Sector;
                                result.Add(BitConverter.GetBytes(gameSector));
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Search for VTS_xx_0.VOB
        /// </summary>
        /// <param name="ifoName">VTS_xx_0.IFO ifo file for VTS_xx_0.VOB</param>
        /// <returns></returns>
        private int GetSectorOfVtsVob(string ifoName)
        {
            Dictionary<string, byte[]> files = GetFilesWithSectors();

            foreach (KeyValuePair<string, byte[]> kvp in files)
            {
                if (kvp.Key == ifoName.Replace(".IFO", ".VOB"))
                {
                    return BitConverter.ToInt32(kvp.Value, 0);
                }
            }

            return -1;
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

                int dirLength = this.sectorSize;

                while (this.br.BaseStream.Position < videoTSSector + dirLength)
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
                        {
                            fileName = ".";
                            dirLength = sizeOfExtent;
                        }
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
