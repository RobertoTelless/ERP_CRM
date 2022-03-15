using EntitiesServices.Model;
using ModelServices.Interfaces.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Services
{
    public interface IOrdemServicoServicoService : IServiceBase<ORDEM_SERVICO_SERVICO>
    {
        Int32 Create(ORDEM_SERVICO_SERVICO item);
        Int32 Edit(ORDEM_SERVICO_SERVICO item, ORDEM_SERVICO_SERVICO itemAntes);
        ORDEM_SERVICO_SERVICO CheckExist(ORDEM_SERVICO_SERVICO item);
        List<ORDEM_SERVICO_SERVICO> GetAllByOs(Int32 id);
    }
}
