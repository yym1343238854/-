using Codans.Helper.Logs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Business;
using Newtonsoft.Json;

namespace TinyApi.Core
{
    public static class Helper
    {
        private static Random random = new Random();
        /// <summary>
        /// 随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="base64EncodedData">Base64字符串</param>
        /// <returns></returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string FormatPrivateContent(this string source)
        {
            if (string.IsNullOrEmpty(source)) return "";
            var content = source.Replace("[private]", $"【以下内容是保密的】");
            content = content.Replace("[/private]", $"【保密内容结束】");
            return content;
        }

        public static string RemovePrivateContent(this string source)
        {
            if (string.IsNullOrEmpty(source)) return "";
            var pattern = @"\[private\].*\[\/private\]";
            return Regex.Replace(source, pattern, "", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
        }

        /// <summary>
        /// Remove HTML from string with Regex.
        /// </summary>
        public static string RemoveHtmlTags(this string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty).RecursiveHtmlDecode().FormatPrivateContent();
        }

        public static string RecursiveHtmlDecode(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;
            var tmp = HttpUtility.HtmlDecode(str);
            while (tmp != str)
            {
                str = tmp;
                tmp = HttpUtility.HtmlDecode(str);
            }
            return str; //completely decoded string
        }

        /// <summary>
        /// 文件迁移
        /// </summary>
        /// <param name="path">目录地址</param>
        /// <param name="localFileName">老地址</param>
        /// <param name="newFileName">新地址</param>
        public static void FileMove(string path, string localFileName, string newFileName)
        {
            CreateDirectory(path);//创建目录

            if (File.Exists(newFileName))
            {
                File.Delete(newFileName);
            }
            File.Move(localFileName, newFileName); //Save to disk.
        }

        /// <summary>
        /// 图片地址拼接
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FixImagePath(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                if (source.Contains("http"))
                {
                    return String.Format(source);
                }
                else
                {
                    if (!source.StartsWith("/"))
                    {
                        return $"{SiteConfig.BaseImageUrl}/{source}";
                    }
                    else
                    {
                        return $"{SiteConfig.BaseImageUrl}{source}";
                    }
                }
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 获取挑战时垃圾桶（对应城市的）照片
        /// </summary>
        /// <param name="cityId"></param>
        /// <param name="orderNo"></param>
        /// <returns></returns>
        public static string GetGarbageCanBg(Guid cityId, int orderNo)
        {
            var url = $"/Userfiles/City/{cityId}/GarbageCanBg/{orderNo:D2}.png";
            return url.FixImagePath();
        }

        /// <summary>
        /// 修复unionId
        /// </summary>
        /// <param name="unionId"></param>
        public static string FixUnionId(this string unionId)
        {
            var s = unionId.Replace("-", "_");
            return s;
        }

        public static DateTime TimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <param name="savePath"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool DownloadImageFromUrl(string imageUrl, string savePath, out string error)
        {
            error = "";
            try
            {
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                System.Net.WebResponse webResponse = webRequest.GetResponse();

                System.IO.Stream stream = webResponse.GetResponseStream();

                var image = System.Drawing.Image.FromStream(stream);

                webResponse.Close();

                var saveFolder = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(saveFolder))
                {
                    Directory.CreateDirectory(saveFolder);
                }

                image.Save(savePath, ImageFormat.Jpeg);

                return true;
            }
            catch (Exception ex)
            {
                error = ex.ToString();
                return false;
            }
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }

        /// <summary>
        /// 格式化内容
        /// </summary>
        /// <param name="source"></param>
        /// <param name="isSummary"></param>
        /// <returns></returns>
        public static string FormatContent(this string source, bool isSummary = false)
        {
            if (source == null) return "";
            var sb = new StringBuilder();
            var lines = source.Split('\n');
            var lineCount = 0;
            foreach (var line in lines)
            {
                if (!line.StartsWith("  "))
                {
                    sb.AppendLine($"  {line.Trim()}");
                }
                else if (!line.StartsWith("  "))
                {
                    sb.AppendLine($"  {line}");
                }
                else
                {
                    sb.AppendLine(line + Environment.NewLine);
                }
                lineCount++;
                if (isSummary)
                {
                    if (lineCount>=2)
                    {
                        sb.AppendLine("......");
                        break;
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 美化时间段
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>string</returns>
        public static string BeautyTimeSpan(this DateTime time)
        {
            string timeSpan;

            DateTime sendTime = time;
            DateTime currentTime = DateTime.Now;

            TimeSpan span = currentTime.Subtract(sendTime);
            int day = span.Days;
            int hour = span.Hours;
            int minute = span.Minutes;
            int second = span.Seconds;

            if (day > 31)
            {
                var years = (int)(span.TotalDays / 365);
                if (years > 0)
                {
                    timeSpan = $"{years}年前";
                }
                else
                {
                    var month = (int)(span.TotalDays / 30);
                    timeSpan = $"{month}个月前";
                }
            }
            else if (day > 7)
            {
                var weeks = (int)(day / 7);
                timeSpan = $"{weeks}周前";
            }
            else if (day > 0 && day <= 7)
            {
                timeSpan = day.ToString() + "天前";
            }
            else if (hour != 0)
            {
                timeSpan = hour.ToString() + "小时前";
            }
            else if (minute != 0)
            {
                timeSpan = minute.ToString() + "分钟前";
            }
            else
            {
                if (second == 0)
                {
                    second = 1;
                }

                timeSpan = second.ToString() + "秒钟前";
            }

            return timeSpan;
        }

        /// <summary>
        /// 格式化秒到时间
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatSeconds(this int seconds)
        {
            var minutes = seconds / 60;
            var leftSeconds = seconds - minutes * 60;
            return $"{minutes:D2}:{leftSeconds:D2}";
        }

        /// <summary>
        /// 获取星期名称
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        public static string GetWeekName(this DateTime day)
        {
            string weekName = "";
            switch (day.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    weekName = "M";
                    break;
                case DayOfWeek.Tuesday:
                    weekName = "T";
                    break;
                case DayOfWeek.Wednesday:
                    weekName = "W";
                    break;
                case DayOfWeek.Thursday:
                    weekName = "T";
                    break;
                case DayOfWeek.Friday:
                    weekName = "F";
                    break;
                case DayOfWeek.Saturday:
                    weekName = "S";
                    break;
                case DayOfWeek.Sunday:
                    weekName = "S";
                    break;
            }
            return weekName;
        }


        /// <summary>
        /// 格式化内容
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FormatContentToHtml(this string source)
        {
            var sb = new StringBuilder();
            var lines = source.Split('\n');
            foreach (var line in lines)
            {
                sb.AppendLine($"<p>{line}</p>");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将分钟折算成日小时分钟格式
        /// </summary>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static string FormatMinutes(int minutes)
        {
            if (minutes <= 0)
            {
                return "0分钟";
            }
            else
            {
                var days = minutes / 1440;
                var hours = (minutes - days * 1440) / 60;
                var leftMinutes = minutes - (days * 1440) - hours * 60;
                var sb = new StringBuilder();
                if (days > 0)
                {
                    sb.Append($"{days}天");
                }

                if (hours > 0)
                {
                    sb.Append($"{hours}小时");
                }

                if (leftMinutes > 0)
                {
                    sb.Append($"{leftMinutes}分钟");
                }

                return sb.ToString();
            }
        }


        /// <summary> 
        ///  生成缩略图 静态方法    
        /// </summary> 
        /// <param name="pathImageFrom"> 源图的路径(含文件名及扩展名) </param> 
        /// <param name="pathImageTo"> 生成的缩略图所保存的路径(含文件名及扩展名) 
        ///                            注意：扩展名一定要与生成的缩略图格式相对应 </param>
        /// <param name="pathThumb">缩略图</param>
        /// <param name="maxLength"> 欲生成的缩略图 “画布” 的宽度(像素值) </param> 
        /// <param name="error"></param> 
        public static bool GenerateThumbnail(string pathImageFrom, string pathImageTo, string pathThumb, int maxLength, out string error, out int width, out int height)
        {
            var xLog = new Logger();
            xLog.AddLine("======= Create Thumbnail ==========");
            xLog.AddLine($"From  :{pathImageFrom}");
            xLog.AddLine($"To    :{pathImageTo}");
            xLog.AddLine($"Thumb :{pathThumb}");
            xLog.AddLine($"maxLength :{maxLength}");

            const int defaultThumbWidth = 120;
            width = 0;
            height = 0;

            error = "";
            var sb = new StringBuilder();
            try
            {
                if (File.Exists(pathImageTo))
                {
                    File.Delete(pathImageTo);
                }
                if (File.Exists(pathThumb))
                {
                    File.Delete(pathThumb);
                }

                var imageFrom = Image.FromFile(pathImageFrom);


                if (imageFrom.PropertyIdList.Contains(0x0112))
                {
                    int rotationValue = imageFrom.GetPropertyItem(0x0112).Value[0];
                    sb.AppendLine("RotationValue:" + imageFrom.GetPropertyItem(0x0112).Value[0]);
                    switch (rotationValue)
                    {
                        case 1: // landscape, do nothing
                            sb.AppendLine("Landscape");
                            break;

                        case 8: // rotated 90 right
                            // de-rotate:
                            sb.AppendLine("90 right");
                            imageFrom.RotateFlip(rotateFlipType: RotateFlipType.Rotate270FlipNone);
                            break;

                        case 3: // bottoms up
                            sb.AppendLine("Up");
                            imageFrom.RotateFlip(rotateFlipType: RotateFlipType.Rotate180FlipNone);
                            break;

                        case 6: // rotated 90 left
                            sb.AppendLine("90 Left");
                            imageFrom.RotateFlip(rotateFlipType: RotateFlipType.Rotate90FlipNone);
                            break;
                    }
                }

                // 源图宽度及高度 
                var imageFromWidth = imageFrom.Width;
                var imageFromHeight = imageFrom.Height;
                // 生成的缩略图实际宽度及高度 

                var bitmapWidth = maxLength;
                var bitmapHeight = (int)((double)imageFromHeight * (double)maxLength / (double)imageFromWidth);

                if (bitmapWidth == 1280 && bitmapHeight == 961)
                    bitmapHeight = 960;
                if (imageFromWidth < imageFromHeight)
                {
                    bitmapHeight = maxLength;
                    bitmapWidth = (int)((double)imageFromWidth * (double)maxLength / (double)imageFromHeight);
                }
                // 生成的缩略图在上述”画布”上的位置 

                width = bitmapWidth;
                height = bitmapHeight;

                // 创建画布 
                var bmp = new Bitmap(bitmapWidth, bitmapHeight);
                var g = Graphics.FromImage(bmp);
                // 用白色清空 
                g.Clear(Color.White);
                // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
                g.InterpolationMode = InterpolationMode.Default;
                // 指定高质量、低速度呈现。 
                g.SmoothingMode = SmoothingMode.HighSpeed;
                // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。 
                g.DrawImage(imageFrom, new Rectangle(0, 0, bitmapWidth, bitmapHeight),
                    new Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);

                bmp.Save(pathImageTo, ImageFormat.Png);
                bmp.Dispose();
                g.Dispose();

                //创建缩略图
                if (!string.IsNullOrEmpty(pathThumb))
                {

                    bitmapWidth = defaultThumbWidth;
                    bitmapHeight = (int)((double)imageFromHeight * (double)defaultThumbWidth / (double)imageFromWidth);
                    // 生成的缩略图在上述”画布”上的位置 
                    // 创建画布 
                    bmp = new Bitmap(bitmapWidth, bitmapHeight);
                    g = Graphics.FromImage(bmp);
                    // 用白色清空 
                    g.Clear(Color.White);
                    // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    // 指定高质量、低速度呈现。 
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。 
                    g.DrawImage(imageFrom, new Rectangle(0, 0, bitmapWidth, bitmapHeight),
                        new Rectangle(0, 0, imageFromWidth, imageFromHeight), GraphicsUnit.Pixel);

                    bmp.Save(pathThumb, ImageFormat.Jpeg);
                    bmp.Dispose();
                    g.Dispose();
                }

                imageFrom.Dispose();


                error = sb.ToString();
                xLog.AddLine("Done. " + error);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                xLog.AddLine("Exception:" + Environment.NewLine + ex.ToString());
                return false;
            }
            finally
            {
                xLog.Save();
            }
        }



    }
}