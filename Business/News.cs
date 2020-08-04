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
    
    public partial class News
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public News()
        {
            this.TeamNews = new HashSet<TeamNews>();
            this.NewsAttachment = new HashSet<NewsAttachment>();
        }
    
        public System.Guid NewsId { get; set; }
        public int Id { get; set; }
        public byte RangeType { get; set; }
        public byte Status { get; set; }
        public byte PeopleType { get; set; }
        public string Title { get; set; }
        public string CoverUrl { get; set; }
        public string Content { get; set; }
        public Nullable<System.DateTime> PublishTime { get; set; }
        public Nullable<int> Hits { get; set; }
        public Nullable<System.DateTime> LastModifyTime { get; set; }
        public System.DateTime Checkintime { get; set; }
        public System.Guid TeacherId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TeamNews> TeamNews { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NewsAttachment> NewsAttachment { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
