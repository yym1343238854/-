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
    
    public partial class Visitor
    {
        public System.Guid VisitorId { get; set; }
        public byte Type { get; set; }
        public int Id { get; set; }
        public string Company { get; set; }
        public string IdNo { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public byte Status { get; set; }
        public string Host { get; set; }
        public byte ColorStatus { get; set; }
        public System.DateTime Checkintime { get; set; }
        public System.Guid Recorder { get; set; }
    
        public virtual Teacher Teacher { get; set; }
    }
}
