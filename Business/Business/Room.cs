using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business {
    public partial class Room : Base {
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="measureType"></param>
        /// <param name="location"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherIds">区域负责人</param>
        /// <returns></returns>
        public bool Create(byte type, string name, byte measureType, string location, Guid schoolId, List<Tuple<Guid, byte>> teacherIds) {
            try {
                var school = Db.School.FirstOrDefault(x => x.SchoolId == schoolId);
                if (school == null) {
                    SetError("指定学校编号不存在");
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
                if (Db.Room.Any(x => x.Name.Equals(name) && x.Status == (byte)EnumDataStatus.Normal && x.SchoolId == schoolId)) {
                    SetError("名称已存在");
                    return false;
                }

                Guid roomId = Guid.NewGuid();
                Room room = new Room() {
                    RoomId = roomId,
                    Checkintime = DateTime.Now,
                    LastUpdateTime = DateTime.Now,
                    Type = type,
                    Name = name,
                    MeasureType = measureType,
                    SchoolId = schoolId,
                    Location = location,
                    Status = (byte)EnumDataStatus.Normal,
                };
                Db.Room.Add(room);
                school.RoomNum += 1; //学校班级数量

                //负责人
                if (teacherIds != null) {
                    foreach (var item in teacherIds) {
                        RoomTeacher rm = new RoomTeacher() {
                            RoomId = roomId,
                            TeacherId = item.Item1,
                            Type = item.Item2,
                            Checkintime = DateTime.Now,
                            Status = (byte)EnumDataStatus.Normal
                        };
                        Db.RoomTeacher.Add(rm);
                    }
                }
                Db.SaveChanges();
                return true;
            } catch (Exception ex) {
                SetError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="measureType"></param>
        /// <param name="location"></param>
        /// <param name="teacherIds">新增负责人</param>
        /// <param name="removeTeacherIds">需要删除的负责人</param>
        /// <returns></returns>
        public bool Update(Guid roomId, byte type, string name, byte measureType, string location, List<Tuple<Guid, byte>> teacherIds, List<Tuple<Guid, byte>> removeTeacherIds) {
            try {
                Room room = Db.Room.FirstOrDefault(x => x.RoomId == roomId && x.Status == (byte)EnumDataStatus.Normal);
                if (room == null) {
                    SetError("指定场所编号不存在");
                    ErrorNumber = (int)EnumErrors.信息不存在;
                    return false;
                }
                if (Db.Room.Any(x => x.Name.Equals(name) && x.Status == (byte)EnumDataStatus.Normal && x.SchoolId == room.SchoolId && x.RoomId != roomId)) {
                    SetError("名称已存在");
                    return false;
                }

                room.Type = type;
                room.LastUpdateTime = DateTime.Now;
                room.Location = location;
                room.MeasureType = measureType;
                room.Name = name;

                //添加负责人
                if (teacherIds != null) {
                    foreach (var item in teacherIds) {
                        // 若存在已被删除的负责人，将其恢复
                        var rt = room.RoomTeacher.Where(x => x.TeacherId == item.Item1).FirstOrDefault();
                        if (rt != null) {
                            rt.Status = (byte)EnumDataStatus.Normal;
                            rt.Type = item.Item2;
                            continue;
                        }
                        RoomTeacher rm = new RoomTeacher() {
                            RoomId = roomId,
                            TeacherId = item.Item1,
                            Type = item.Item2,
                            Checkintime = DateTime.Now,
                            Status = (byte)EnumDataStatus.Normal
                        };
                        Db.RoomTeacher.Add(rm);
                    }
                }

                //删除负责人
                if (removeTeacherIds != null) {
                    foreach (var item in removeTeacherIds) {
                        var rt = room.RoomTeacher.FirstOrDefault(x => x.TeacherId == item.Item1);
                        if (rt == null) continue;
                        rt.Status = (byte)EnumDataStatus.Delete;
                    }
                }

                Db.SaveChanges();
                return true;
            } catch (Exception ex) {
                SetError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="roomIds"></param>
        /// <returns></returns>
        public bool Delete(List<Guid> roomIds) {
            try {
                if (roomIds != null) {
                    foreach (var item in roomIds) {
                        var room = Db.Room.FirstOrDefault(x => x.RoomId == item && x.Status == (byte)EnumDataStatus.Normal);
                        if(room == null) {
                            SetError("指定场所编号不存在");
                            ErrorNumber = (int)EnumErrors.信息不存在;
                            return false;
                        }
                        room.Status = (byte)EnumDataStatus.Delete;
                    }
                    Db.SaveChanges();
                }
                return true;
            } catch (Exception ex) {
                SetError(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        public bool List(string keyword, byte roomType, out List<Room> list) {
            list = null;
            try {
                return true;
            } catch (Exception ex) {
                SetError(ex.Message);
                return false;
            }
        }

    }
}
