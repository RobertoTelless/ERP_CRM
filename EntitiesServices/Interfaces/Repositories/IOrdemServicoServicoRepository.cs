using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Repositories
{
    public interface IOrdemServicoServicoRepository : IRepositoryBase<ORDEM_SERVICO_SERVICO>
    {
        ORDEM_SERVICO_SERVICO CheckExist(ORDEM_SERVICO_SERVICO item);
        List<ORDEM_SERVICO_SERVICO> GetAllbyOs(Int32 id);
    }
}
