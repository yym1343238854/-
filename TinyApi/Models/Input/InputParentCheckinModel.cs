namespace TinyApi.Models.Input
{
    /// <summary>
    /// 家长打卡模型
    /// </summary>
    public class InputParentCheckinModel:InputBaseModel
    {
        /// <summary>
        /// 健康码 1 - 绿色 2 橙色  3 红色
        /// </summary>
        public byte ColorStatus { get; set; }
    }
}