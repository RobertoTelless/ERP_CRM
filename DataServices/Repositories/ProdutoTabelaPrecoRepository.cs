using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoTabelaPrecoRepository : RepositoryBase<PRODUTO_TABELA_PRECO>, IProdutoTabelaPrecoRepository
    {
        public PRODUTO_TABELA_PRECO CheckExist(PRODUTO_TABELA_PRECO item)
        {
            IQueryable<PRODUTO_TABELA_PRECO> query = Db.PRODUTO_TABELA_PRECO;
            query = query.Where(p => p.PROD_CD_ID == item.PROD_CD_ID);
            query = query.Where(p => p.FILI_CD_ID == item.FILI_CD_ID);
            return query.FirstOrDefault();
        }

        public PRODUTO_TABELA_PRECO GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_TABELA_PRECO> query = Db.PRODUTO_TABELA_PRECO;
            query = query.Where(p => p.PRTP_CD_ID == id);
            return query.FirstOrDefault();
        }

        public PRODUTO_TABELA_PRECO GetByProdFilial(Int32 prod, Int32 fili)
        {
            IQueryable<PRODUTO_TABELA_PRECO> query = Db.PRODUTO_TABELA_PRECO;
            query = query.Where(p => p.PROD_CD_ID == prod);
            query = query.Where(p => p.FILI_CD_ID == fili);
            return query.FirstOrDefault();
        }
    }
}
