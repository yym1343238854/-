using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    /// <summary>
    /// 小程序Session
    /// </summary>
    public partial class TinyAppSession : Base
    {
        public bool Add(string openId,string sessionKey,Guid sessionId)
        {
            try
            {
                var q = Db.TinyAppSession.Where(x => x.OpenId == openId);
                if (q.Any())
                {
                    var tas = q.SingleOrDefault();
                    tas.SessionKey = sessionKey;
                    tas.SessionId = sessionId;
                    tas.LastActiveTime = DateTime.Now;
                }
                else
                {
                    var tas = new TinyAppSession
                    {
                        LastActiveTime = DateTime.Now,
                        OpenId = openId,
                        SessionId = sessionId,
                        SessionKey = sessionKey,
                        Status = 1,
                        Checkintime = DateTime.Now
                    };
                    Db.TinyAppSession.Add(tas);
                }
                Db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return false;
            }
        }

        public bool Load(string openId,out Guid sessionId)
        {
            sessionId = Guid.Empty;
            var q = Db.TinyAppSession.Where(x => x.OpenId == openId);
            if (q.Any())
            {
                var tas = q.SingleOrDefault();
                sessionId = tas.SessionId;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
