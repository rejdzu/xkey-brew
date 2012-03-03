using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using XkeyBrew.Utils.BLBinaryReader;

namespace XkeyApp.Utils.DvdReader
{
    class VTSM_PGCI_LU_MENUS : List<VTSM_PGCI_LU_MENU>
    {
        private short numberOfMenus;

        public short NumberOfMenus
        {
            get { return numberOfMenus; }
            set { numberOfMenus = value; }
        }

        private int endByteOfLU_MENUS;

        public int EndByteOfLU_MENUS
        {
            get { return endByteOfLU_MENUS; }
            set { endByteOfLU_MENUS = value; }
        }

        public VTSM_PGCI_LU_MENUS(byte[] array)
        {
            MemoryStream ms = new MemoryStream(array);
            MyBinaryReader br = new MyBinaryReader(ms);

            numberOfMenus = br.ReadInt16B();
            br.Skip(2);
            endByteOfLU_MENUS = br.ReadInt32B();

            for (int i = 0; i < numberOfMenus; i++)
            {
                VTSM_PGCI_LU_MENU menu = new VTSM_PGCI_LU_MENU(array, i);
                this.Add(menu);
            }
        }
    }
}
