using System;
using System.Collections.Generic;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Linq;
using EntitiesServices.Work_Classes;
using System.Data.Entity;
using CrossCutting;

namespace DataServices.Repositories
{
    public class OrdemServicoAnexoRepository : RepositoryBase<ORDEM_SERVICO_ANEXO>, IOrdemServicoAnexoRepository
    {
        public List<ORDEM_SERVICO_ANEXO> GetAllItens()
        {
            return Db.ORDEM_SERVICO_ANEXO.ToList();
        }

        public ORDEM_SERVICO_ANEXO GetItemById(Int32 id)
        {
            IQueryable<ORDEM_SERVICO_ANEXO> query = Db.ORDEM_SERVICO_ANEXO.Where(p => p.ORSX_CD_ID == id);
            return query.FirstOrDefault();
        }
    }
}
