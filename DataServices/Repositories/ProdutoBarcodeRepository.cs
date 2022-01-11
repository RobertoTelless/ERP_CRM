using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class ProdutoBarcodeRepository : RepositoryBase<PRODUTO_BARCODE>, IProdutoBarcodeRepository
    {
        public List<PRODUTO_BARCODE> GetAllItens()
        {
            return Db.PRODUTO_BARCODE.ToList();
        }

        public PRODUTO_BARCODE GetItemById(Int32 id)
        {
            IQueryable<PRODUTO_BARCODE> query = Db.PRODUTO_BARCODE.Where(p => p.PRBC_CD_ID == id);
            return query.FirstOrDefault();
        }

        public PRODUTO_BARCODE GetByProd(Int32 prod)
        {
            IQueryable<PRODUTO_BARCODE> query = Db.PRODUTO_BARCODE;
            query = query.Where(x => x.PROD_CD_ID == prod);
            return query.FirstOrDefault();
        }
    }
}
