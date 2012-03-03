using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using XboxDvdMenu.Properties;
using System.Text.RegularExpressions;
using System.Web;
using System.Drawing.Drawing2D;
using XkeyBrew.Utils.IsoGameReader;

namespace XboxDvdMenu
{
    public class ISO
    {
        public string Filename;

        public string Gamename;

        public FileInfo IsoFile;

        public TextBox Log;

        public string Path;

        public ProgressBar ProgressBar1;

        public string RedirectURL;

        public string GAMETITLE;
        public string GAMEGENRE;
        public string GAMEDESC;
        public string GAMEIMAGE;
        public string GAMEBOX;
        public string TRAILER;
        public string JumpToSelectThisGame;
        public string JumpToTrailler;
        public string JumpToGameDetails;
        public int page;

        private string AddDiscDataToBanner(string bannerPath, int discNumber, int discCount)
        {
            int height;
            if (discCount <= 1)
            {
                return bannerPath;
            }
            else
            {
                int scale = 30;
                byte[] bannerArray = File.ReadAllBytes(bannerPath);
                MemoryStream bannerMs = new MemoryStream(bannerArray);
                Image banner = Image.FromStream(bannerMs);
                byte[] discIconArray = File.ReadAllBytes(string.Concat(Application.StartupPath, "\\media\\disc.png"));
                MemoryStream discIconMs = new MemoryStream(discIconArray);
                ISO sO = this;
                Image image = Image.FromStream(discIconMs);
                if (banner.Height < scale)
                {
                    height = banner.Height - 4;
                }
                else
                {
                    height = scale;
                }
                Image discIcon = sO.Resize(image, height);
                byte[] discCurrentIconArray = File.ReadAllBytes(string.Concat(Application.StartupPath, "\\media\\disc_current.png"));
                MemoryStream discCurrentIconMs = new MemoryStream(discCurrentIconArray);
                Image discCurrentIcon = this.Resize(Image.FromStream(discCurrentIconMs), scale);
                int offset = 10;
                Bitmap bitmapDisc = new Bitmap((discCount - 1) * 10 + discIcon.Width, discIcon.Height);
                Graphics g = Graphics.FromImage(bitmapDisc);
                for (int i = 0; i < discCount; i++)
                {
                    if (discNumber - 1 != i)
                    {
                        g.DrawImage(discIcon, i * offset, 0, discIcon.Height, discIcon.Width);
                    }
                    else
                    {
                        g.DrawImage(discCurrentIcon, i * offset, 0, discIcon.Height, discIcon.Width);
                    }
                }
                Graphics gr = Graphics.FromImage(banner);
                gr.DrawImage(bitmapDisc, banner.Width - bitmapDisc.Width - 2, 2, bitmapDisc.Width, bitmapDisc.Height);
                bannerPath = string.Concat(Application.StartupPath, "\\temp\\", this.Filename.Replace(this.IsoFile.Extension, "-banner.png"));
                banner.Save(bannerPath, ImageFormat.Png);
                return bannerPath;
            }
        }

        public string GameBanner(bool chkArtwork)
        {
            string banner;
            string bannerPath = this.GameBannerBasic(chkArtwork);
            try
            {
                IsoGameInfo isoFile = new IsoGameInfo(this.Path);
                banner = this.AddDiscDataToBanner(this.GameBannerBasic(chkArtwork), isoFile.XeXHeaderInfo.DiscNumber, isoFile.XeXHeaderInfo.DiscCount);
            }
            catch (Exception)
            {
                banner = bannerPath;
            }
            return banner;
        }

        public string GameBannerBasic(bool chkArtwork)
        {
            string str;
            TextBox log;
            TextBox textBox = this.Log;
            textBox.Text = string.Concat(textBox.Text, "Loading banner:", this.Path, Environment.NewLine);
            if (!File.Exists(string.Concat(this.Path.Replace(this.IsoFile.Extension, ""), "-banner.png")))
            {
                WaffleXML waffle = new WaffleXML(this.Path.Replace(this.IsoFile.Extension, ".xml"));
                if (waffle.Banner == null)
                {
                    if (chkArtwork)
                    {
                        Iso iso = new Iso(this.IsoFile.FullName, true);
                        try
                        {
                            if (iso.DefaultXeX != null)
                            {
                                TextBox log1 = this.Log;
                                log1.Text = string.Concat(log1.Text, "Searching Xbox.com for banner:", this.Path, Environment.NewLine);
                                WbClient wc = new WbClient();

                                byte[] data = wc.DownloadData(string.Concat("http://download.xbox.com/content/images/66acd000-77fe-1000-9115-d802", iso.DefaultXeX.XeXHeader.TitleId.ToLower(), "/1033/banner.png"));
                                if (data == null)
                                {
                                    throw new Exception("Download Failed");
                                }
                                else
                                {
                                    waffle.Banner = data;
                                    TextBox textBox1 = this.Log;
                                    textBox1.Text = string.Concat(textBox1.Text, "Banner saved to XML File", this.Path, Environment.NewLine);
                                    str = this.GameBanner(true);
                                }
                            }
                            else
                            {
                                throw new Exception("Not a Game");
                            }
                        }
                        catch (Exception)
                        {
                            TextBox log2 = this.Log;
                            log2.Text = string.Concat(log2.Text, "Banner Download Failed:", this.Path, Environment.NewLine);
                            log = this.Log;
                            log.Text = string.Concat(log.Text, "No banner found:", this.Path, Environment.NewLine);
                            return "media\\blank-banner.png";
                        }
                        return str;
                    }
                    log = this.Log;
                    log.Text = string.Concat(log.Text, "No banner found:", this.Path, Environment.NewLine);
                    return "media\\blank-banner.png";
                }
                else
                {
                    StreamWriter binWritter = new StreamWriter(string.Concat(Application.StartupPath, "\\temp\\", this.Filename.Replace(this.IsoFile.Extension, "-banner.png")), false);
                    binWritter.BaseStream.Write(waffle.Banner, 0, (int)waffle.Banner.Length);
                    binWritter.Flush();
                    binWritter.Close();
                    TextBox textBox2 = this.Log;
                    textBox2.Text = string.Concat(textBox2.Text, "Banner found in XML:", this.Path, Environment.NewLine);
                    return string.Concat(Application.StartupPath, "\\temp\\", this.Filename.Replace(this.IsoFile.Extension, "-banner.png"));
                }
            }
            else
            {
                TextBox log3 = this.Log;
                log3.Text = string.Concat(log3.Text, "Local Banner Found:", this.Path, Environment.NewLine);
                return string.Concat(this.Path.Replace(this.IsoFile.Extension, ""), "-banner.png");
            }
        }

        public string GameBox(bool chkArtwork)
        {
            string str;
            TextBox log;
            TextBox textBox = this.Log;
            textBox.Text = string.Concat(textBox.Text, "Finding cover for:", this.Path, Environment.NewLine);
            if (!File.Exists(string.Concat(this.Path.Replace(this.IsoFile.Extension, ""), "-cover.jpg")))
            {
                WaffleXML waffle = new WaffleXML(this.Path.Replace(this.IsoFile.Extension, ".xml"));
                if (waffle.BoxArt == null)
                {
                    if (chkArtwork)
                    {
                        Iso iso = new Iso(this.IsoFile.FullName, true);
                        try
                        {
                            if (iso.DefaultXeX != null)
                            {
                                TextBox log1 = this.Log;
                                log1.Text = string.Concat(log1.Text, "Searching Xbox.com for cover", this.Path, Environment.NewLine);
                                WbClient wc = new WbClient();
                                byte[] data = wc.DownloadData(string.Concat("http://tiles.xbox.com/consoleAssets/", iso.DefaultXeX.XeXHeader.TitleId.ToLower(), "/en-US/largeboxart.jpg"));
                                if (data != null)
                                {
                                    waffle.BoxArt = data;
                                    TextBox textBox1 = this.Log;
                                    textBox1.Text = string.Concat(textBox1.Text, "Cover saved to XML", this.Path, Environment.NewLine);
                                    str = this.GameBox(chkArtwork);
                                }
                                else
                                {
                                    throw new Exception("Download Failed");
                                }
                            }
                            else
                            {
                                throw new Exception("Not a Game");
                            }
                        }
                        catch (Exception)
                        {
                            TextBox log2 = this.Log;
                            log2.Text = string.Concat(log2.Text, "Download Failed", this.Path, Environment.NewLine);
                            log = this.Log;
                            log.Text = string.Concat(log.Text, "No Cover found", this.Path, Environment.NewLine);
                            return "media\\blank-cover.jpg";
                        }
                        return str;
                    }
                    log = this.Log;
                    log.Text = string.Concat(log.Text, "No Cover found", this.Path, Environment.NewLine);
                    return "media\\blank-cover.jpg";
                }
                else
                {
                    StreamWriter binWritter = new StreamWriter(string.Concat(Application.StartupPath, "\\temp\\", this.Filename.Replace(this.IsoFile.Extension, "-cover.jpg")), false);
                    binWritter.BaseStream.Write(waffle.BoxArt, 0, (int)waffle.BoxArt.Length);
                    binWritter.Flush();
                    binWritter.Close();
                    TextBox textBox2 = this.Log;
                    textBox2.Text = string.Concat(textBox2.Text, "Found in XML", this.Path, Environment.NewLine);
                    return string.Concat(Application.StartupPath, "\\temp\\", this.Filename.Replace(this.IsoFile.Extension, "-cover.jpg"));
                }
            }
            else
            {
                TextBox log3 = this.Log;
                log3.Text = string.Concat(log3.Text, "Local cover found", this.Path, Environment.NewLine);
                return string.Concat(this.Path.Replace(this.IsoFile.Extension, ""), "-cover.jpg");
            }
        }

        public string GameDesc(bool chkArtwork)
        {
            TextBox log = this.Log;
            log.Text = string.Concat(log.Text, "Finding Desc", this.Path, Environment.NewLine);
            WaffleXML waffleXMLFile = new WaffleXML(this.Path.Replace(this.IsoFile.Extension, ".xml"));
            if (waffleXMLFile.Summary == "")
            {
                if (chkArtwork)
                {
                    try
                    {
                        Iso iso = new Iso(this.IsoFile.FullName, true);
                        if (iso.DefaultXeX == null)
                        {
                        }
                        else
                        {
                            TextBox textBox = this.Log;
                            textBox.Text = string.Concat(textBox.Text, "Xbox.com Desc", this.Path, Environment.NewLine);
                            WbClient wc = new WbClient();


                            string page = wc.DownloadString(wc.RedirectURL(this,
                                           "http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" + iso.DefaultXeX.XeXHeader.TitleId.ToLower() + "?nosplash=1"));
                            Regex replacer = new Regex("<meta name=\"description\" content=\".*\" />");
                            Match results = replacer.Match(page);
                            string desc = results.Value.Substring(34);
                            desc = desc.Substring(0, desc.Length - 5).Trim();
                            waffleXMLFile.Summary = desc;
                            TextBox log1 = this.Log;
                            log1.Text = string.Concat(log1.Text, "Found", this.Path, Environment.NewLine);
                            string str = desc;
                            return str;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                TextBox textBox1 = this.Log;
                textBox1.Text = string.Concat(textBox1.Text, "Not Found", this.Path, Environment.NewLine);
                return "";
            }
            else
            {
                TextBox log2 = this.Log;
                log2.Text = string.Concat(log2.Text, "In XML", this.Path, Environment.NewLine);
                return waffleXMLFile.Summary;
            }
        }

        public string GameGenre(bool chkArtwork)
        {
            TextBox log = this.Log;
            log.Text = string.Concat(log.Text, "Finding Genre", this.Path, Environment.NewLine);
            WaffleXML waffle = new WaffleXML(this.Path.Replace(this.IsoFile.Extension, ".xml"));
            if (waffle.InfoItem("Genre") == "")
            {
                if (chkArtwork)
                {
                    try
                    {
                        Iso iso = new Iso(this.IsoFile.FullName, true);
                        if (iso.DefaultXeX != null)
                        {
                            TextBox textBox = this.Log;
                            textBox.Text = string.Concat(textBox.Text, "Xbox.com Search", this.Path, Environment.NewLine);
                            WbClient wc = new WbClient();
                            string page = wc.DownloadString(wc.RedirectURL(this,
                                                   "http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" + iso.DefaultXeX.XeXHeader.TitleId.ToLower() + "?nosplash=1"));
                            if (page.IndexOf("Genre:", StringComparison.Ordinal) == 0)
                            {
                            }
                            else
                            {
                                int startIndex = page.IndexOf("Genre:") + 6;
                                string genre = page.Substring(startIndex);
                                genre = genre.Substring(0, genre.IndexOf("</li>"));
                                genre = HttpUtility.HtmlDecode(genre.Trim().Replace("\n", "")).Replace("<li>", "").Replace("</label>", "");
                                waffle.InfoItem("Genre", genre);
                                TextBox log1 = this.Log;
                                log1.Text = string.Concat(log1.Text, "Found", this.Path, Environment.NewLine);
                                string str = genre;
                                return str;
                            }
                        }
                        else
                        {
                            throw new Exception("Not a Game");
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                TextBox textBox1 = this.Log;
                textBox1.Text = string.Concat(textBox1.Text, "No Genre Found", this.Path, Environment.NewLine);
                return "";
            }
            else
            {
                TextBox log2 = this.Log;
                log2.Text = string.Concat(log2.Text, "Found in XML file", this.Path, Environment.NewLine);
                return waffle.InfoItem("Genre");
            }
        }

        public string GameTitle(bool chkArtwork)
        {
            TextBox log = this.Log;
            log.Text = string.Concat(log.Text, "Finding Title", this.Path, Environment.NewLine);
            WaffleXML waffle = new WaffleXML(this.Path.Replace(this.IsoFile.Extension, ".xml"));
            if (waffle.Title == "")
            {
                if (chkArtwork)
                {
                    try
                    {
                        Iso iso = new Iso(this.IsoFile.FullName, true);
                        if (iso.DefaultXeX != null)
                        {
                            TextBox textBox = this.Log;
                            textBox.Text = string.Concat(textBox.Text, "Xbox.com search", this.Path, Environment.NewLine);
                            WbClient wc = new WbClient();
                            string page = wc.DownloadString(wc.RedirectURL(this,
                                               "http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" + iso.DefaultXeX.XeXHeader.TitleId.ToLower() + "?nosplash=1"));
                            if (page.IndexOf("<title>") == 0)
                            {
                            }
                            else
                            {
                                int startIndex = page.IndexOf("<title>") + 7;
                                string title = page.Substring(startIndex);
                                title = title.Substring(0, title.IndexOf(" - Xbox.com"));
                                title = HttpUtility.HtmlDecode(title.Trim().Replace("\n", "")).Replace("&", "&amp;");
                                waffle.Title = title;
                                string str = title;
                                return str;
                            }
                        }
                        else
                        {
                            throw new Exception("Not a Game");
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                TextBox log1 = this.Log;
                log1.Text = string.Concat(log1.Text, "Using filename", this.Path, Environment.NewLine);
                return this.Gamename;
            }
            else
            {
                TextBox textBox1 = this.Log;
                textBox1.Text = string.Concat(textBox1.Text, "Found in XML", this.Path, Environment.NewLine);
                return waffle.Title;
            }
        }

        public string GameTrailer(bool chkTrailers)
        {
            TextBox log = this.Log;
            log.Text = string.Concat(log.Text, "Finding Trailler", this.Path, Environment.NewLine);
            if (!File.Exists(this.Path.Replace(this.IsoFile.Extension, ".wmv")))
            {
                if (chkTrailers)
                {
                    Iso isoGame = new Iso(this.IsoFile.FullName, true);
                    if (isoGame.DefaultXeX != null)
                    {
                        try
                        {
                            TextBox textBox = this.Log;
                            textBox.Text = string.Concat(textBox.Text, "Xbox.com", this.Path, Environment.NewLine);
                            WbClient wc = new WbClient();
                            string page = wc.DownloadString(wc.RedirectURL(this,
                                   ("http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" +
                                                 isoGame.DefaultXeX.XeXHeader.TitleId.ToLower()) + "?nosplash=1"));
                            Regex regexReplacer = new Regex("addVideo\\('[^,]*,");
                            Match regexmatchResult = regexReplacer.Match(page);
                            if (!regexmatchResult.Success)
                            {
                            }
                            else
                            {
                                string strVideoAsx = wc.DownloadString(regexmatchResult.Value.Replace("\\x3a", ":").Replace("\\x2f", "/").Substring(10).Replace(",", "").Trim());
                                regexReplacer = new Regex("href=\"[^\"]*\"");
                                regexmatchResult = regexReplacer.Match(strVideoAsx);
                                TextBox log1 = this.Log;
                                log1.Text = string.Concat(log1.Text, "Downloading", this.Path, Environment.NewLine);
                                string strVideoUrl = regexmatchResult.Value.Replace("href=\"", "").Replace("\"", "");
                                wc.DownloadFile(strVideoUrl, this.Path.Replace(this.IsoFile.Extension, ".wmv"), this.ProgressBar1);
                                string str = this.Path.Replace(this.IsoFile.Extension, ".wmv");
                                return str;
                            }
                        }
                        catch (Exception)
                        {
                            this.ProgressBar1.Value = 0;
                            TextBox textBox1 = this.Log;
                            textBox1.Text = string.Concat(textBox1.Text, "Failed", this.Path, Environment.NewLine);
                        }
                    }
                }
                TextBox log2 = this.Log;
                log2.Text = string.Concat(log2.Text, "Not Found", this.Path, Environment.NewLine);
                return "media\\blank.mpg";
            }
            else
            {
                TextBox textBox2 = this.Log;
                textBox2.Text = string.Concat(textBox2.Text, "Local", this.Path, Environment.NewLine);
                return this.Path.Replace(this.IsoFile.Extension, ".wma");
            }
        }

        public Image Resize(Image srcImage, int newSize)
        {
            Bitmap newImage = new Bitmap(newSize, newSize);
            Graphics gr = Graphics.FromImage(newImage);
            using (gr)
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(srcImage, new Rectangle(0, 0, newSize, newSize));
            }
            return newImage;
        }
    }
}