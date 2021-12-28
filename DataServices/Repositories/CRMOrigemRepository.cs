using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CRMOrigemRepository : RepositoryBase<CRM_ORIGEM>, ICRMOrigemRepository
    {
        public List<CRM_ORIGEM> GetAllItens()
        {
            return Db.CRM_ORIGEM.ToList();
        }

        public CRM_ORIGEM GetItemById(Int32 id)
        {
            IQueryable<CRM_ORIGEM> query = Db.CRM_ORIGEM.Where(p => p.CROR_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 