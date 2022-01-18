using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoKitRepository : RepositoryBase<PRODUTO_KIT>, IProdutoKitRepository
    {
        public List<PRODUTO_KIT> GetAllItens()
        {
            return Db.PRODUTO_KIT.ToList();
        }

        public PRODUTO_KIT GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_KIT> query = Db.PRODUTO_KIT.Where(p => p.PRKI_CD_KIT == id);
            return query.FirstOrDefault();
        }

        public PRODUTO_KIT GetByProd(Int32 prod)
        {
            IQueryable<PRODUTO_KIT> query = Db.PRODUTO_KIT;
            query = query.Where(x => x.PROD_CD_ID == prod);
            return query.FirstOrDefault();
        }
    }
}
