using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XkeyBrew.Utils.BLBinaryReader;

namespace XkeyApp.Utils.DvdReader
{
    class VTSM_PGCI_UT
    {
        private int numberOfVTSM_PGCI_LU;

        public int NumberOfVTSM_PGCI_LU
        {
            get { return numberOfVTSM_PGCI_LU; }
            set { numberOfVTSM_PGCI_LU = value; }
        }

        private int endByteOfVTSM_PGCI_LU;

        public int EndByteOfVTSM_PGCI_LU
        {
            get { return endByteOfVTSM_PGCI_LU; }
            set { endByteOfVTSM_PGCI_LU = value; }
        }

        private VTSM_PGCI_LU_MENUS menus;

        public VTSM_PGCI_LU_MENUS Menus
        {
            get { return menus; }
            set { menus = value; }
        }

        public VTSM_PGCI_UT(byte[] array)
        {
            try
            {
                MemoryStream ms = new MemoryStream(array);
                MyBinaryReader br = new MyBinaryReader(ms);

                numberOfVTSM_PGCI_LU = br.ReadInt16B();
                br.Skip(2);
                endByteOfVTSM_PGCI_LU = br.ReadInt32B();
                br.Skip(4);
                int LU_1startByte = br.ReadInt32B();
                br.BaseStream.Seek(LU_1startByte, SeekOrigin.Begin);
                menus = new VTSM_PGCI_LU_MENUS(br.ReadBytes(array.Length - LU_1startByte));
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching sectors in DVD MENU", ex);
            }
        }
    }
}
