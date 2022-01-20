using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAtendimentoAgendaRepository : IRepositoryBase<ATENDIMENTO_AGENDA>
    {
        List<ATENDIMENTO_AGENDA> GetAgendaByAtendimento(ATENDIMENTO item);
    
    }
}
