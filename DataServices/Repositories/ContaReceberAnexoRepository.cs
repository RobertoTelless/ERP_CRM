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
    public class ContaReceberAnexoRepository : RepositoryBase<CONTA_RECEBER_ANEXO>, IContaReceberAnexoRepository
    {
        public List<CONTA_RECEBER_ANEXO> GetAllItens()
        {
            return Db.CONTA_RECEBER_ANEXO.ToList();
        }

        public CONTA_RECEBER_ANEXO GetItemById(Int32 id)
        {
            IQueryable<CONTA_RECEBER_ANEXO> query = Db.CONTA_RECEBER_ANEXO.Where(p => p.CRAN_CD_ID == id);
            return query.FirstOrDefault();
        }

    }
}
 