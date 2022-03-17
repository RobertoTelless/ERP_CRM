using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelServices.Interfaces.Repositories
{
    public interface IOrdemServicoAgendaRepository : IRepositoryBase<ORDEM_SERVICO_AGENDA>
    {
        List<ORDEM_SERVICO_AGENDA> GetAgendaByOs(ORDEM_SERVICO item);
    }
}
