using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.Interfaces
{
    public interface IOrdemServicoAgendaAppService : IAppServiceBase<ORDEM_SERVICO_AGENDA>
    {
        Int32 ValidateCreate(ORDEM_SERVICO_AGENDA item);
        List<ORDEM_SERVICO_AGENDA> GetAgendaByOs(ORDEM_SERVICO item);
    }
}
