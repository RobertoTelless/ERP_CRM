using System;
using System.Collections.Generic;
using EntitiesServices.Model;  
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class DocumentoHistoricoRepository : RepositoryBase<DOCUMENTO_HISTORICO>, IDocumentoHistoricoRepository
    {
        public DOCUMENTO_HISTORICO GetItemById(Int32 id)
        {
            IQueryable<DOCUMENTO_HISTORICO> query = Db.DOCUMENTO_HISTORICO;
            query = query.Where(p => p.DOHI_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<DOCUMENTO_HISTORICO> GetAllItens(Int32 idAss)
        {
            IQueryable<DOCUMENTO_HISTORICO> query = Db.DOCUMENTO_HISTORICO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }
    }
}
