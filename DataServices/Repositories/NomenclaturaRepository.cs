using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class NomenclaturaRepository : RepositoryBase<NOMENCLATURA_BRAS_SERVICOS>, INomenclaturaRepository
    {
        public NOMENCLATURA_BRAS_SERVICOS GetItemById(Int32 id)
        {
            IQueryable<NOMENCLATURA_BRAS_SERVICOS> query = Db.NOMENCLATURA_BRAS_SERVICOS;
            query = query.Where(p => p.NBSE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<NOMENCLATURA_BRAS_SERVICOS> GetAllItens()
        {
            IQueryable<NOMENCLATURA_BRAS_SERVICOS> query = Db.NOMENCLATURA_BRAS_SERVICOS;
            return query.ToList();
        }
    }
}
