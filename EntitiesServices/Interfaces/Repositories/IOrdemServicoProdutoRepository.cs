using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Repositories
{
    public interface IOrdemServicoProdutoRepository : IRepositoryBase<ORDEM_SERVICO_PRODUTO>
    {
        ORDEM_SERVICO_PRODUTO CheckExist(ORDEM_SERVICO_PRODUTO item);
        List<ORDEM_SERVICO_PRODUTO> GetAllByOs(Int32 id);
    }
}
