using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XkeyBrew.Utils.BLBinaryReader;

namespace XkeyApp.Utils.DvdReader
{
    class VTSM_PGCI_LU_MENU
    {
        private int startByteVTSM_PGCI;

        public int StartByteVTSM_PGCI
        {
            get { return startByteVTSM_PGCI; }
            set { startByteVTSM_PGCI = value; }
        }

        private int sector;

        public int Sector
        {
            get { return sector; }
            set { sector = value; }
        }

        private bool isGameMenu = false;

        public bool IsGameMenu
        {
            get { return isGameMenu; }
            set { isGameMenu = value; }
        }

        public VTSM_PGCI_LU_MENU(byte[] array, int numberOfMenu)
        {
            MemoryStream ms = new MemoryStream(array);
            MyBinaryReader br = new MyBinaryReader(ms);

            br.Skip(8 + (numberOfMenu * 8));
            //byte a = br.ReadByte();
            isGameMenu = br.ReadByte() == (byte)0;
            br.Skip(3);
            startByteVTSM_PGCI = br.ReadInt32B();

            br.BaseStream.Seek(startByteVTSM_PGCI + 0xE8, SeekOrigin.Begin);
            short offsetCellPlaybackInformation = br.ReadInt16B();

            br.BaseStream.Seek(startByteVTSM_PGCI + offsetCellPlaybackInformation, SeekOrigin.Begin);
            br.Skip(8);
            sector = br.ReadInt32B();
        }
    }
}
