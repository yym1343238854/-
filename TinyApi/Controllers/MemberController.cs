using Business;
using Codans.Helper.Logs;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using TinyApi.Models.Input;
using TinyApi.Models.Result;

namespace TinyApi.Controllers
{
    /// <summary>
    /// 用户操作
    /// </summary>
    public class MemberController : ApiController
    {
        /// <summary>
        /// 与微信小程序账号后台的AppId设置保持一致，区分大小写。
        /// </summary>
        public static readonly string WxOpenAppId = SiteConfig.WxOpenAppId;

        /// <summary>
        /// 与微信小程序账号后台的AppSecret设置保持一致，区分大小写
        /// </summary>
        public static readonly string WxOpenAppSecret = SiteConfig.WxOpenAppSecret;

        #region 登录 Login
        /// <summary>
        /// 登录（获取基础信息）  
        /// 需获取unionId时，为下一步获取用户详细信息做准备
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataLoginInfo> Login(InputLoginModel model)
        {

            var xLog = new Logger();
            xLog.AddLine("=================== Member Login ========================");
            xLog.AddLine($"JsCode      :{model.JsCode}");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");
            xLog.AddLine($"{WxOpenAppId} {WxOpenAppSecret}");

            var r = new InvokeResult<DataLoginInfo>() { Data = new DataLoginInfo() };
            try
            {
                var jsonResult = SnsApi.JsCode2Json(WxOpenAppId, WxOpenAppSecret, model.JsCode);
                if (jsonResult.errcode == ReturnCode.请求成功)
                {
                    //使用SessionContainer管理登录信息（推荐）
                    var unionId = jsonResult.unionid;
                    var sessionId = Guid.NewGuid();
                    var sessionBag = SessionContainer.UpdateSession(sessionId.ToString(), jsonResult.openid, jsonResult.session_key, unionId);

                    r.Data.OpenId = jsonResult.openid;
                    r.Data.UnionId = jsonResult.unionid;
                    xLog.AddLine($"JsonResult:{Environment.NewLine}{Newtonsoft.Json.JsonConvert.SerializeObject(jsonResult, Formatting.Indented)}");

                    var xSession = new TinyAppSession();
                    if (!xSession.Add(jsonResult.openid, jsonResult.session_key, sessionId))
                    {
                        xLog.AddLine($"Session update error:{xSession.ErrorMessage}");
                    }

                    var openId = jsonResult.openid;
                    unionId = jsonResult.unionid;

                    if (string.IsNullOrEmpty(openId))
                    {
                        openId = "";
                    }

                    if (string.IsNullOrEmpty(unionId))
                    {
                        unionId = "";
                    }

                    var xUser = new WeiXinUser();

                    if (xUser.LoadByTinyAppAccount(unionId, openId, out var user))
                    {
                        if (user != null)
                        {
                            r.Data.UnionId = unionId;
                            r.Data.Avatar = user.HeadImage;
                            r.Data.NickName = user.NickName;

                            if (!string.IsNullOrEmpty(openId) && !string.IsNullOrEmpty(unionId)
                                && (user.OpenId != user.UnionId)
                                && !string.IsNullOrEmpty(user.HeadImage)
                                && !string.IsNullOrEmpty(user.NickName))
                            {
                                if (xUser.FetchRole(unionId,out _, out var parents, out var teachers))
                                {
                                    if (parents != null)
                                    {
                                        r.Data.Parents = new List<DataParentRole>();
                                        foreach (var item in parents)
                                        {
                                            var p = new DataParentRole();
                                            p.ParentId = item.ParentId;
                                            p.StudentId = item.StudentId;
                                            r.Data.Parents.Add(p);
                                        }
                                    }

                                    if (teachers != null)
                                    {
                                        r.Data.Teachers = new List<DataTeacherRole>();
                                        foreach (var item in teachers)
                                        {
                                            var t = new DataTeacherRole();
                                            t.TeacherId = item.TeacherId;
                                            t.TeamId = item.TeamId;
                                            r.Data.Teachers.Add(t);
                                        }
                                    }

                                    r.Success = true;
                                }
                                else
                                {
                                    r.ErrorMessage = xUser.ErrorMessage;
                                    xLog.AddLine("获取角色失败：" + xUser.ErrorMessage);
                                    r.Success = false;
                                }
                            }
                            else
                            {
                                r.ErrorNumber = 404;
                                r.ErrorMessage = "信息不全！";
                                r.Success = false;
                            }
                        }
                        else
                        {
                            r.Success = false;
                            r.ErrorNumber = 404;
                            r.ErrorMessage = "用户不存在！";
                        }
                    }
                    else
                    {
                        r.ErrorNumber = 404;
                        r.Success = false;
                        r.ErrorMessage = "用户不存在！";
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "请求失败：" + jsonResult.errmsg;
                }

                if (string.IsNullOrEmpty(r.Data.OpenId))
                {
                    r.Data.OpenId = "";
                }
                if (string.IsNullOrEmpty(r.Data.UnionId))
                {
                    r.Data.UnionId = "";
                }

                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }
        }
        #endregion

        #region 欢迎 Welcome
        /// <summary>
        /// 欢迎 Welcome，已有unionId，快速获取信息  
        /// 不通过微信服务器，仅在业务数据库中获取
        /// 如果传入TeamId大于0，则返回指定的TeamId(Guid)
        /// </summary>
        /// <returns></returns>
        public InvokeResult<DataDecodeEncryt> Welcome(InputBaseWithTeamIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Member Welcome ========================");
            xLog.AddLine($"UnionId      :{model.UnionId}");
            xLog.AddLine($"Sign         :{model.Sign}");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataDecodeEncryt>() { Data=new DataDecodeEncryt() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.FetchRole(model.UnionId, out var user, out var parents, out var teachers))
                {
                    r.Data.UnionId = model.UnionId;
                    r.Data.OpenId = user.TinyAppOpenId;

                    if (parents != null)
                    {
                        r.Data.Parents = new List<DataParentRole>();
                        foreach (var item in parents)
                        {
                            var p = new DataParentRole();
                            p.ParentId = item.ParentId;
                            p.StudentId = item.StudentId;
                            r.Data.Parents.Add(p);
                        }
                    }

                    if (teachers != null)
                    {
                        r.Data.Teachers = new List<DataTeacherRole>();
                        foreach (var item in teachers)
                        {
                            var t = new DataTeacherRole();
                            t.TeacherId = item.TeacherId;
                            t.TeamId = item.TeamId;
                            r.Data.Teachers.Add(t);
                        }
                    }

                    r.Success = true;
                }
                else
                {
                    r.ErrorMessage = xUser.ErrorMessage;
                    xLog.AddLine("获取角色失败：" + xUser.ErrorMessage);
                    r.Success = false;
                }
                r.Success = true;


                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }
        }
        #endregion

        #region 解密数据 DecodeEncryptedData
        /// <summary>
        /// 解密数据，通过openId等值从微信服务器获取用户详细信息，包含unionId、头像、昵称等
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataDecodeEncryt> DecodeEncryptedData(InputDecodeEncryptModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Member DecodeEncryptedData ========================");
            xLog.AddLine($"OpenId             :{model.OpenId}");
            xLog.AddLine($"EncryptedData      :{model.EncryptedData}");
            xLog.AddLine($"Iv                 :{model.Iv}");
            xLog.AddLine($"From               :{model.From}");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataDecodeEncryt>() { Data = new DataDecodeEncryt() };
            try
            {
                var xSession = new TinyAppSession();

                if (xSession.Load(model.OpenId, out Guid sessionId))
                {
                    xLog.AddLine($"SessionId:{sessionId}");
                    var decodedEntity = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeUserInfoBySessionId(
                        sessionId.ToString(),
                        model.EncryptedData, model.Iv);

                    xLog.AddLine($"DecodeData:{Environment.NewLine}{JsonConvert.SerializeObject(decodedEntity, Formatting.Indented)}");

                    //检验水印
                    if (decodedEntity != null)
                    {
                        var checkWartmark = decodedEntity.CheckWatermark(WxOpenAppId);
                        if (checkWartmark)
                        {
                            var xUser = new WeiXinUser();
                            if (xUser.TinyAppRegister(decodedEntity.unionId, decodedEntity.openId, model.From, decodedEntity.gender, "",
                                decodedEntity.country, decodedEntity.province, decodedEntity.city, decodedEntity.nickName, "", decodedEntity.avatarUrl))
                            {
                                xLog.AddLine("User Registered!");
                            }
                            else
                            {
                                xLog.AddLine($"User Register Error:{xUser.ErrorMessage}");
                            }
                            r.Data.OpenId = decodedEntity.openId;
                            r.Data.UnionId = decodedEntity.unionId;

                            if (xUser.FetchRole(decodedEntity.unionId,out  _, out var parents, out var teachers))
                            {
                                if (parents != null)
                                {
                                    r.Data.Parents = new List<DataParentRole>();
                                    foreach (var item in parents)
                                    {
                                        var p = new DataParentRole();
                                        p.ParentId = item.ParentId;
                                        p.StudentId = item.StudentId;
                                        r.Data.Parents.Add(p);
                                    }
                                }

                                if (teachers != null)
                                {
                                    r.Data.Teachers = new List<DataTeacherRole>();
                                    foreach (var item in teachers)
                                    {
                                        var t = new DataTeacherRole();
                                        t.TeacherId = item.TeacherId;
                                        t.TeamId = item.TeamId;
                                        r.Data.Teachers.Add(t);
                                    }
                                }

                                r.Success = true;
                            }
                            else
                            {
                                r.ErrorMessage = xUser.ErrorMessage;
                                xLog.AddLine("获取角色失败：" + xUser.ErrorMessage);
                                r.Success = false;
                            }

                        }
                        else
                        {
                            r.ErrorMessage = "水印验证不通过！";
                            r.Success = false;
                        }
                    }
                    else
                    {
                        r.ErrorMessage = "加密失败！";
                        r.Success = false;
                    }
                }
                else
                {
                    r.ErrorMessage = $"获取SessionKey失败：{xSession.ErrorMessage}";
                    r.Success = false;
                }
                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                if (r.ErrorMessage.Contains("SessionId无效"))
                {
                    r.ErrorNumber = (int)Base.EnumErrors.SessionId无效;
                }
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }
        }
        #endregion
    }
}
