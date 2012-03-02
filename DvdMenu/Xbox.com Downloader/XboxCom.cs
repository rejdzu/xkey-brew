using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XboxDvdMenu;

namespace Xbox.com_Downloader
{
    public class XboxCom : IMediaDownloader
    {
        public byte[] DownloadBanner(string gameID, string filename)
        {
            return null;
        }

        public byte[] DownloadCover(string gameID, string filename)
      {
          return null;
      }

        public byte[] DownloadTrailer(string gameID, string filename)
        {
            return null;
        }

        public string DownloadTitle(string gameID, string filename)
        {
            return null;
        }
        public string DownloadDesc(string gameID, string filename)
        {
            return null;
        }
        public string DownloadProp(string property, string gameID, string filename)
        {
            return null;
        }

    }
}
