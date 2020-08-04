namespace TinyApi.Models.Input
{
    /// <summary>
    /// 解压模型
    /// </summary>
    public class InputDecodeEncryptModel
    {
        public string OpenId { get; set; }

        /// <summary>
        /// 加密数据
        /// </summary>
        public string EncryptedData { get; set; }

        /// <summary>
        /// 向量
        /// </summary>
        public string Iv { get; set; }

        public int From { get; set; }

        public int TeamId { get; set; }
    }
}