using System;

namespace TinyApi.Models.Result
{
    public class DataRegisterResult
    {
        public Guid ParentId { get; set; }

        public Guid StudentId { get; set; }

        public Guid TeamId { get; set; }
    }
}