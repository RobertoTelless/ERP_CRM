using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoGradeRepository : RepositoryBase<PRODUTO_GRADE>, IProdutoGradeRepository
    {
        public List<PRODUTO_GRADE> GetAllItens()
        {
            return Db.PRODUTO_GRADE.ToList();
        }

        public PRODUTO_GRADE GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_GRADE> query = Db.PRODUTO_GRADE.Where(p => p.PRGR_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
