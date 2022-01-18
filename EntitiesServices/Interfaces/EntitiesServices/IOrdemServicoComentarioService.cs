using EntitiesServices.Model;
using ModelServices.Interfaces.EntitiesServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntitiesServices.Interfaces.Services
{
    public interface IOrdemServicoComentarioService : IServiceBase<ORDEM_SERVICO_COMENTARIOS>
    {
        Int32 Create(ORDEM_SERVICO_COMENTARIOS item);

        List<ORDEM_SERVICO_COMENTARIOS> GetByOs(ORDEM_SERVICO item);
    }
}
