using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class RegimeTributarioRepository : RepositoryBase<REGIME_TRIBUTARIO>, IRegimeTributarioRepository
    {
        public REGIME_TRIBUTARIO GetItemById(Int32 id)
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO;
            query = query.Where(p => p.RETR_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<REGIME_TRIBUTARIO> GetAllItens(Int32 idAss)
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO.Where(p => p.RETR_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<REGIME_TRIBUTARIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<REGIME_TRIBUTARIO> query = Db.REGIME_TRIBUTARIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
