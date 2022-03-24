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
    public class CRMComercialComentarioRepository : RepositoryBase<CRM_COMERCIAL_COMENTARIO_NOVA>, ICRMComercialComentarioRepository
    {
        public List<CRM_COMERCIAL_COMENTARIO_NOVA> GetAllItens()
        {
            return Db.CRM_COMERCIAL_COMENTARIO_NOVA.ToList();
        }

        public CRM_COMERCIAL_COMENTARIO_NOVA GetItemById(Int32 id)
        {
            IQueryable<CRM_COMERCIAL_COMENTARIO_NOVA> query = Db.CRM_COMERCIAL_COMENTARIO_NOVA.Where(p => p.CRMC_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 