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
    public class ContaReceberRateioRepository : RepositoryBase<CONTA_RECEBER_RATEIO>, IContaReceberRateioRepository
    {
        public CONTA_RECEBER_RATEIO CheckExist(CONTA_RECEBER_RATEIO conta)
        {
            IQueryable<CONTA_RECEBER_RATEIO> query = Db.CONTA_RECEBER_RATEIO;
            query = query.Where(p => p.CECU_CD_ID == conta.CECU_CD_ID);
            return query.FirstOrDefault();
        }

        public List<CONTA_RECEBER_RATEIO> GetAllItens()
        {
            return Db.CONTA_RECEBER_RATEIO.ToList();
        }

        public CONTA_RECEBER_RATEIO GetItemById(Int32 id)
        {
            IQueryable<CONTA_RECEBER_RATEIO> query = Db.CONTA_RECEBER_RATEIO.Where(p => p.CRRA_CD_ID == id);
            query = query.Include(p => p.CONTA_RECEBER);
            return query.FirstOrDefault();
        }

    }
}
 