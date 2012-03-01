using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace XkeyBrew.Utils.IsoGameInfo
{
    public class IsoGameInfo : IDisposable
    {
        private IsoType isoType;
        private XeXHeader xexHeader;
        private byte[] defaultXexFile;
        private string path;
        private GameInfo gameInfo;

        /// <summary>
        /// Header info of default.xex
        /// </summary>
        public XeXHeader XeXHeaderInfo { get { return xexHeader; } }

        /// <summary>
        /// Default.xex file as byte array
        /// </summary>
        public byte[] DefaultXeXFile { get { return defaultXexFile; } }

        /// <summary>
        /// Type of iso file
        /// </summary>
        public IsoType IsoType { get { return isoType; } }

        /// <summary>
        /// Get game info from xbox: images, title name, description
        /// </summary>
        public GameInfo GameInfo { get { return gameInfo; } }

        /// <summary>
        /// Main constructor of IsoGameInfo
        /// </summary>
        /// <param name="path">path to iso file</param>
        public IsoGameInfo(string path)
        {
            try
            {
                this.path = path;
                Iso iso = new Iso(path);
                
                this.xexHeader = ObjectCopier.Clone<XeXHeader>(iso.DefaultXeX.XeXHeader);
                this.defaultXexFile = new byte[iso.DefaultXeX.File.Length];
                iso.DefaultXeX.File.CopyTo(this.defaultXexFile, 0);
                this.isoType = iso.IsoType;
                this.gameInfo = new GameInfo(this.xexHeader.TitleId);

                iso.Dispose();
                iso = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Saves default.xex to desired path
        /// </summary>
        /// <param name="path">path to file, including file name</param>
        /// <returns>true if no errors</returns>
        public bool SaveDefaultXexToDisc(string path)
        {
            try
            {
                System.IO.File.WriteAllBytes(path, defaultXexFile);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Download cover from http://xbox.com
        /// </summary>
        /// <returns>Cover as Image</returns>
        public Image DownloadCoverFromXboxCom()
        {
            try
            {
                string url = "http://download.xbox.com/content/images/66acd000-77fe-1000-9115-d802%titleid%/1033/boxartlg.jpg";
                System.Net.WebClient client = new System.Net.WebClient();

                url = url.Replace("%titleid%", this.xexHeader.TitleId.ToLower());

                byte[] image = client.DownloadData(url);

                if (image != null)
                {
                    MemoryStream ms = new MemoryStream(image);
                    return Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading cover from www.xbox.com for iso: " + this.path, ex);
            }

            return null;
        }

        /// <summary>
        /// Download cover from http://covers.jqe360.com
        /// </summary>
        /// <returns>Front cover as Image</returns>
        public Image DownloadCoverFromJqe360()
        {
            return DownloadCoverFromJqe360(false);
        }

        /// <summary>
        /// Download cover from http://covers.jqe360.com
        /// </summary>
        /// <param name="frontAndBack">true - front and back; false - front</param>
        /// <returns>Cover as Image</returns>
        public Image DownloadCoverFromJqe360(bool frontAndBack)
        {
            try
            {
                string url = "http://covers.jqe360.com/Covers/%titleid%/cover.jpg";
                System.Net.WebClient client = new System.Net.WebClient();

                bool upper = false;
                bool lower = false;
                byte[] image = null;

                try
                {
                    string urlTmp = url.Replace("%titleid%", this.xexHeader.TitleId.ToUpper());
                    image = client.DownloadData(urlTmp);
                    upper = true;
                }
                catch { }

                if (!upper)
                {
                    try
                    {
                        string urlTmp = url.Replace("%titleid%", this.xexHeader.TitleId.ToLower());
                        image = client.DownloadData(urlTmp);
                        lower = true;
                    }
                    catch { }
                }

                if (upper && lower)
                    throw new Exception("Didn't find any pictures for this game");

                if (image != null)
                {
                    MemoryStream ms = new MemoryStream(image);
                    Image result = Image.FromStream(ms);

                    if (!frontAndBack)
                    {
                        result = CutFrontCover(result);
                    }

                    Image compressed = CompressFile(result);

                    if (compressed != null)
                        result = compressed;

                    return result;
                }
                else
                    throw new Exception("Didn't find any pictures for this game");
            }
            catch (Exception ex)
            {
                throw new Exception("Error downloading cover from covers.jqe360.com for iso: " + this.path, ex);
            }

            return null;
        }

        private Image CompressFile(Image image)
        {
            Image result = null;

            try
            {
                int maxLength = 204800;

                MemoryStream ms = new MemoryStream();

                ImageCodecInfo jpegCodec = GetCodecInfo();
                EncoderParameter qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)90);
                EncoderParameters encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = qualityParam;

                if (jpegCodec != null)
                {
                    image.Save(ms, jpegCodec, encoderParams);
                    int drop = 5;

                    while (ms.Length > maxLength || drop == 80)
                    {
                        ms = new MemoryStream();

                        encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)(90 - drop));
                        image.Save(ms, jpegCodec, encoderParams);
                        
                        drop += 5;
                    }

                    if(ms.Length <= maxLength)
                        result = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error compressing file to xkey limit", ex);
            }

            return result;
        }

        private ImageCodecInfo GetCodecInfo()
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            ImageCodecInfo jpegCodec = null;

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == "image/jpeg")
                {
                    jpegCodec = codecs[i];
                    break;
                }
            }

            return jpegCodec;
        }

        private Image CutFrontCover(Image image)
        {
            try
            {
                int compressionLevel = 10;

                int width = 425;
                int height = image.Height;

                int mainWidth = image.Width;
                int mainHeight = image.Height;

                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                
                g.Clear(Color.Transparent);
                g.DrawImage(image, new Rectangle(0, 0, width, height), new Rectangle(mainWidth - width, 0, width, height), GraphicsUnit.Pixel);

                MemoryStream ms = new MemoryStream();

                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                return (Image)bmp;
            }
            catch (Exception ex)
            {
                throw new Exception("Error cutting front cover covers.jqe360.com for iso: " + this.path, ex);
            }

            return null;
        }

        /// <summary>
        /// Disposes object
        /// </summary>
        public void Dispose()
        {
            path = String.Empty;
            isoType = IsoType.GDF;
            xexHeader = null;
            xexHeader.Dispose();
            defaultXexFile = null;
        }
    }
}
