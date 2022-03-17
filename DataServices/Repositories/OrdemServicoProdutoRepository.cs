using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class OrdemServicoProdutoRepository : RepositoryBase<ORDEM_SERVICO_PRODUTO>, IOrdemServicoProdutoRepository
    {
        public ORDEM_SERVICO_PRODUTO CheckExist(ORDEM_SERVICO_PRODUTO item)
        {
            IQueryable<ORDEM_SERVICO_PRODUTO> query = Db.ORDEM_SERVICO_PRODUTO;
            query = query.Where(x => x.ORSE_CD_ID == item.ORSE_CD_ID);
            query = query.Where(x => x.PROD_CD_ID == item.PROD_CD_ID);
            return query.FirstOrDefault();
        }

        public List<ORDEM_SERVICO_PRODUTO> GetAllByOs(Int32 id)
        {
            IQueryable<ORDEM_SERVICO_PRODUTO> query = Db.ORDEM_SERVICO_PRODUTO.Where(x => x.OSPR_IN_ATIVO == 1);
            query = query.Where(x => x.ORSE_CD_ID == id);
            return query.ToList();
        }
    }
}
