using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IServicoAnexoRepository : IRepositoryBase<SERVICO_ANEXO>
    {
        List<SERVICO_ANEXO> GetAllItens();
        SERVICO_ANEXO GetItemById(Int32 id);
    }
}
