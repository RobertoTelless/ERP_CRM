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
    public class ContaReceberTagRepository : RepositoryBase<CONTA_RECEBER_TAG>, IContaReceberTagRepository
    {
        public List<CONTA_RECEBER_TAG> GetAllItens()
        {
            return Db.CONTA_RECEBER_TAG.ToList();
        }

        public CONTA_RECEBER_TAG GetItemById(Int32 id)
        {
            IQueryable<CONTA_RECEBER_TAG> query = Db.CONTA_RECEBER_TAG.Where(p => p.CRTA_IN_ATIVO == id);
            return query.FirstOrDefault();
        }
    }
}
 