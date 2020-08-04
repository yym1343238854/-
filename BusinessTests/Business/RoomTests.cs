using Microsoft.VisualStudio.TestTools.UnitTesting;
using Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Tests {
    [TestClass()]
    public class RoomTests {
        [TestMethod()]
        public void CreateTest() {
            Room xRoom = new Room();
            try {
                xRoom.Create((byte)Base.EnumRoomType.食堂, "一食堂", 0, "", new Guid("DB8EE83F-57C7-41ED-9775-8A3B932ED996"), new List<Guid>());
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }

        [TestMethod()]
        public void UpdateTest() {
            Room xRoom = new Room();
            try {
                List<Guid> teacherIds = new List<Guid>();
                teacherIds.Add(new Guid("3E9C4FBD-97CD-4A5A-B02C-FB5A8DEB667A"));
                teacherIds.Add(new Guid("C2ED22D9-9F25-47BD-A13F-9D73403A70B1"));
                var r = xRoom.Update(new Guid("385FBE8A-C9FB-4EFF-9AF6-C07132B3EEA2"), (byte)Base.EnumRoomType.教室, "101教室", 3, "朝阳楼101", teacherIds, new List<Guid>());
            } catch (Exception ex) {
                Assert.Fail(ex.Message);
            }
        }
    }
}