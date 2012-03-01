using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;
using System.Xml;
using XkeyBrew.Utils.Shared;

namespace XkeyBrew.Utils.IsoGameInfo
{
    /// <summary>
    /// Contains game information from web
    /// </summary>
    public class GameInfo
    {
        private string title_id;

        public string TitleId
        {
            get { return title_id; }
            set { title_id = value; }
        }

        private string smallBoxArt = "http://download.xbox.com/content/images/66acd000-77fe-1000-9115-d802%titleid%/1033/boxartsm.jpg";

        public string SmallBoxArt
        {
            get { return smallBoxArt.Replace("%titleid%", this.title_id); }
        }

        private Image smallBoxArtImage = null;

        public Image SmallBoxArtImage
        {
            get
            {
                return smallBoxArtImage;
            }
        }

        private string largeBoxArt = "http://download.xbox.com/content/images/66acd000-77fe-1000-9115-d802%titleid%/1033/boxartlg.jpg";

        public string LargeBoxArt
        {
            get { return largeBoxArt.Replace("%titleid%", this.title_id); }
        }

        private Image largeBoxArtImage = null;

        public Image LargeBoxArtImage
        {
            get
            {
                return largeBoxArtImage;
            }
        }

        private string banner = "http://download.xbox.com/content/images/66acd000-77fe-1000-9115-d802%titleid%/1033/banner.png";

        public string Banner
        {
            get { return banner.Replace("%titleid%", this.title_id); }
        }

        private Image bannerImage = null;

        public Image BannerImage
        {
            get
            {
                return bannerImage;
            }
        }

        private string gameUrl = "http://marketplace.xbox.com/en-US/Product/66acd000-77fe-1000-9115-d802%titleid%?nosplash=1";

        public string GameUrl
        {
            get { return gameUrl.Replace("%titleid%", this.title_id); }
        }

        private string gameDescription;

        public string GameDescription
        {
            get { return gameDescription; }
        }

        private string gameName;

        public string GameName
        {
            get { return gameName; }
        }

        public XmlDocument GameInfoXml
        {
            get 
            {
                XmlDocument xml = new XmlDocument();
                XmlDeclaration dec = xml.CreateXmlDeclaration("1.0", "utf-8", null);
                xml.AppendChild(dec);
                XmlElement root = xml.CreateElement("GameInfo");
                XmlElement titleId = xml.CreateElement("TitleId");
                titleId.InnerText = this.TitleId;
                XmlElement gameName = xml.CreateElement("Name");
                gameName.InnerText = this.GameName;
                XmlElement gameDescription = xml.CreateElement("Description");
                gameDescription.InnerText = this.GameDescription;
                root.AppendChild(titleId);
                root.AppendChild(gameName);
                root.AppendChild(gameDescription);
                xml.AppendChild(root);

                return xml;
            }
        }

        public GameInfo(string title_id)
        {
            this.title_id = title_id.ToLower();
            GetGameInfo();
        }

        private void GetGameInfo()
        {
            GetInfo();
            smallBoxArtImage = DownloadImage(SmallBoxArt);
            largeBoxArtImage = DownloadImage(LargeBoxArt);
            bannerImage = DownloadImage(Banner);
        }

        private void GetInfo()
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = Encoding.UTF8;

                string source = client.DownloadString(GameUrl);

                if (!ISharedMethods.IsObjectEmptyOrNull(source))
                {
                    Match overview1Match = Regex.Match(source, "<div id=\\\"overview1.*?<div id=\\\"overview2", RegexOptions.Singleline);
                    Match descriptionMatch = Regex.Match(overview1Match.Value, "<p>.+?</p>", RegexOptions.Singleline);
                    Match gameNameMatch = Regex.Match(overview1Match.Value, "<img alt=\\\".+?\\\"", RegexOptions.Singleline);
                    
                    gameDescription = HttpUtility.HtmlDecode(descriptionMatch.Value.Replace("<p>", "").Replace("</p>", ""));
                    gameName = HttpUtility.HtmlDecode(gameNameMatch.Value.Remove(gameNameMatch.Value.Length - 1).Replace("<img alt=\"", ""));
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Error downloading game description for Title id: " + this.title_id, ex);
            }
        }

        private string GetGameName()
        {
            string result = null;

            return result;
        }

        private Image DownloadImage(string url)
        {
            Image result = null;

            try
            {
                System.Net.WebClient client = new System.Net.WebClient();

                byte[] image = client.DownloadData(url);

                if (image != null)
                {
                    MemoryStream ms = new MemoryStream(image);
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                //throw new Exception("Error downloading image for Title id: " + this.title_id, ex);
            }

            return result;
        }
    }
}
