namespace TinyApi.Models.Input
{
    /// <summary>
    /// 图片文件信息
    /// </summary>
    public class ImageFileModel
    {
        /// <summary>
        /// url
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 文件大小
        /// </summary>
        public int FileSize { get; set; }
        /// <summary>
        /// 旧名
        /// </summary>
        public string OldName { get; set; }
        /// <summary>
        /// 新名称
        /// </summary>
        public string NewName { get; set; }
        /// <summary>
        /// 文件后缀
        /// </summary>
        public string FileExt { get; set; }

    }
}