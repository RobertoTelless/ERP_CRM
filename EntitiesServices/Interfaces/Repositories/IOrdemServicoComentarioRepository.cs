using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelServices.Interfaces.Repositories
{
    public interface IOrdemServicoComentarioRepository : IRepositoryBase<ORDEM_SERVICO_COMENTARIOS>
    {
        List<ORDEM_SERVICO_COMENTARIOS> GetByOs(ORDEM_SERVICO item);
    }
}
