using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class AtendimentoAgendaRepository : RepositoryBase<ATENDIMENTO_AGENDA>, IAtendimentoAgendaRepository
    {
        public List<ATENDIMENTO_AGENDA> GetAgendaByAtendimento(ATENDIMENTO item)
        {
            Int32? idAss = SessionMocks.IdAssinante;
            IQueryable<ATENDIMENTO_AGENDA> query = Db.ATENDIMENTO_AGENDA.Where(p => p.ATAG_IN_ATIVO == 1);
            query = query.Where(p => p.ATEN_CD_ID == item.ATEN_CD_ID);
            return query.ToList();
        }
    }
}
