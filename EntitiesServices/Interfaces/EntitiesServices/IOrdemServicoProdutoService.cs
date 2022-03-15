using EntitiesServices.Model;
using ModelServices.Interfaces.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Services
{
    public interface IOrdemServicoProdutoService : IServiceBase<ORDEM_SERVICO_PRODUTO>
    {
        Int32 Create(ORDEM_SERVICO_PRODUTO item);
        Int32 Edit(ORDEM_SERVICO_PRODUTO item, ORDEM_SERVICO_PRODUTO itemAntes);
        ORDEM_SERVICO_PRODUTO CheckExist(ORDEM_SERVICO_PRODUTO item);
        List<ORDEM_SERVICO_PRODUTO> GetAllByOs(Int32 id);
    }
}
