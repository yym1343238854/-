using System;
using System.Collections.Generic;

namespace TinyApi.Models.Result
{
    public class DataDecodeEncryt
    {
        public string UnionId { get; set; }

        public string OpenId { get; set; }

        public List<DataTeacherRole> Teachers { get; set; }

        public List<DataParentRole> Parents { get; set; }

    }

    public class DataTeacherRole
    {
        public Guid TeacherId { get; set; }

        public Guid TeamId { get; set; }
    }

    public class DataParentRole
    {
        public Guid ParentId { get; set; }

        public Guid StudentId { get; set; }
    }
}