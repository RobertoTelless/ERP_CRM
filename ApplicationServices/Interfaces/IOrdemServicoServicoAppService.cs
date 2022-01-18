using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.Interfaces
{
    public interface IOrdemServicoServicoAppService : IAppServiceBase<ORDEM_SERVICO_SERVICO>
    {
        Int32 ValidateCreate(ORDEM_SERVICO_SERVICO item);
        Int32 ValidateDelete(ORDEM_SERVICO_SERVICO item, ORDEM_SERVICO_SERVICO itemAntes);
        Int32 ValidateReativar(ORDEM_SERVICO_SERVICO item, ORDEM_SERVICO_SERVICO itemAntes);

        ORDEM_SERVICO_SERVICO CheckExist(ORDEM_SERVICO_SERVICO item);
        List<ORDEM_SERVICO_SERVICO> GetAllByOs(Int32 id);
    }
}
