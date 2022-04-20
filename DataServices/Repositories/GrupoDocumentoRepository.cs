using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;

namespace DataServices.Repositories
{
    public class GrupoDocumentoRepository : RepositoryBase<GRUPO_DOCUMENTO>, IGrupoDocumentoRepository
    {
        public GRUPO_DOCUMENTO CheckExist(GRUPO_DOCUMENTO conta, Int32 idAss)
        {
            IQueryable<GRUPO_DOCUMENTO> query = Db.GRUPO_DOCUMENTO;
            query = query.Where(p => p.GRDC_NM_NOME == conta.GRDC_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public GRUPO_DOCUMENTO GetItemById(Int32 id)
        {
            IQueryable<GRUPO_DOCUMENTO> query = Db.GRUPO_DOCUMENTO;
            query = query.Where(p => p.GRDC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<GRUPO_DOCUMENTO> GetAllItens(Int32 idAss)
        {
            IQueryable<GRUPO_DOCUMENTO> query = Db.GRUPO_DOCUMENTO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
 