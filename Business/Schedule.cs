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
    
    public partial class Schedule
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Schedule()
        {
            this.TeamSchedule = new HashSet<TeamSchedule>();
        }
    
        public System.Guid ScheduleId { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
        public byte Status { get; set; }
        public byte Type { get; set; }
        public string StartHhmm { get; set; }
        public string EndHhmm { get; set; }
        public int TeamNum { get; set; }
        public System.DateTime Checkintime { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeamSchedule> TeamSchedule { get; set; }
    }
}
