using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaPagarTagService : IServiceBase<CONTA_PAGAR_TAG>
    {
        Int32 Create(CONTA_PAGAR_TAG item, LOG log);
        Int32 Create(CONTA_PAGAR_TAG item);
        Int32 Edit(CONTA_PAGAR_TAG item, LOG log);
        Int32 Edit(CONTA_PAGAR_TAG item);
        Int32 Delete(CONTA_PAGAR_TAG item, LOG log);

        CONTA_PAGAR_TAG GetItemById(Int32 id);
        List<CONTA_PAGAR_TAG> GetAllItens();
    }

}
