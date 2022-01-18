using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Repositories
{
    public interface IOrdemServicoAnexoRepository : IRepositoryBase<ORDEM_SERVICO_ANEXO>
    {
        List<ORDEM_SERVICO_ANEXO> GetAllItens();
        ORDEM_SERVICO_ANEXO GetItemById(Int32 id);
    }
}
