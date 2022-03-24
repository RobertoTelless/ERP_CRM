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
    public class CRMComercialContatoRepository : RepositoryBase<CRM_COMERCIAL_CONTATO>, ICRMComercialContatoRepository
    {
        public List<CRM_COMERCIAL_CONTATO> GetAllItens()
        {
            return Db.CRM_COMERCIAL_CONTATO.ToList();
        }

        public CRM_COMERCIAL_CONTATO GetItemById(Int32 id)
        {
            IQueryable<CRM_COMERCIAL_CONTATO> query = Db.CRM_COMERCIAL_CONTATO.Where(p => p.CRCO_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 