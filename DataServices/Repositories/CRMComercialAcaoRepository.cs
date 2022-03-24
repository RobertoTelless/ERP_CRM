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
    public class CRMComercialAcaoRepository : RepositoryBase<CRM_COMERCIAL_ACAO>, ICRMComercialAcaoRepository
    {
        public List<CRM_COMERCIAL_ACAO> GetAllItens(Int32 idUsu)
        {
            IQueryable<CRM_COMERCIAL_ACAO> query = Db.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public CRM_COMERCIAL_ACAO GetItemById(Int32 id)
        {
            IQueryable<CRM_COMERCIAL_ACAO> query = Db.CRM_COMERCIAL_ACAO.Where(p => p.CRCA_IN_ATIVO == id);
            return query.FirstOrDefault();
        }
    }
}
 