using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IAtendimentoAgendaService : IServiceBase<ATENDIMENTO_AGENDA>
    {
        Int32 Create(ATENDIMENTO_AGENDA item, LOG log);
        Int32 Create(ATENDIMENTO_AGENDA item);

        List<ATENDIMENTO_AGENDA> GetAgendaByAtendimento(ATENDIMENTO item);

    }
}
