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
    public class ContaPagarRateioRepository : RepositoryBase<CONTA_PAGAR_RATEIO>, IContaPagarRateioRepository
    {
        public CONTA_PAGAR_RATEIO CheckExist(CONTA_PAGAR_RATEIO conta)
        {
            IQueryable<CONTA_PAGAR_RATEIO> query = Db.CONTA_PAGAR_RATEIO;
            query = query.Where(p => p.CECU_CD_ID == conta.CECU_CD_ID);
            return query.FirstOrDefault();
        }

        public List<CONTA_PAGAR_RATEIO> GetAllItens()
        {
            return Db.CONTA_PAGAR_RATEIO.ToList();
        }

        public CONTA_PAGAR_RATEIO GetItemById(Int32 id)
        {
            IQueryable<CONTA_PAGAR_RATEIO> query = Db.CONTA_PAGAR_RATEIO.Where(p => p.CPRA_CD_ID == id);
            query = query.Include(p => p.CONTA_PAGAR);
            return query.FirstOrDefault();
        }
    }

}
 