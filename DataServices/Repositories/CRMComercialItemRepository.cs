using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CRMComercialItemRepository : RepositoryBase<CRM_COMERCIAL_ITEM>, ICRMComercialItemRepository
    {
        public List<CRM_COMERCIAL_ITEM> GetAllItens()
        {
            return Db.CRM_COMERCIAL_ITEM.ToList();
        }

        public CRM_COMERCIAL_ITEM GetItemById(Int32 id)
        {
            IQueryable<CRM_COMERCIAL_ITEM> query = Db.CRM_COMERCIAL_ITEM.Where(p => p.CRCI_IN_ATIVO == id);
            return query.FirstOrDefault();
        }

        public CRM_COMERCIAL_ITEM GetItemByProduto(Int32 id)
        {
            IQueryable<CRM_COMERCIAL_ITEM> query = Db.CRM_COMERCIAL_ITEM.Where(p => p.PROD_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
