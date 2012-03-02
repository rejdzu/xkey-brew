using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Xbox.com_Downloader
{
    internal class WbClient : WebClient
    {
        public Dictionary<string, object> Cache = new Dictionary<string, object>();
        public Dictionary<string, string> RedirectURLCache = new Dictionary<string, string>();
        private byte[] data;
        public bool waiting;

        public string RedirectURL(string baseURL)
        {
            if (RedirectURLCache.ContainsKey(baseURL))
                return RedirectURLCache[baseURL];
            var uri = new Uri(baseURL);
            var httpreqRequest = (HttpWebRequest) WebRequest.Create(uri);
            httpreqRequest.Method = "GET";
            httpreqRequest.AllowAutoRedirect = true;
            RedirectURLCache.Add(baseURL, httpreqRequest.GetResponse().ResponseUri.AbsoluteUri);
            return RedirectURLCache[baseURL];
        }

        public byte[] DownloadData(string address, ProgressBar pb)
        {
            pb.Maximum = 100;
            base.DownloadProgressChanged +=
                (object s, DownloadProgressChangedEventArgs e) => pb.Value = e.ProgressPercentage;
            base.DownloadDataCompleted += (object s, DownloadDataCompletedEventArgs e) =>
                                              {
                                                  if (e.Error == null)
                                                  {
                                                      waiting = false;
                                                      data = e.Result;
                                                  }
                                              };
            base.DownloadDataAsync(new Uri(address));
            while (waiting)
            {
                Application.DoEvents();
            }
            return data;
        }

        public void DownloadFile(string address, string filename, ProgressBar pb)
        {
            pb.Maximum = 100;
            base.DownloadProgressChanged +=
                (object s, DownloadProgressChangedEventArgs e) => pb.Value = e.ProgressPercentage;
            base.DownloadFileCompleted += (object s, AsyncCompletedEventArgs e) => waiting = false;
            base.DownloadFileAsync(new Uri(address), filename);
            while (waiting)
            {
                Application.DoEvents();
            }
        }

        public new string DownloadString(string address)
        {
            if (Cache.ContainsKey(address))
                return Cache[address].ToString();

            base.Encoding = Encoding.UTF8;
            Cache.Add(address, base.DownloadString(address));
            return Cache[address].ToString();
        }
    }
}