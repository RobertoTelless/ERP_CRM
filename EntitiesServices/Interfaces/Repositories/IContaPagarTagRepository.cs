using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaPagarTagRepository : IRepositoryBase<CONTA_PAGAR_TAG>
    {
        List<CONTA_PAGAR_TAG> GetAllItens();
        CONTA_PAGAR_TAG GetItemById(Int32 id);
    }

}
