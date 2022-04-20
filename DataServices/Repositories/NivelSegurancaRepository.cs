using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class NivelSegurancaRepository : RepositoryBase<NIVEL_SEGURANCA>, INivelSegurancaRepository
    {
        public NIVEL_SEGURANCA GetItemById(Int32 id)
        {
            IQueryable<NIVEL_SEGURANCA> query = Db.NIVEL_SEGURANCA;
            query = query.Where(p => p.NISE_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<NIVEL_SEGURANCA> GetAllItens(Int32 idAss)
        {
            IQueryable<NIVEL_SEGURANCA> query = Db.NIVEL_SEGURANCA;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
