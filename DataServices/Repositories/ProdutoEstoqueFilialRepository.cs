using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoEstoqueFilialRepository : RepositoryBase<PRODUTO_ESTOQUE_FILIAL>, IProdutoEstoqueFilialRepository
    {
        public List<PRODUTO_ESTOQUE_FILIAL> GetAllItens(Int32 idAss)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL.Where(p => p.PREF_IN_ATIVO == 1);
            return query.ToList<PRODUTO_ESTOQUE_FILIAL>();
        }

        public PRODUTO_ESTOQUE_FILIAL GetByProdFilial(Int32 prod, Int32 fili)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL;
            query = query.Where(p => p.PROD_CD_ID == prod);
            query = query.Where(p => p.FILI_CD_ID == fili);
            return query.FirstOrDefault();
        }

        public List<PRODUTO_ESTOQUE_FILIAL> GetByProd(Int32 id)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL;
            query = query.Where(p => p.PROD_CD_ID == id);
            return query.ToList<PRODUTO_ESTOQUE_FILIAL>();
        }

        public PRODUTO_ESTOQUE_FILIAL CheckExist(PRODUTO_ESTOQUE_FILIAL item, Int32 idAss)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL;
            query = query.Where(p => p.PROD_CD_ID == item.PROD_CD_ID);
            query = query.Where(p => p.FILI_CD_ID == item.FILI_CD_ID);
            return query.FirstOrDefault();
        }

        public PRODUTO_ESTOQUE_FILIAL GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL;
            query = query.Where(p => p.PREF_CD_ID == id);
            return query.FirstOrDefault();
        }

        public PRODUTO_ESTOQUE_FILIAL GetItemById(PRODUTO item)
        {
            IQueryable<PRODUTO_ESTOQUE_FILIAL> query = Db.PRODUTO_ESTOQUE_FILIAL;
            query = query.Where(p => p.PROD_CD_ID == item.PROD_CD_ID).OrderByDescending(x => x.PREF_DT_ULTIMO_MOVIMENTO);
            return query.FirstOrDefault();
        }
    }
}
