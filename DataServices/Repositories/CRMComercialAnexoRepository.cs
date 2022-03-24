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
    public class CRMComercialAnexoRepository : RepositoryBase<CRM_COMERCIAL_ANEXO>, ICRMComercialAnexoRepository
    {
        public List<CRM_COMERCIAL_ANEXO> GetAllItens()
        {
            return Db.CRM_COMERCIAL_ANEXO.ToList();
        }

        public CRM_COMERCIAL_ANEXO GetItemById(Int32 id)
        {
            IQueryable<CRM_COMERCIAL_ANEXO> query = Db.CRM_COMERCIAL_ANEXO.Where(p => p.CRCA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
 