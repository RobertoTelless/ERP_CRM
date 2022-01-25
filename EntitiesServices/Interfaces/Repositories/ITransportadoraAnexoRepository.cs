using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITransportadoraAnexoRepository : IRepositoryBase<TRANSPORTADORA_ANEXO>
    {
        List<TRANSPORTADORA_ANEXO> GetAllItens();
        TRANSPORTADORA_ANEXO GetItemById(Int32 id);
    }
}
