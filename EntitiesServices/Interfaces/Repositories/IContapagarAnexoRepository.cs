using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaPagarAnexoRepository : IRepositoryBase<CONTA_PAGAR_ANEXO>
    {
        List<CONTA_PAGAR_ANEXO> GetAllItens();
        CONTA_PAGAR_ANEXO GetItemById(Int32 id);
    }
}
