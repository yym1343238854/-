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
    
    public partial class Parent
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Parent()
        {
            this.ExceptionCase = new HashSet<ExceptionCase>();
            this.StudentParent = new HashSet<StudentParent>();
            this.ParentCheckin = new HashSet<ParentCheckin>();
            this.StudentBbtRecord = new HashSet<StudentBbtRecord>();
        }
    
        public System.Guid ParentId { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public byte HealthyStatus { get; set; }
        public string IdNo { get; set; }
        public string Mobile { get; set; }
        public System.DateTime Checkintime { get; set; }
        public Nullable<System.DateTime> LastAccessTime { get; set; }
        public byte ColorStatus { get; set; }
        public string UnionId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExceptionCase> ExceptionCase { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentParent> StudentParent { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParentCheckin> ParentCheckin { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StudentBbtRecord> StudentBbtRecord { get; set; }
        public virtual WeiXinUser WeiXinUser { get; set; }
    }
}
