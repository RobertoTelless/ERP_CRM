using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.Interfaces
{
    public interface IOrdemServicoProdutoAppService : IAppServiceBase<ORDEM_SERVICO_PRODUTO>
    {
        Int32 ValidateCreate(ORDEM_SERVICO_PRODUTO item);
        Int32 ValidateDelete(ORDEM_SERVICO_PRODUTO item, ORDEM_SERVICO_PRODUTO itemAntes);
        Int32 ValidateReativar(ORDEM_SERVICO_PRODUTO item, ORDEM_SERVICO_PRODUTO itemAntes);

        ORDEM_SERVICO_PRODUTO CheckExist(ORDEM_SERVICO_PRODUTO item);
        List<ORDEM_SERVICO_PRODUTO> GetAllByOs(Int32 id);
    }
}
