using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TipoMetadadoRepository : RepositoryBase<TIPO_METADADO>, ITipoMetadadoRepository
    {
        public TIPO_METADADO GetItemById(Int32 id)
        {
            IQueryable<TIPO_METADADO> query = Db.TIPO_METADADO;
            query = query.Where(p => p.TIME_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<TIPO_METADADO> GetAllItens(Int32 idAss)
        {
            IQueryable<TIPO_METADADO> query = Db.TIPO_METADADO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
