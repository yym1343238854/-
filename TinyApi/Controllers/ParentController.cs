using Business;
using Codans.Helper.Logs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using TinyApi.Core;
using TinyApi.Models.Input;
using TinyApi.Models.Result;
using static Business.Base;

namespace TinyApi.Controllers
{
    /// <summary>
    /// 家长相关操作
    /// </summary>
    public class ParentController : ApiController
    {
        #region 获取某月的测温记录 ListMonthBbtRecords
        /// <summary>
        /// 获取某月的测温记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataStudentMonthTempRecords> ListMonthBbtRecords(InputStudentMonthTempRecordsModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent ListMonthBbtRecords ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");
            xLog.AddLine("");
            var r = new InvokeResult<DataStudentMonthTempRecords>() { Data = new DataStudentMonthTempRecords() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out Parent parent))
                {
                    var xBbt = new StudentBbtRecord();
                    if (xBbt.ListMonthByStudent(model.StudentId, model.Year, model.Month, out var list))
                    {
                        r.Data.Days = new List<DataStudentDayRecord>();

                        var maxDays = DateTime.DaysInMonth(model.Year, model.Month);
                        for (int i = 1; i <= maxDays; i++)
                        {
                            var day = new DataStudentDayRecord();
                            day.Day = i;

                            var css = "green";

                            var familyRecord = list.Where(x => x.SpecDay.Day == i && x.Type == (byte)EnumBbtType.家测).OrderByDescending(x => x.Checkintime).FirstOrDefault();

                            if (familyRecord != null)
                            {
                                var record = new DataTempRecord();
                                record.RecordId = familyRecord.StudentBbtRecordId;
                                record.Temperature = familyRecord.Temperature;
                                if (familyRecord.Temperature <= SiteConfig.MaxNormalTemperature)
                                {
                                    record.Color = "green";
                                }
                                else
                                {
                                    record.Color = "red";
                                    css = "red";
                                }
                                day.FamilyTempRecord = record;
                            }

                            var firstRecord = list.Where(x => x.SpecDay.Day == i && x.Type == (byte)EnumBbtType.晨测).OrderByDescending(x => x.Checkintime).FirstOrDefault();

                            if (firstRecord != null)
                            {
                                var record = new DataTempRecord();
                                record.RecordId = firstRecord.StudentBbtRecordId;
                                record.Temperature = firstRecord.Temperature;
                                if (firstRecord.Temperature <= SiteConfig.MaxNormalTemperature)
                                {
                                    record.Color = "green";
                                }
                                else
                                {
                                    record.Color = "red";
                                    css = "red";
                                }
                                day.FirstTempRecord = record;
                            }

                            var secordRecord = list.Where(x => x.SpecDay.Day == i && x.Type == (byte)EnumBbtType.午测).OrderByDescending(x => x.Checkintime).FirstOrDefault();

                            if (secordRecord != null)
                            {
                                var record = new DataTempRecord();
                                record.RecordId = secordRecord.StudentBbtRecordId;
                                record.Temperature = secordRecord.Temperature;
                                if (secordRecord.Temperature <= SiteConfig.MaxNormalTemperature)
                                {
                                    record.Color = "green";
                                }
                                else
                                {
                                    record.Color = "red";
                                    css = "red";
                                }
                                day.SecondTempRecord = record;
                            }

                            var records = list.Where(x => x.SpecDay.Day == i).OrderByDescending(x => x.Checkintime).ToList();
                            day.Records = new List<DataTempRecord>();
                            foreach (var item in records)
                            {
                                var record = new DataTempRecord();

                                record.RecordId = item.StudentBbtRecordId;
                                record.Temperature = item.Temperature;
                                if (item.Temperature <= SiteConfig.MaxNormalTemperature)
                                {
                                    record.Color = "green";
                                }
                                else
                                {
                                    record.Color = "red";
                                    css = "red";
                                }

                                var type = "";
                                switch (item.Type)
                                {
                                    case 1:
                                        {
                                            type = "学校";
                                            break;
                                        }
                                    case 2:
                                        {
                                            type = "学校";
                                            break;
                                        }
                                    case 3:
                                        {
                                            type = "家测";
                                            break;
                                        }
                                    case 4:
                                        {
                                            type = "学校抽测";
                                            break;
                                        }
                                }

                                record.Content = $"【{type}】{item.Temperature}摄氏度";
                                record.Hhmm = item.Checkintime.ToString("HH:mm");
                                day.Records.Add(record);
                            }
                            if (day.Records.Any())
                            {
                                day.ColorCss = css;
                            }
                            else
                            {
                                day.ColorCss = "";
                            }
                           

                            r.Data.Days.Add(day);
                        }

                        r.Success = true;
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xBbt.ErrorMessage;
                        r.ErrorNumber = xBbt.ErrorNumber;
                        xLog.AddLine("ListMonthRecords Error:" + xBbt.ErrorMessage);
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = xUser.ErrorMessage;
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine("CheckParent Error:" + xUser.ErrorMessage);
                }
                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.ToString());
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion

        #region 注册 Register（传StudentId时）
        /// <summary>
        /// 注册（传StudentId时）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult Register(InputParentRegisterWithStudentIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent Register ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");
            xLog.AddLine("");
            var r = new InvokeResult();
            try
            {
                var xParent = new Parent();
                if (xParent.Register(model.UnionId, model.StudentId, model.Name, model.Mobile, model.IdNo, model.Relation))
                {
                    r.Success = true;
                    xLog.AddLine("success.");
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = xParent.ErrorMessage;
                    r.ErrorNumber = xParent.ErrorNumber;
                    xLog.AddLine("Register Error:" + xParent.ErrorNumber);
                }
                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.ToString());
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

        #region 注册（传学生照片) UploadRegister
        /// <summary>
        /// 传学生头像注册 UploadRegister
        /// 格式：在Form的Data项中，存储下面类的实例序列化字符串  
        /// {  
        ///     UnionId:"",  
        ///     Sign:"",  
        ///     Name:"",  
        ///     Mobile:""  
        ///     IdNo:"",  
        ///     Relation:""  
        ///     Teamid:""  
        ///     StudentName:""  
        ///     StudentIdNo:""  
        ///     StudentNo:  
        /// }  
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UploadRegister()
        {
            var xLog = new Logger();
            xLog.AddLine("=============== Parent UploadRegister ===============");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine("Ip    :{0}", ip);

            #region 数据初始化
            var r = new InvokeResult<DataRegisterResult>() { Data = new DataRegisterResult() };

            var model = new InputParentRegisterModel();//资料信息
            var imageFile = new ImageFileModel();//图片信息

            string yearFolder = DateTime.Now.ToString("yyyyMM");
            string monthFolder = DateTime.Now.ToString("yyyyMMdd");

            WeiXinUser member = null;
            var xMember = new WeiXinUser();
            #endregion

            #region MultipartFormData 初始化设置
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/UserFiles/");
            Core.Helper.CreateDirectory(root);/* 创建目录 */
            var provider = new MultipartFormDataStreamProvider(root);
            #endregion

            try
            {
                // Read the form data and return an async task. 
                await Request.Content.ReadAsMultipartAsync(provider);

                #region  1、获取formdata 
                xLog.AddLine("FormData:{0}", provider.FormData.Count);
                // This illustrates how to get the form data. 
                foreach (var key in provider.FormData.AllKeys)
                {
                    var value = provider.FormData[key];
                    xLog.AddLine($"{key}:{value}\n");

                    //Data返回InputRegisterMobileModel的json字符串，首先将data字符串base64转换，然后将字符串反序列化。
                    if (key.ToLower() == "data")
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            var jsonData = value;//.Base64Decode();

                            xLog.AddLine($"JsonData:{jsonData}");

                            if (JsonSplit.IsJson(jsonData))
                            {
                                xLog.AddLine($"IsJsonData:true");
                                model = JsonConvert.DeserializeObject<InputParentRegisterModel>(jsonData);

                                if (model != null)
                                {
                                    xLog.AddLine($"Params:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

                                    if (string.IsNullOrEmpty(model.UnionId))
                                    {
                                        model.UnionId = Base.DemoUnionId;
                                    }

                                    if (xMember.CheckUnionId(model.UnionId, out var m))
                                    {
                                        member = m;
                                    }
                                }
                                else
                                {
                                    r.ErrorMessage = "参数反序列化失败！";
                                    r.ErrorNumber = (int)Base.EnumErrors.参数校验错误;
                                    return new HttpResponseMessage()
                                    {
                                        Content = new StringContent(JsonConvert.SerializeObject(r))
                                    };
                                }
                            }
                            else
                            {
                                r.ErrorMessage = "参数为空，提交失败！";
                                r.ErrorNumber = (int)Base.EnumErrors.参数校验错误;
                                return new HttpResponseMessage()
                                {
                                    Content = new StringContent(JsonConvert.SerializeObject(r))
                                };
                            }

                        }
                    }
                }

                #endregion

                #region 2、获取FileData
                xLog.AddLine("FileData:{0}", provider.FileData.Count);
                var attPhotos = new List<ImageFileModel>();
                // This illustrates how to get the file names for uploaded files. 
                foreach (var file in provider.FileData)
                {
                    var fileInfo = new FileInfo(file.LocalFileName);
                    imageFile = new ImageFileModel();
                    imageFile.FileSize = (int)fileInfo.Length;

                    xLog.AddLine($"FileSize:{imageFile.FileSize}");

                    if (string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable,
                            "This request is not properly formatted");
                    }
                    string fileName = file.Headers.ContentDisposition.FileName;/* 文件名称 */
                    imageFile.OldName = fileName;
                    xLog.AddLine($"OldName:{imageFile.OldName}");

                    string key = file.Headers.ContentDisposition.Name.Trim('"');

                    if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                    {
                        fileName = fileName.Trim('"');
                    }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                    {
                        fileName = Path.GetFileName(fileName);
                    }

                    xLog.AddLine($"FileInfo:Key{key} Uploaded file: {fileName} {fileInfo.Length} bytes");

                    string ext = Path.GetExtension(fileName);

                    if (ext == "" || ext == ".")
                        ext = ".png";

                    imageFile.FileExt = ext;

                    string sFolder = Path.Combine(root, "Students", yearFolder, monthFolder, model.TeamId.ToString());

                    //创建文件夹
                    Helper.CreateDirectory(sFolder);
                    xLog.AddLine($"Folder:{sFolder}");
                    string newName = Guid.NewGuid().ToString().Replace("-", "") + ext;//新的文件名称
                    string newFileName = Path.Combine(sFolder, newName);
                    imageFile.NewName = newFileName;

                    Core.Helper.FileMove(sFolder, file.LocalFileName, newFileName);
                    xLog.AddLine("File:{0}", newFileName);
                    imageFile.Url =
                        $"/Userfiles/Students/{yearFolder}/{monthFolder}/{model.TeamId.ToString()}/{newName}?r={Core.Helper.RandomString(8)}";


                    string newSmallFileName = $"s_{newName}";
                    string newSmallFileFullName = Path.Combine(sFolder, newSmallFileName);

                    FileInfo filePhoto = new FileInfo(newFileName);
                    var sizeInBytes = filePhoto.Length;

                    var xImage = Image.FromStream(new MemoryStream(File.ReadAllBytes(newFileName)));
                    var imageWidth = xImage.Width;
                    var imageHeight = xImage.Height;
                    xImage.Dispose();

                    if (imageWidth > 2000 || imageHeight > 2000 || sizeInBytes > 2 * 1024 * 1024)
                    {
                        string errorMsg = "";
                        //创建缩略图
                        if (Core.Helper.GenerateThumbnail(newFileName, newSmallFileFullName, "", 1280, out errorMsg, out _, out _))
                        {
                            xLog.AddLine("创建缩略图成功。" + newFileName);
                            imageFile.Url =
                        $"/Userfiles/Query/{yearFolder}/{monthFolder}/{member.UnionId}/s_{newName}?r={Core.Helper.RandomString(8)}";
                        }
                        else
                        {
                            xLog.AddLine("创建缩略图失败：" + errorMsg);
                        }


                    }


                    attPhotos.Add(imageFile);
                }
                #endregion

                #region 3、完善信息
                var fullUrl = imageFile.Url.FixImagePath();
                xLog.AddLine($"Url = {fullUrl}");

                var xParent = new Parent();
                if (xParent.Register(model.UnionId, model.Name, model.Mobile, model.IdNo, model.Relation,
                    model.TeamId, model.StudentName, model.StudentIdNo, imageFile.Url, model.StudentNo, out var parentId, out var studentId))
                {
                    xLog.AddLine($"Save Success.");
                    r.Data.ParentId = parentId;
                    r.Data.StudentId = studentId;
                    r.Data.TeamId = model.TeamId;
                    r.Success = true;
                }
                else
                {
                    xLog.AddLine("Save Error:{0}", xParent.ErrorMessage);
                    r.ErrorMessage = xParent.ErrorMessage;
                    r.ErrorNumber = xParent.ErrorNumber;
                    r.Success = false;
                }

                #endregion
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(r);
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json)
                };
            }
            catch (System.Exception e)
            {
                xLog.AddLine("Exception:" + e.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion

        #region 家长记录学生体温 RecordStudentBbt
        /// <summary>
        /// 家长记录学生体温 RecordStudentBbt
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult RecordStudentBbt(InputUploadBbt model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent RecordStudentBbt ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");
            xLog.AddLine("");
            var r = new InvokeResult();
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out Parent parent))
                {
                    var xBbt = new StudentBbtRecord();
                    if (xBbt.AddByParent(parent.ParentId, model.StudentId, DateTime.Today, model.Temperature, ""))
                    {
                        xLog.AddLine("Success.");
                        r.Success = true;
                    }
                    else
                    {
                        xLog.AddLine($"AddByParent Error:{xBbt.ErrorMessage}");
                        r.ErrorMessage = xBbt.ErrorMessage;
                        r.ErrorNumber = xBbt.ErrorNumber;
                        r.Success = false;
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = xUser.ErrorMessage;
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine("CheckUnionId Error:" + xUser.ErrorNumber);
                }
                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.ToString());
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion

        #region 首页
        /// <summary>
        /// 家长首页
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataParentHome> Home(InputStudentIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent Home ========================");
            xLog.AddLine($"UnionId      :{model.UnionId}");
            xLog.AddLine($"Sign         :{model.Sign}");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataParentHome>() { Data = new DataParentHome(), Success = true };
            try
            {
                var xUser = new WeiXinUser();
                var xStudent = new Student();
                if (xUser.CheckParent(model.UnionId, out var parent))
                {
                    if (parent == null)
                    {
                        xLog.AddLine($"CheckParent Error:家长不存在");
                        r.Success = false;
                        r.ErrorMessage = "家长不存在";
                    }

                    if (xStudent.Load(model.StudentId, out var student))
                    { // 加载学生信息
                        if (student != null)
                        {
                            Team team = student.TeamStudent.FirstOrDefault(x => x.Status == 1).Team;
                            r.Data.TodayString = DateTime.Today.ToString("yyyy年MM月dd日 dddd");
                            r.Data.HasParentCheckin = parent.ParentCheckin.Any(x => x.SpecDay == DateTime.Today && x.Status != 0);
                            var holiday = student.Team.School.Holiday.Where(x => x.SpecDay == DateTime.Now.Date && x.Status != 0).FirstOrDefault();
                            r.Data.HolidayStatus = holiday == null ? (byte)0 : holiday.Type;
                            r.Data.NeedFamilyTest = team.School.NeedFamilyTest;
                            r.Data.UploadBbtPhotoType = team.School.NeedTemperaturePhoto;
                            r.Data.Team = new DataTeamInfo();
                            r.Data.Team.SchoolName = team.School.Name;
                            r.Data.Team.TeamName = team.Name;
                            var teamRoom = team.TeamClassroom.SingleOrDefault(x => x.Status == 1);
                            r.Data.Team.RoomName = teamRoom != null ? teamRoom.Room.Name : "";
                            r.Data.Team.StudentNum = team.StudentNum;
                            r.Data.Team.ParentNum = team.ParentNum;
                            r.Data.Team.TeacherNum = team.TeacherNum;
                            r.Data.Team.TeamId = team.TeamId;
                            r.Data.Team.Student = new DataStudentInfo();
                            r.Data.Team.Student.AvatarUrl = student.AvatarUrl.FixImagePath();
                            r.Data.Team.Student.Name = student.Name;
                            r.Data.Team.Student.IdNo = student.IdNo;
                            r.Data.Team.Student.No = student.No;
                            r.Data.Team.Student.StudentId = student.StudentId;

                            #region 家长打卡情况
                            r.Data.FamilyMembers = new List<FamilyMemberCheckin>();
                            foreach (var item in student.StudentParent.Where(x => x.Status == 1))
                            {
                                var member = new FamilyMemberCheckin();
                                member.ParentId = item.ParentId;
                                member.AvatarUrl = item.Parent.WeiXinUser.HeadImage;
                                member.HasCheckin = item.Parent.ParentCheckin.Any(x => x.ParentId == item.ParentId && x.SpecDay == DateTime.Today && x.Status == (byte)EnumDataStatus.Normal);

                                var pc = item.Parent.ParentCheckin.Where(x => x.ParentId == item.ParentId && x.SpecDay == DateTime.Today && x.Status == (byte)EnumDataStatus.Normal).OrderByDescending(x => x.Checkintime).FirstOrDefault();

                                if (pc != null)
                                {
                                    member.ColorStatus = pc.ColorStatus;

                                    switch (pc.ColorStatus)
                                    {
                                        case 1:
                                            {
                                                member.CssClass = "green";
                                                break;
                                            }
                                        case 2:
                                            {
                                                member.CssClass = "orange";
                                                break;
                                            }
                                        case 3:
                                            {
                                                member.CssClass = "red";
                                                break;
                                            }
                                    }
                                }

                                r.Data.FamilyMembers.Add(member);
                            }
                            #endregion

                            #region 体温记录、家长是否测温
                            // 日测温列表

                            var familyTestRecord = student.StudentBbtRecord.FirstOrDefault(x => x.Status == 1 && x.Type == (byte)EnumBbtType.家测 && x.SpecDay == DateTime.Today);
                            if (familyTestRecord != null)
                            {
                                r.Data.FamilyBbt = new BbtRecord()
                                {
                                    MeasureTimeStr = familyTestRecord.Checkintime.ToString("HH:mm"),
                                    Temperature = familyTestRecord.Temperature,
                                    Status = familyTestRecord.Temperature >= SiteConfig.MaxNormalTemperature ? true : false
                                };
                            }
                            r.Data.HasFamilyMeasureBbt = familyTestRecord != null ? true : false; //家长是否测温


                            // 晨测 type:1
                            var morningTestRecord = student.StudentBbtRecord.FirstOrDefault(x => x.Status == 1 && x.Type == (byte)EnumBbtType.晨测 && x.SpecDay == DateTime.Today);

                            if (morningTestRecord != null)
                            {
                                r.Data.FirstSchoolBbt = new BbtRecord()
                                {
                                    MeasureTimeStr = morningTestRecord.Checkintime.ToString("HH:mm"),
                                    Temperature = morningTestRecord.Temperature,
                                    Status = morningTestRecord.Temperature >= SiteConfig.MaxNormalTemperature ? true : false
                                };
                            }
                            // 午测 type:2
                            var noonTestRecord = student.StudentBbtRecord.FirstOrDefault(x => x.Status == 1 && x.Type == (byte)EnumBbtType.午测 && x.SpecDay == DateTime.Today);
                            if (noonTestRecord != null)
                            {
                                r.Data.SecondSchoolBbt = new BbtRecord()
                                {
                                    MeasureTimeStr = noonTestRecord.Checkintime.ToString("HH:mm"),
                                    Temperature = noonTestRecord.Temperature,
                                    Status = noonTestRecord.Temperature >= SiteConfig.MaxNormalTemperature ? true : false
                                };
                            }
                            #endregion

                            #region 最新动态列表
                            r.Data.Tips = new List<Tip>();

                            var xTips = new Tips();
                            if (xTips.ListByParent(parent.ParentId, model.StudentId, DateTime.Today, out var list))
                            {
                                if (list != null)
                                {
                                    foreach (var t in list)
                                    {
                                        var tip = new Tip()
                                        {
                                            TipsId = t.TipsId,
                                            TimeStr = t.Checkintime.ToString("HH:mm"),
                                            Content = t.Content,
                                            FontColor = t.FontColor
                                        };

                                        r.Data.Tips.Add(tip);
                                    }

                                }
                            }
                            #endregion

                            #region 上下学时间安排

                            var checkinTime = team.TeamSchedule.SingleOrDefault(x => x.Schedule.Type == (byte)EnumScheduleType.上学 && x.Status == (byte)EnumDataStatus.Normal);

                            if (checkinTime != null)
                            {
                                r.Data.CheckinSchedule = new DaySchedule
                                {
                                    Type = 1,
                                    StartTimeStr = checkinTime.Schedule.StartHhmm,
                                    EndTimeStr = checkinTime.Schedule.EndHhmm
                                };
                            }

                            var checkoutTime = team.TeamSchedule.SingleOrDefault(x => x.Schedule.Type == (byte)EnumScheduleType.放学 && x.Status == (byte)EnumDataStatus.Normal);

                            if (checkoutTime != null)
                            {
                                r.Data.CheckoutSchedule = new DaySchedule
                                {
                                    Type = 1,
                                    StartTimeStr = checkoutTime.Schedule.StartHhmm,
                                    EndTimeStr = checkoutTime.Schedule.EndHhmm
                                };
                            }
                            #endregion

                            if (!r.Data.HasFamilyMeasureBbt)
                            {
                                r.Data.TodoTasks = 1;
                            }

                            if (!r.Data.HasParentCheckin)
                            {
                                r.Data.TodoTasks++;
                            }
                        }
                        else
                        {
                            xLog.AddLine($"Load Error:{xStudent.ErrorMessage}");
                            r.Success = false;
                            r.ErrorMessage = xUser.ErrorMessage;
                        }
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckUnion Error:{r.ErrorMessage}");
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
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion

        #region 上传体温
        /// <summary>
        /// 上传体温 UploadBbt
        /// 格式：在Form的Data项中，存储下面类的实例序列化字符串  
        /// {  
        ///     UnionId:"",  
        ///     Sign:"",  
        ///     Name:"",  
        ///     StudentId:"",  
        ///     Temperature:"",  
        ///     Type:""  
        /// }  
        /// Type：测温类型(1.晨测 2.午测 3.家测 4.临时检查)  
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UploadStudentBbt()
        {
            var xLog = new Logger();
            xLog.AddLine("=============== Parent UploadBbt ===============");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine("Ip    :{0}", ip);

            #region 数据初始化
            var r = new InvokeResult();

            var model = new InputUploadBbt();//表单信息
            var imageFile = new ImageFileModel();//图片信息

            string yearFolder = DateTime.Now.ToString("yyyyMM");
            string monthFolder = DateTime.Now.ToString("yyyyMMdd");

            WeiXinUser member = null;
            var xMember = new WeiXinUser();
            #endregion

            #region MultipartFormData 初始化设置
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/UserFiles/");
            Core.Helper.CreateDirectory(root);/* 创建目录 */
            var provider = new MultipartFormDataStreamProvider(root);
            #endregion

            try
            {
                // Read the form data and return an async task. 
                await Request.Content.ReadAsMultipartAsync(provider);

                #region  1、获取formdata 
                xLog.AddLine("FormData:{0}", provider.FormData.Count);
                // This illustrates how to get the form data. 
                foreach (var key in provider.FormData.AllKeys)
                {
                    var value = provider.FormData[key];
                    xLog.AddLine($"{key}:{value}\n");

                    //Data返回InputRegisterMobileModel的json字符串，首先将data字符串base64转换，然后将字符串反序列化。
                    if (key.ToLower() == "data")
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            var jsonData = value;//.Base64Decode();

                            xLog.AddLine($"JsonData:{jsonData}");

                            if (JsonSplit.IsJson(jsonData))
                            {
                                xLog.AddLine($"IsJsonData:true");
                                model = JsonConvert.DeserializeObject<InputUploadBbt>(jsonData);

                                if (model != null)
                                {
                                    xLog.AddLine($"Params:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

                                    if (string.IsNullOrEmpty(model.UnionId)) { model.UnionId = Base.DemoUnionId; }

                                    if (xMember.CheckUnionId(model.UnionId, out var m)) { member = m; }
                                }
                                else
                                {
                                    r.ErrorMessage = "参数反序列化失败！";
                                    r.ErrorNumber = (int)Base.EnumErrors.参数校验错误;
                                    return new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(r)) };
                                }
                            }
                            else
                            {
                                r.ErrorMessage = "参数为空，提交失败！";
                                r.ErrorNumber = (int)Base.EnumErrors.参数校验错误;
                                return new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(r)) };
                            }

                        }
                    }
                }
                #endregion

                #region 2、获取FileData
                xLog.AddLine("FileData:{0}", provider.FileData.Count);
                var attPhotos = new List<ImageFileModel>();
                // This illustrates how to get the file names for uploaded files. 
                foreach (var file in provider.FileData)
                {
                    var fileInfo = new FileInfo(file.LocalFileName);
                    imageFile = new ImageFileModel();
                    imageFile.FileSize = (int)fileInfo.Length;

                    xLog.AddLine($"FileSize:{imageFile.FileSize}");

                    if (string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.NotAcceptable,
                            "This request is not properly formatted");
                    }
                    string fileName = file.Headers.ContentDisposition.FileName;/* 文件名称 */
                    imageFile.OldName = fileName;
                    xLog.AddLine($"OldName:{imageFile.OldName}");

                    string key = file.Headers.ContentDisposition.Name.Trim('"');

                    if (fileName.StartsWith("\"") && fileName.EndsWith("\"")) { fileName = fileName.Trim('"'); }
                    if (fileName.Contains(@"/") || fileName.Contains(@"\")) { fileName = Path.GetFileName(fileName); }

                    xLog.AddLine($"FileInfo:Key{key} Uploaded file: {fileName} {fileInfo.Length} bytes");

                    string ext = Path.GetExtension(fileName);

                    if (ext == "" || ext == ".") ext = ".png";

                    imageFile.FileExt = ext;

                    string sFolder = Path.Combine(root, "StudentBbt", yearFolder, monthFolder, model.StudentId.ToString());

                    //创建文件夹
                    Helper.CreateDirectory(sFolder);
                    xLog.AddLine($"Folder:{sFolder}");
                    string newName = Guid.NewGuid().ToString().Replace("-", "") + ext;//新的文件名称
                    string newFileName = Path.Combine(sFolder, newName);
                    imageFile.NewName = newFileName;

                    Core.Helper.FileMove(sFolder, file.LocalFileName, newFileName);
                    xLog.AddLine("File:{0}", newFileName);
                    imageFile.Url = $"/Userfiles/StudentBbt/{yearFolder}/{monthFolder}/{model.StudentId.ToString()}/{newName}?r={Core.Helper.RandomString(8)}";


                    string newSmallFileName = $"s_{newName}";
                    string newSmallFileFullName = Path.Combine(sFolder, newSmallFileName);

                    FileInfo filePhoto = new FileInfo(newFileName);
                    var sizeInBytes = filePhoto.Length;

                    var xImage = Image.FromStream(new MemoryStream(File.ReadAllBytes(newFileName)));
                    var imageWidth = xImage.Width;
                    var imageHeight = xImage.Height;
                    xImage.Dispose();

                    if (imageWidth > 2000 || imageHeight > 2000 || sizeInBytes > 2 * 1024 * 1024)
                    {
                        string errorMsg = "";
                        //创建缩略图
                        if (Core.Helper.GenerateThumbnail(newFileName, newSmallFileFullName, "", 1280, out errorMsg, out _, out _))
                        {
                            xLog.AddLine("创建缩略图成功。" + newFileName);
                            imageFile.Url = $"/Userfiles/Query/{yearFolder}/{monthFolder}/{member.UnionId}/s_{newName}?r={Core.Helper.RandomString(8)}";
                        }
                        else
                        {
                            xLog.AddLine("创建缩略图失败：" + errorMsg);
                        }
                    }

                    attPhotos.Add(imageFile);
                }
                #endregion

                #region 3、完善信息
                var fullUrl = imageFile.Url.FixImagePath();
                xLog.AddLine($"Url = {fullUrl}");

                var xStudentBbtRecord = new StudentBbtRecord();
                var parent = member.Parent.FirstOrDefault(x => x.Status == 1);
                if (parent != null)
                {
                    if (xStudentBbtRecord.AddByParent(parent.ParentId, model.StudentId, DateTime.Today, model.Temperature, imageFile.Url))
                    {
                        xLog.AddLine($"Save Success.");
                        r.Success = true;
                    }
                    else
                    {
                        xLog.AddLine("Save Error:{0}", xStudentBbtRecord.ErrorMessage);
                        r.ErrorMessage = xStudentBbtRecord.ErrorMessage;
                        r.ErrorNumber = xStudentBbtRecord.ErrorNumber;
                        r.Success = false;
                    }
                }
                else
                {
                    xLog.AddLine("Save Error:家长不存在");
                    r.ErrorMessage = "家长不存在";
                    r.Success = false;
                }
                #endregion
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(r);
                return new HttpResponseMessage() { Content = new StringContent(json) };
            }
            catch (System.Exception e)
            {
                xLog.AddLine("Exception:" + e.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion

        #region 打卡 Checkin
        /// <summary>
        /// 家长打卡
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult Checkin(InputParentCheckinModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent Checkin ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult();
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out var parent))
                {
                    var xParent = new Parent();
                    if (xParent.Checkin(parent.ParentId, (EnumColorStatus)model.ColorStatus))
                    {
                        r.Success = true;
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xParent.ErrorMessage;
                        r.ErrorNumber = xParent.ErrorNumber;
                        xLog.AddLine("Parent Checkin Error:" + xParent.ErrorNumber);
                    }
                }
                else
                {
                    xLog.AddLine("CheckParent Error:家长不存在");
                    r.ErrorMessage = "家长不存在";
                    r.Success = false;
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

        #region 家长月打卡情况列表 ListMonthCheckinRecords
        /// <summary>
        /// 家长月打卡情况列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataParentMonthCheckinRecords> ListMonthCheckinRecords(InputStudentMonthTempRecordsModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent ListMonthCheckinRecords ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");
            xLog.AddLine("");
            var r = new InvokeResult<DataParentMonthCheckinRecords>() { Data = new DataParentMonthCheckinRecords() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out Parent parent))
                {
                    var xCheckin = new ParentCheckin();
                    if (xCheckin.ListMonthRecords(parent.ParentId, model.Year, model.Month, out var list))
                    {
                        r.Data.Days = new List<DataParentDayCheckinRecord>();

                        var maxDays = DateTime.DaysInMonth(model.Year, model.Month);

                        var count = 0;

                        for (int i = 1; i <= maxDays; i++)
                        {
                            var day = new DataParentDayCheckinRecord();
                            day.Day = i;

                            

                            var dayRecord = list.Where(x => x.SpecDay.Day == i).OrderByDescending(x => x.Checkintime).FirstOrDefault();
                            if (dayRecord != null)
                            {
                                count++;
                                day.Hhmm = dayRecord.Checkintime.ToString("HH:mm");
                                day.ColorStatus = dayRecord.ColorStatus;

                                if (!string.IsNullOrEmpty(dayRecord.PhotoUrl))
                                {
                                    day.PhotoUrl = dayRecord.PhotoUrl.FixImagePath();
                                }

                                switch (dayRecord.ColorStatus)
                                {
                                    case (byte)EnumColorStatus.绿码:
                                        {
                                            day.ColorCss = "green";
                                            break;
                                        }
                                    case (byte)EnumColorStatus.橙码:
                                        {
                                            day.ColorCss = "orange";
                                            break;
                                        }
                                    case (byte)EnumColorStatus.红码:
                                        {
                                            day.ColorCss = "red";
                                            break;
                                        }
                                    default:
                                        {
                                            day.ColorCss = "";
                                            break;
                                        }
                                }
                            }
                            else
                            {
                                day.ColorStatus = 0;
                                day.ColorCss = "";
                                day.PhotoUrl = "";
                            }
                            r.Data.Days.Add(day);
                        }

                        r.Data.CheckinDays = count;


                        r.Success = true;
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xCheckin.ErrorMessage;
                        r.ErrorNumber = xCheckin.ErrorNumber;
                        xLog.AddLine("ListMonthRecords Error:" + xCheckin.ErrorMessage);
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = xUser.ErrorMessage;
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine("CheckParent Error:" + xUser.ErrorMessage);
                }
                return r;
            }
            catch (Exception ex)
            {
                xLog.AddLine("Exception:\n" + ex.ToString());
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            }
            finally
            {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion
        #endregion

        #region 体温日历
        /// <summary>
        /// 体温日历
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataBbtCalendar> ListBbtCalendar(InputBbtRecords model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent ListBbtCalendar ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataBbtCalendar>() { Data = new DataBbtCalendar() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckUnionId(model.UnionId, out _))
                {
                    var xStudentBbtRecord = new StudentBbtRecord();
                    if (xStudentBbtRecord.ListByStudent(model.StudentId, model.SpecDay, out var bbtRecords))
                    {
                        if (bbtRecords != null)
                        {
                            // 按日期分组
                            r.Data.SpecDays = new List<SpecDay>();
                            foreach (var item in bbtRecords.GroupBy(x => x.SpecDay))
                            {
                                r.Data.SpecDays.Add(new SpecDay()
                                {
                                    DateStr = item.Key.ToString("yyyy-MM-dd"),
                                    MeasureNum = item.Count(),
                                    Status = item.Any(x => x.Temperature <= SiteConfig.MaxNormalTemperature) ? true : false
                                });
                            }
                            r.Success = true;
                            xLog.AddLine($"Success.");
                        }
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xStudentBbtRecord.ErrorMessage;
                        xLog.AddLine($"List StudentBbtRecords Error:{r.ErrorMessage}");
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckUnion Error:{r.ErrorMessage}");
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

        #region 日体温记录列表
        /// <summary>
        /// 体温列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataBbtRecords> ListBbtRecords(InputBbtRecords model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent ListBbtRecords ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataBbtRecords>() { Data = new DataBbtRecords() };
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckUnionId(model.UnionId, out _))
                {
                    var xStudentBbtRecord = new StudentBbtRecord();
                    if (xStudentBbtRecord.ListByStudent(model.StudentId, model.SpecDay, out var bbtRecords))
                    {
                        if (bbtRecords != null)
                        {
                            r.Data.BbtRecords = new List<BbtRecordDetail>();
                            foreach (var item in bbtRecords)
                            {
                                r.Data.BbtRecords.Add(new BbtRecordDetail()
                                {
                                    Type = item.Type,
                                    MeasureTimeStr = item.Checkintime.ToString("HH:ss"),
                                    Temperature = item.Temperature,
                                    Status = item.Temperature <= SiteConfig.MaxNormalTemperature ? true : false
                                });
                            }
                            r.Success = true;
                            xLog.AddLine($"Success.");
                        }
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xStudentBbtRecord.ErrorMessage;
                        xLog.AddLine($"List StudentBbtRecords Error:{r.ErrorMessage}");
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckUnion Error:{r.ErrorMessage}");
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

        #region 校园动态列表 ListTips
        /// <summary>
        /// 校园动态列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataSchoolTips> ListTips(InputTeamIdModel model) {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent ListTips ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");
            xLog.AddLine("");
            var r = new InvokeResult<DataSchoolTips>() { Data = new DataSchoolTips() };
            r.Data.RoomTypes = new List<RoomTab>();
            try {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out Parent parent)) {
                    var xTips = new Tips();
                    if (xTips.ListRoomTips(parent.ParentId, model.TeamId, DateTime.Today, out var list)) {
                        r.Data.TabCount = 3;
                        #region 教室
                        var rt1 = new RoomTab() { Rooms = new List<RoomTips>(), TabId = 1, TabName = "教室" };
                        var teamRoom = list.Where(x => x.Room.Type == (byte)EnumRoomType.教室).GroupBy(x => (x.Room.RoomId, x.Room.Name));
                        foreach (var item in teamRoom) {
                            var tips = new List<Tip>();
                            foreach (var tipItem in item.ToArray()) {
                                tips.Add(new Tip { TimeStr = tipItem.ActionTime.ToString("HH:mm"), Content = tipItem.Content, FontColor = tipItem.FontColor, TipsId = tipItem.TipsId });
                            }
                            rt1.Rooms.Add(new RoomTips() { Name = item.Key.Name, RoomId = item.Key.RoomId, Tips = tips });
                        }
                        r.Data.RoomTypes.Add(rt1);
                        #endregion

                        #region 食堂
                        var rt2 = new RoomTab() { Rooms = new List<RoomTips>(), TabId = 2, TabName = "食堂" };
                        var diningRoom = list.Where(x => x.Room.Type == (byte)EnumRoomType.食堂).GroupBy(x => (x.Room.RoomId, x.Room.Name));
                        foreach (var item in diningRoom) {
                            var tips = new List<Tip>();
                            foreach (var tipItem in item.ToArray()) {
                                tips.Add(new Tip { TimeStr = tipItem.ActionTime.ToString("HH:mm"), Content = tipItem.Content, FontColor = tipItem.FontColor, TipsId = tipItem.TipsId });
                            }
                            rt2.Rooms.Add(new RoomTips() { Name = item.Key.Name, RoomId = item.Key.RoomId, Tips = tips });
                        }
                        r.Data.RoomTypes.Add(rt2);
                        #endregion

                        #region 其他场所
                        var rt3 = new RoomTab() { Rooms = new List<RoomTips>(), TabId = 3, TabName = "其他场所" };
                        var otherRoom = list.Where(x => x.Room.Type != (byte)EnumRoomType.教室 && x.Room.Type != (byte)EnumRoomType.食堂).GroupBy(x => (x.Room.RoomId, x.Room.Name));
                        foreach (var item in otherRoom) {
                            var tips = new List<Tip>();
                            foreach (var tipItem in item.ToArray()) {
                                tips.Add(new Tip { TimeStr = tipItem.ActionTime.ToString("HH:mm"), Content = tipItem.Content, FontColor = tipItem.FontColor, TipsId = tipItem.TipsId });
                            }
                            rt3.Rooms.Add(new RoomTips() { Name = item.Key.Name, RoomId = item.Key.RoomId, Tips = tips });
                        }
                        r.Data.RoomTypes.Add(rt3);
                        #endregion

                        r.Success = true;
                    } else {
                        r.Success = false;
                        r.ErrorMessage = xTips.ErrorMessage;
                        r.ErrorNumber = xTips.ErrorNumber;
                        xLog.AddLine("ListMonthRecords Error:" + xTips.ErrorMessage);
                    }
                } else {
                    r.Success = false;
                    r.ErrorMessage = xUser.ErrorMessage;
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine("CheckParent Error:" + xUser.ErrorMessage);
                }
                return r;
            } catch (Exception ex) {
                xLog.AddLine("Exception:\n" + ex.ToString());
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            } finally {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion

        #region 疫情咨询 EpidemicPreventionInfo
        /// <summary>
        /// 疫情咨询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataEpidemicPreventionInfo> EpidemicPreventionInfo(InputTeamIdModel model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent EpidemicPreventionInfo ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataEpidemicPreventionInfo>() { Data = new DataEpidemicPreventionInfo() };
            r.Data.News = new List<DataNews>();
            r.Data.Documents = new List<DataDocument>();
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out Parent parent))
                {
                    var xTeam = new Team();
                    if (xTeam.Load(model.TeamId, out Team team))
                    {
                        r.Data.StudentNum = team.School.StudentNum;
                        r.Data.TeacherNum = team.School.TeacherNum;
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xTeam.ErrorMessage;
                        xLog.AddLine($"List Team Error:{r.ErrorMessage}");
                        return r;
                    }

                    #region 通知列表
                    var xNews = new News();
                    if (xNews.ListByParent(model.TeamId, 1, int.MaxValue, out var news))
                    {
                        foreach (var item in news)
                        {
                            r.Data.News.Add(new DataNews()
                            {
                                NewsId = item.NewsId,
                                PublishTimeStr = item.PublishTime.Value.ToString("yyyy年MM月dd日"),
                                Title = item.Title,
                                Hits = item.Hits ?? 0
                            });
                        }
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xNews.ErrorMessage;
                        xLog.AddLine($"List News Error:{r.ErrorMessage}");
                        return r;
                    }
                    #endregion

                    #region 防疫指南列表
                    var xDocument = new Document();
                    if (xDocument.List(1, int.MaxValue, out var documents))
                    {
                        foreach (var item in documents)
                        {
                            r.Data.Documents.Add(new DataDocument()
                            {
                                DocumentId = item.DocumentId,
                                PublishTimeStr = item.PublishTime.Value.ToString("yyyy年MM月dd日"),
                                Title = item.Title,
                                Hits = item.Views ?? 0,
                                DocumentUrl = item.Url
                            });
                        }
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xNews.ErrorMessage;
                        xLog.AddLine($"List Document Error:{r.ErrorMessage}");
                        return r;
                    }
                    #endregion

                    r.Success = true;
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = xUser.ErrorMessage;
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine("CheckParent Error:" + xUser.ErrorNumber);
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

        #region 通知详情 NewsDetail
        /// <summary>
        /// 疫情咨询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataNewsDetail> NewsDetail(InputNewsDetail model)
        {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent NewsDetail ========================");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataNewsDetail>() { Data = new DataNewsDetail() };
            r.Data.AttachMents = new List<DataNewsAttachMent>();
            try
            {
                var xUser = new WeiXinUser();
                if (xUser.CheckParent(model.UnionId, out Parent parent))
                {
                    var xNews = new News();
                    if (xNews.Load(model.NewsId, out News news))
                    {
                        r.Data.Content = news.Content;
                        r.Data.Hits = news.Hits ?? 0;
                        r.Data.Title = news.Title;
                        r.Data.PublishTimeStr = news.PublishTime.Value.ToString("yyyy年MM月dd日 HH:mm");
                        r.Data.NewsId = news.NewsId;
                        foreach (var item in news.NewsAttachment)
                        {
                            r.Data.AttachMents.Add(new DataNewsAttachMent()
                            {
                                AttachMentId = item.NewsAttachmentId,
                                Url = item.Url,
                                FileName = item.OriginalName,
                                FileSize = item.FileSize
                            });
                        }
                        r.Success = true;
                    }
                    else
                    {
                        r.Success = false;
                        r.ErrorMessage = xNews.ErrorMessage;
                        xLog.AddLine($"Load News Error:{r.ErrorMessage}");
                    }
                }
                else
                {
                    r.Success = false;
                    r.ErrorMessage = xUser.ErrorMessage;
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine("CheckParent Error:" + xUser.ErrorNumber);
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


        #region 个人中心 PersonalInfo
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        public InvokeResult<DataPersonalInfo> PersonalInfo(InputBaseModel model) {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent PersonalInfo ========================");
            xLog.AddLine($"UnionId      :{model.UnionId}");
            xLog.AddLine($"Sign         :{model.Sign}");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");

            var r = new InvokeResult<DataPersonalInfo>() { Data = new DataPersonalInfo() };
            try {
                var xUser = new WeiXinUser();
                if (xUser.FetchRole(model.UnionId, out var user, out var parents, out var teachers)) {
                    if (parents == null || parents.Count <= 0) {
                        r.ErrorMessage = "家长不存在";
                        xLog.AddLine("家长不存在");
                        r.Success = false;
                    } else {
                        r.Data.Name = parents.First().Parent.Name;
                        r.Data.Mobile = parents.First().Parent.Mobile;
                        r.Data.HeadImage = user.HeadImage;
                        if (parents != null) {
                            r.Data.Parents = new List<DataParentIdentity>();
                            foreach (var item in parents) {
                                var p = new DataParentIdentity();
                                p.ParentId = item.ParentId;
                                p.StudentId = item.StudentId;
                                p.StudentName = item.Student.Name;
                                p.AvatarUrl = item.Student.AvatarUrl.FixImagePath();
                                r.Data.Parents.Add(p);
                            }
                        }

                        if (teachers != null) {
                            r.Data.Teachers = new List<DataTeacherIdentity>();
                            foreach (var item in teachers) {
                                var t = new DataTeacherIdentity();
                                t.TeacherId = item.TeacherId;
                                t.TeamId = item.TeamId;
                                t.TeamName = item.Team.Name;
                                t.AvatarUrl = item.Teacher.AvatarUrl.FixImagePath();
                                r.Data.Teachers.Add(t);
                            }
                        }

                        r.Success = true;
                    }
                } else {
                    r.ErrorMessage = xUser.ErrorMessage;
                    xLog.AddLine("获取角色失败：" + xUser.ErrorMessage);
                    r.Success = false;
                }
                r.Success = true;

                return r;
            } catch (Exception ex) {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            } finally {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r)}");
                xLog.Save();
            }
        }
        #endregion

        #region 成员管理 MemberManagement
        /// <summary>
        /// 成员管理
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public InvokeResult<DataFamilyMember> MemberManagement(InputStudentIdModel model) {
            var xLog = new Logger();
            xLog.AddLine("=================== Parent MemberManagement ========================");
            xLog.AddLine($"UnionId      :{model.UnionId}");
            xLog.AddLine($"Sign         :{model.Sign}");
            string ip = HttpContext.Current.Request.UserHostAddress;
            xLog.AddLine($"Ip          :{ip}");
            xLog.AddLine("---------------------------------------------------------------");
            xLog.AddLine($"Input Data:{Environment.NewLine}{JsonConvert.SerializeObject(model, Formatting.Indented)}");

            var r = new InvokeResult<DataFamilyMember>() { Data = new DataFamilyMember()};
            try {
                var xUser = new WeiXinUser();
                var xStudent = new Student();
                if (xUser.CheckParent(model.UnionId, out var parent)) {
                    if (xStudent.Load(model.StudentId, out var student)) { // 加载学生信息
                        if (student != null) {
                            //班级学生信息
                            Team team = student.TeamStudent.FirstOrDefault(x => x.Status == 1).Team;
                            r.Data.TodayString = DateTime.Today.ToString("yyyy年MM月dd日 dddd");
                            r.Data.Team = new DataTeamInfo();
                            r.Data.Team.SchoolName = team.School.Name;
                            r.Data.Team.TeamName = team.Name;
                            var teamRoom = team.TeamClassroom.SingleOrDefault(x => x.Status == 1);
                            r.Data.Team.RoomName = teamRoom != null ? teamRoom.Room.Name : "";
                            r.Data.Team.StudentNum = team.StudentNum;
                            r.Data.Team.ParentNum = team.ParentNum;
                            r.Data.Team.TeacherNum = team.TeacherNum;
                            r.Data.Team.TeamId = team.TeamId;
                            r.Data.Team.Student = new DataStudentInfo();
                            r.Data.Team.Student.AvatarUrl = student.AvatarUrl.FixImagePath();
                            r.Data.Team.Student.Name = student.Name;
                            r.Data.Team.Student.IdNo = student.IdNo;
                            r.Data.Team.Student.No = student.No;
                            r.Data.Team.Student.StudentId = student.StudentId;
                            //家庭成员列表
                            r.Data.FamilyMembers = new List<FamilyMember>();
                            foreach (var item in student.StudentParent) {
                                if (item.Status == (byte)EnumDataStatus.Normal) {
                                    r.Data.FamilyMembers.Add(new FamilyMember() {
                                        ParentId = item.ParentId,
                                        HasBindWeiXin = !string.IsNullOrEmpty(item.Parent.UnionId),
                                        HeadImage = item.Parent.WeiXinUser.HeadImage,
                                        Mobile = item.Parent.Mobile,
                                        ParentName = item.Parent.Name
                                    });
                                }
                            }

                            r.Success = true;
                        }
                    } else {
                        xLog.AddLine($"Load Error:{xStudent.ErrorMessage}");
                        r.Success = false;
                        r.ErrorMessage = xUser.ErrorMessage;
                    }
                } else {
                    r.Success = false;
                    r.ErrorMessage = "当前用户未注册！";
                    r.ErrorNumber = xUser.ErrorNumber;
                    xLog.AddLine($"CheckParent Error:{r.ErrorMessage}");
                }
                return r;
            } catch (Exception ex) {
                xLog.AddLine("Exception:\n" + ex.Message);
                r.Success = false;
                r.ErrorMessage = ex.Message;
                return r;
            } finally {
                xLog.AddLine($"Results:{Environment.NewLine}{JsonConvert.SerializeObject(r, Formatting.Indented)}");
                xLog.Save();
            }
        }
        #endregion
    }
}
