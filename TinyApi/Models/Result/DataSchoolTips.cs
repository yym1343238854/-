using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TinyApi.Models.Result {
    /// <summary>
    /// 校园动态
    /// </summary>
    public class DataSchoolTips {
        /// <summary>
        /// 标签页个数
        /// </summary>
        public int TabCount { get; set; }

        /// <summary>
        /// 场所类型列表
        /// </summary>
        public List<RoomTab> RoomTypes { get; set; }
    }

    /// <summary>
    /// 场所类型
    /// </summary>
    public class RoomTab {
        /// <summary>
        /// 类型
        /// </summary>
        public byte TabId { get; set; }

        /// <summary>
        /// 类型名称（tab标题）
        /// </summary>
        public string TabName { get; set; }

        /// <summary>
        /// 场所列表
        /// </summary>
        public List<RoomTips> Rooms { get; set; }
    }

    /// <summary>
    /// 场所的动态列表
    /// </summary>
    public class RoomTips {
        /// <summary>
        /// 场所编号
        /// </summary>
        public Guid RoomId { get; set; }

        /// <summary>
        /// 场所名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 动态列表
        /// </summary>
        public List<Tip> Tips { get; set; }
    }
}