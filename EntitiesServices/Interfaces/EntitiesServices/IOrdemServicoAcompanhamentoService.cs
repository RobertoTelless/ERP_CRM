using EntitiesServices.Model;
using ModelServices.Interfaces.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Services
{
    public interface IOrdemServicoAcompanhamentoService : IServiceBase<ORDEM_SERVICO_ACOMPANHAMENTO>
    {
        Int32 Create(ORDEM_SERVICO_ACOMPANHAMENTO item);

        List<ORDEM_SERVICO_ACOMPANHAMENTO> GetByOs(ORDEM_SERVICO item);
    }
}
