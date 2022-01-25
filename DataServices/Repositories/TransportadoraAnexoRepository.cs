using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class TransportadoraAnexoRepository : RepositoryBase<TRANSPORTADORA_ANEXO>, ITransportadoraAnexoRepository
    {
        public List<TRANSPORTADORA_ANEXO> GetAllItens()
        {
            return Db.TRANSPORTADORA_ANEXO.ToList();
        }

        public TRANSPORTADORA_ANEXO GetItemById(Int32 id)
        {
            IQueryable<TRANSPORTADORA_ANEXO> query = Db.TRANSPORTADORA_ANEXO.Where(p => p.TRAX_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
