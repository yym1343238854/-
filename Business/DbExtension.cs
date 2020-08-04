using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public partial class HealthySchoolEntities : DbContext
    {
        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                sb.AppendLine(ex.Message);

                sb.AppendLine("验证异常：");
                foreach (var item in ((DbEntityValidationException)ex).EntityValidationErrors)
                {
                    foreach (var error in item.ValidationErrors)
                    {
                        sb.AppendLine(error.ErrorMessage);
                    }
                }
                throw new Exception(sb.ToString());
            }
            catch (DbUpdateException ex)
            {
                var message = HandleException(ex);
                throw new Exception(message);
            }

        }

        public string HandleException(Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine(exception.Message);

            if (exception is DbUpdateException dbUpdateEx)
            {
                if (dbUpdateEx.InnerException != null
                        && dbUpdateEx.InnerException.InnerException != null)
                {
                    if (dbUpdateEx.InnerException.InnerException is SqlException sqlException)
                    {
                        sb.AppendLine($"{sqlException.Message}");
                    }
                }
            }
            return sb.ToString();
        }
    }
}
