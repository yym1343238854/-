using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi
{
    /// <summary>
    /// 网站设定
    /// </summary>
    public static class SiteConfig
    {
        /// <summary>
        /// 默认用户
        /// </summary>
        public static string DefaultUnionId => "ohxsKwPTVSwNHXYpOB43Xcv3jER4";

        /// <summary>
        /// 微信Id
        /// </summary>
        public static string WxOpenAppId
        {
            get
            {
                try
                {
                    string s = System.Configuration.ConfigurationManager.AppSettings["WeiXinAppId"].ToString();
                    return s;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// 微信秘钥
        /// </summary>
        public static string WxOpenAppSecret
        {
            get
            {
                try
                {
                    string s = System.Configuration.ConfigurationManager.AppSettings["WeiXinAppSecret"].ToString();
                    return s;
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
        }

        /// <summary>
        /// 图片基地址
        /// </summary>
        public static string BaseImageUrl => System.Configuration.ConfigurationManager.AppSettings["UserFilesRoot"];

        /// <summary>
        /// 最高正常体温
        /// </summary>
        public static decimal MaxNormalTemperature => (decimal)37.3;

    }
}