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
    public class ContaPagarParcelaRepository : RepositoryBase<CONTA_PAGAR_PARCELA>, IContaPagarParcelaRepository
    {
        public List<CONTA_PAGAR_PARCELA> GetAllItens(Int32 idAss)
        {
            IQueryable<CONTA_PAGAR_PARCELA> query = Db.CONTA_PAGAR_PARCELA.Where(p => p.CPPA_IN_ATIVO == 1);
            query = query.Include(p => p.CONTA_PAGAR);
            query = query.Where(p => p.CONTA_PAGAR.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public CONTA_PAGAR_PARCELA GetItemById(Int32 id)
        {
            IQueryable<CONTA_PAGAR_PARCELA> query = Db.CONTA_PAGAR_PARCELA.Where(p => p.CPPA_CD_ID == id);
            query = query.Include(p => p.CONTA_PAGAR);
            query = query.Include(p => p.CONTA_PAGAR.FORMA_PAGAMENTO);
            return query.FirstOrDefault();
        }
    }
}
 