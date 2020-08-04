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
    
    public partial class School
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public School()
        {
            this.Holiday = new HashSet<Holiday>();
            this.Room = new HashSet<Room>();
            this.Teacher = new HashSet<Teacher>();
            this.Asset = new HashSet<Asset>();
            this.Team = new HashSet<Team>();
        }
    
        public System.Guid SchoolId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public byte Status { get; set; }
        public System.DateTime Checkintime { get; set; }
        public int TeamNum { get; set; }
        public int TeacherNum { get; set; }
        public int StudentNum { get; set; }
        public int RoomNum { get; set; }
        public int ParentNum { get; set; }
        public System.Guid CountyId { get; set; }
        public string Address { get; set; }
        public string SubDomain { get; set; }
        public byte NeedDisInfectPhoto { get; set; }
        public byte NeedAirPhoto { get; set; }
        public bool NeedFamilyTest { get; set; }
        public byte NeedTemperaturePhoto { get; set; }
        public bool EnterMode { get; set; }
        public bool NeedClaimModule { get; set; }
    
        public virtual County County { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Holiday> Holiday { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Room> Room { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Teacher> Teacher { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset> Asset { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Team> Team { get; set; }
    }
}
