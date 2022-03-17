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
    public class OrdemServicoAgendaRepository : RepositoryBase<ORDEM_SERVICO_AGENDA>, IOrdemServicoAgendaRepository
    {
        public List<ORDEM_SERVICO_AGENDA> GetAgendaByOs(ORDEM_SERVICO item)
        {
            IQueryable<ORDEM_SERVICO_AGENDA> query = Db.ORDEM_SERVICO_AGENDA.Where(x => x.OSAG_IN_ATIVO == 1);
            query = query.Where(x => x.ORSE_CD_ID == item.ORSE_CD_ID);
            return query.ToList<ORDEM_SERVICO_AGENDA>();
        }
    }
}
