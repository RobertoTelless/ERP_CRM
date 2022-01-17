using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaReceberTagService : IServiceBase<CONTA_RECEBER_TAG>
    {
        Int32 Create(CONTA_RECEBER_TAG item, LOG log);
        Int32 Create(CONTA_RECEBER_TAG item);
        Int32 Edit(CONTA_RECEBER_TAG item, LOG log);
        Int32 Edit(CONTA_RECEBER_TAG item);
        Int32 Delete(CONTA_RECEBER_TAG item, LOG log);

        CONTA_RECEBER_TAG GetItemById(Int32 id);
        List<CONTA_RECEBER_TAG> GetAllItens();

    }
}
