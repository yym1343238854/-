using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 家长主页信息
    /// </summary>
    public class DataParentHome 
    {
        /// <summary>
        /// 班级学生具体信息
        /// </summary>
        public DataTeamInfo Team { get; set; }

        /// <summary>
        /// 今日信息
        /// </summary>
        public string TodayString { get; set; }

        /// <summary>
        /// 上课状态(0.上课 1.放假 3.停课)
        /// </summary>
        public byte HolidayStatus { get; set; }

        /// <summary>
        /// 是否测量过体温
        /// </summary>
        public bool HasFamilyMeasureBbt { get; set; }

        /// <summary>
        /// 是否需要家庭测温(true:need false:do not need)
        /// </summary>
        public bool NeedFamilyTest { get; set; }

        /// <summary>
        /// 上传体温照片类型 (1.必须上传照片 2.可选上传 3.不可上传)
        /// </summary>
        public byte UploadBbtPhotoType { get; set; }

        
        /// <summary>
        /// 家庭成员打卡状况列表
        /// </summary>
        public List<FamilyMemberCheckin> FamilyMembers { get; set; }

        /// <summary>
        /// 家庭测温记录
        /// </summary>
        public BbtRecord FamilyBbt { get; set; }

        /// <summary>
        /// 学校晨测
        /// </summary>
        public BbtRecord FirstSchoolBbt { get; set; }

        /// <summary>
        /// 学校午测
        /// </summary>
        public BbtRecord SecondSchoolBbt { get; set; }

        /// <summary>
        /// 最新动态
        /// </summary>
        public List<Tip> Tips { get; set; }

        /// <summary>
        /// 到校时间
        /// </summary>
        public DaySchedule CheckinSchedule { get; set; }

        /// <summary>
        /// 离校时间
        /// </summary>
        public DaySchedule CheckoutSchedule { get; set; }

        public bool HasParentCheckin { get; set; }

        /// <summary>
        /// 待完成任务
        /// </summary>
        public int TodoTasks { get; set; }
    }

    /// <summary>
    /// 家庭成员打卡状况
    /// </summary>
    public class FamilyMemberCheckin {
        /// <summary>
        /// 家长编号
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string AvatarUrl { get; set; }

        /// <summary>
        /// 是否打卡
        /// </summary>
        public bool HasCheckin { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public byte ColorStatus { get; set; }

        /// <summary>
        /// CSS类
        /// </summary>
        public string CssClass { get; set; }
    }

    /// <summary>
    /// 体温记录
    /// </summary>
    public class BbtRecord {

        /// <summary>
        /// 温度
        /// </summary>
        public decimal Temperature { get; set; }

        /// <summary>
        /// 体温是否正常
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// 测温时间
        /// </summary>
        //public DateTime MeasureTime { get; set; }

        /// <summary>
        /// 测温时间
        /// </summary>
        public string MeasureTimeStr { get; set; }
    }

    public class BbtRecordDetail:BbtRecord
    {
        /// <summary>
        /// 测温类型 1 晨测 2 午测 3 家测 4 抽测
        /// </summary>
       public byte Type { get; set; }
    }

    /// <summary>
    /// 动态
    /// </summary>
    public class Tip {
        /// <summary>
        /// 动态时间
        /// </summary>
        //public DateTime Time { get; set; }

        public Guid TipsId { get; set; }

        /// <summary>
        /// 动态时间
        /// </summary>
        public string TimeStr { get; set; }

        /// <summary>
        /// 动态内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 动态文字显示颜色
        /// </summary>
        public string FontColor { get; set; }
    }

    /// <summary>
    /// 计划
    /// </summary>
    public class DaySchedule {
        /// <summary>
        /// 类型(1.上学 2.放学)
        /// </summary>
        public byte Type { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        //public DateTime StartTime { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTimeStr { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        //public DateTime EndTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTimeStr { get; set; }
    }
}