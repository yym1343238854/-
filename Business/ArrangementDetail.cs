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
    
    public partial class ArrangementDetail
    {
        public System.Guid ArrangementDetailId { get; set; }
        public int Id { get; set; }
        public byte Status { get; set; }
        public System.DateTime StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public System.DateTime Checkintime { get; set; }
        public System.Guid ArrangementId { get; set; }
    
        public virtual Arrangement Arrangement { get; set; }
    }
}
