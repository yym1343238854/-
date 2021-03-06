//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Business
{
    using System;
    using System.Collections.Generic;
    
    public partial class Attachment
    {
        public System.Guid AttachmentId { get; set; }
        public int Id { get; set; }
        public byte Status { get; set; }
        public string Url { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string OriginalName { get; set; }
        public System.DateTime Checkintime { get; set; }
        public int FileSize { get; set; }
        public Nullable<System.Guid> AirRecordId { get; set; }
        public Nullable<System.Guid> DisinfectRecordId { get; set; }
    
        public virtual AirRecord AirRecord { get; set; }
        public virtual DisinfectRecord DisinfectRecord { get; set; }
    }
}
