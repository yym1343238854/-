using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business {
    public partial class Document : Base {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="news"></param>
        /// <returns></returns>
        public bool List(int pageIndex, int pageSize, out List<Document> documents) {
            documents = null;
            try {
                documents = Db.Document.Where(x => x.Status == (byte)EnumNewsStatus.发布)
                    .OrderByDescending(x => x.PublishTime).Skip((pageIndex - 1) * pageSize).Take(pageSize)
                    .ToList();
                return true;
            } catch (Exception ex) {
                SetError(ex);
                return false;
            }
        }

    }
}
