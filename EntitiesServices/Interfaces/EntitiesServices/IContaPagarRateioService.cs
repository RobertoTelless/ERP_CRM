using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaPagarRateioService : IServiceBase<CONTA_PAGAR_RATEIO>
    {
        CONTA_PAGAR_RATEIO CheckExist(CONTA_PAGAR_RATEIO item);
        Int32 Create(CONTA_PAGAR_RATEIO item, LOG log);
        Int32 Create(CONTA_PAGAR_RATEIO item);
        Int32 Edit(CONTA_PAGAR_RATEIO item, LOG log);
        Int32 Edit(CONTA_PAGAR_RATEIO item);
        Int32 Delete(CONTA_PAGAR_RATEIO item);

        CONTA_PAGAR_RATEIO GetItemById(Int32 id);
        List<CONTA_PAGAR_RATEIO> GetAllItens();
    }

}
