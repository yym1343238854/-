using System;

namespace TinyApi.Models.Result
{
    public class DataTeamInfo
    {
        public string SchoolName { get; set; }

        public string TeamName { get; set; }

        public Guid TeamId { get; set; }

        public string RoomName { get; set; }

        public int StudentNum { get; set; }

        public int ParentNum { get; set; }

        public int TeacherNum { get; set; }

        public DataStudentInfo Student { get; set; }
    }

    public class DataStudentInfo
    {
        public string AvatarUrl { get; set; }

        public string Name { get; set; }

        public string IdNo { get; set; }

        public int No { get; set; }

        public Guid StudentId { get; set; }
    }
}