using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Game1
{
    class GoogleMaps
    {
        internal static void DownloadMap(double latitude, double longitude, int zoom, string fileName)
        {
            DownloadMapWithoutLogo(latitude, longitude, zoom, fileName);
        }

        private static void DownloadMapWithoutLogo(double latitude, double longitude, int zoom, String fileName)
        {
            using (WebClient wc = new WebClient())
            {
                double offness = Math.Pow(0.5, zoom + 1);
                byte[] bytes = wc.DownloadData(String.Format("https://maps.googleapis.com/maps/api/staticmap?maptype=satellite&center={0},{1}&zoom={2}&size=256x256&scale=2&key=AIzaSyDePV-JaAnPp5S-2XV1TQgKiWIaMTgtXIo", latitude, longitude, zoom));
                MemoryStream ms = new MemoryStream(bytes);
                Image img1 = Image.FromStream(ms);
                byte[] bytes2 = wc.DownloadData(String.Format("https://maps.googleapis.com/maps/api/staticmap?maptype=satellite&center={0},{1}&zoom={2}&size=256x256&scale=2&key=AIzaSyDePV-JaAnPp5S-2XV1TQgKiWIaMTgtXIo", ToLat(ToY(latitude * Math.PI / 180) - offness) * 180 / Math.PI, longitude, zoom));
                MemoryStream ms2 = new MemoryStream(bytes2);
                Image img2 = Image.FromStream(ms2);
                Bitmap combined = new Bitmap(img1.Width, img1.Height);
                int width = img1.Width;
                int height = img1.Height;
                using (var bitmap = new Bitmap(width, height))
                {
                    using (var canvas = Graphics.FromImage(bitmap))
                    {
                        canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        canvas.DrawImage(img1,
                                         new Rectangle(0,
                                                       0,
                                                       width,
                                                       height),
                                         new Rectangle(0,
                                                       0,
                                                       width,
                                                       height),
                                         GraphicsUnit.Pixel);
                        canvas.DrawImage(img2, 0, height / 2);
                        canvas.Save();
                    }
                    try
                    {
                        bitmap.Save(fileName, ImageFormat.Png);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private static double ToLat(double y)
        {
            return 2 * Math.Atan(Math.Pow(Math.E, (y - 0.5) * 2 * Math.PI)) - Math.PI / 2;
        }

        private static double ToY(double lat)
        {
            return Math.Log(Math.Tan(lat / 2 + Math.PI / 4)) / (Math.PI * 2) + 0.5;
        }
    }
}
