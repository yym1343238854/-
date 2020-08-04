using System;

namespace TinyApi.Models.Input
{
    public class InputTeamIdWidthStudentIdModel : InputBaseModel
    {
        public Guid TeamId { get; set; }

        public Guid? StudentId { get; set; }
    }
}