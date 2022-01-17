using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaReceberRateioService : IServiceBase<CONTA_RECEBER_RATEIO>
    {
        CONTA_RECEBER_RATEIO CheckExist(CONTA_RECEBER_RATEIO item);
        Int32 Create(CONTA_RECEBER_RATEIO item, LOG log);
        Int32 Create(CONTA_RECEBER_RATEIO item);
        Int32 Edit(CONTA_RECEBER_RATEIO item, LOG log);
        Int32 Edit(CONTA_RECEBER_RATEIO item);
        Int32 Delete(CONTA_RECEBER_RATEIO item);

        CONTA_RECEBER_RATEIO GetItemById(Int32 id);
        List<CONTA_RECEBER_RATEIO> GetAllItens();
    }
}
