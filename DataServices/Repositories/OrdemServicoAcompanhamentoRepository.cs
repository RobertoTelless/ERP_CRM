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
    public class OrdemServicoAcompanhamentoRepository : RepositoryBase<ORDEM_SERVICO_ACOMPANHAMENTO>, IOrdemServicoAcompanhamentoRepository
    {
        public List<ORDEM_SERVICO_ACOMPANHAMENTO> GetByOs(ORDEM_SERVICO conta)
        {
            IQueryable<ORDEM_SERVICO_ACOMPANHAMENTO> query = Db.ORDEM_SERVICO_ACOMPANHAMENTO;
            query = query.Where(x => x.ORSE_CD_ID == conta.ORSE_CD_ID);
            return query.ToList<ORDEM_SERVICO_ACOMPANHAMENTO>();
        }
    }
}
