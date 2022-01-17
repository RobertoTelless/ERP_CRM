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
    public class ContaPagarTagRepository : RepositoryBase<CONTA_PAGAR_TAG>, IContaPagarTagRepository
    {
        public List<CONTA_PAGAR_TAG> GetAllItens()
        {
            return Db.CONTA_PAGAR_TAG.ToList();
        }

        public CONTA_PAGAR_TAG GetItemById(Int32 id)
        {
            IQueryable<CONTA_PAGAR_TAG> query = Db.CONTA_PAGAR_TAG.Where(p => p.CPTA_CD_ID == id);
            return query.FirstOrDefault();
        }
    }

}
 