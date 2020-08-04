using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class ParentCheckin:Base
    {
        public bool ListMonthRecords(Guid parentId,int year,int month, out List<ParentCheckin> records)
        {
            records = Db.ParentCheckin.Where(x => x.ParentId == parentId && x.SpecDay.Year == year && x.SpecDay.Month == month && x.Status == (byte)EnumDataStatus.Normal).OrderBy(x => x.SpecDay).ToList();
            return true;
        }
    }
}
