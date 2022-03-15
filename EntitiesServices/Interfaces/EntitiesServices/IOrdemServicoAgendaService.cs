using EntitiesServices.Model;
using ModelServices.Interfaces.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Services
{
    public interface IOrdemServicoAgendaService : IServiceBase<ORDEM_SERVICO_AGENDA>
    {
        Int32 Create(ORDEM_SERVICO_AGENDA item);
        List<ORDEM_SERVICO_AGENDA> GetAgendaByOs(ORDEM_SERVICO item);
    }
}
