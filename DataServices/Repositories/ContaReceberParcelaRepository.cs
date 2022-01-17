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
    public class ContaReceberParcelaRepository : RepositoryBase<CONTA_RECEBER_PARCELA>, IContaReceberParcelaRepository
    {
        public List<CONTA_RECEBER_PARCELA> GetAllItens()
        {
            return Db.CONTA_RECEBER_PARCELA.ToList();
        }

        public CONTA_RECEBER_PARCELA GetItemById(Int32 id)
        {
            IQueryable<CONTA_RECEBER_PARCELA> query = Db.CONTA_RECEBER_PARCELA.Where(p => p.CRPA_CD_ID == id);
            query = query.Include(p => p.CONTA_RECEBER);
            return query.FirstOrDefault();
        }

    }
}
 