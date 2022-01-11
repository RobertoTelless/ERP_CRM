using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoOrigemRepository : RepositoryBase<PRODUTO_ORIGEM>, IProdutoOrigemRepository
    {
        public PRODUTO_ORIGEM GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ORIGEM> query = Db.PRODUTO_ORIGEM;
            query = query.Where(p => p.PROR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<PRODUTO_ORIGEM> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_ORIGEM> query = Db.PRODUTO_ORIGEM.Where(p => p.PROR_IN_ATIVO == 1);
            return query.ToList();
        }

        public List<PRODUTO_ORIGEM> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<PRODUTO_ORIGEM> query = Db.PRODUTO_ORIGEM;
            return query.ToList();
        }
    }
}
