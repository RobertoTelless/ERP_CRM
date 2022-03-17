using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelServices.Interfaces.Repositories
{
    public interface IOrdemServicoAcompanhamentoRepository : IRepositoryBase<ORDEM_SERVICO_ACOMPANHAMENTO>
    {
        List<ORDEM_SERVICO_ACOMPANHAMENTO> GetByOs(ORDEM_SERVICO conta);
    }
}
