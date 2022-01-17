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
    public class ContaPagarAnexoRepository : RepositoryBase<CONTA_PAGAR_ANEXO>, IContaPagarAnexoRepository
    {
        public List<CONTA_PAGAR_ANEXO> GetAllItens()
        {
            return Db.CONTA_PAGAR_ANEXO.ToList();
        }

        public CONTA_PAGAR_ANEXO GetItemById(Int32 id)
        {
            IQueryable<CONTA_PAGAR_ANEXO> query = Db.CONTA_PAGAR_ANEXO.Where(p => p.CPAN_CD_ID == id);
            return query.FirstOrDefault();
        }
    }

}
 