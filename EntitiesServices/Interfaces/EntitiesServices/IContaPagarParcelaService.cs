using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IContaPagarParcelaService : IServiceBase<CONTA_PAGAR_PARCELA>
    {
        Int32 Create(CONTA_PAGAR_PARCELA item, LOG log);
        Int32 Create(CONTA_PAGAR_PARCELA item);
        Int32 Edit(CONTA_PAGAR_PARCELA item, LOG log);
        Int32 Edit(CONTA_PAGAR_PARCELA item);
        Int32 Delete(CONTA_PAGAR_PARCELA item, LOG log);

        CONTA_PAGAR_PARCELA GetItemById(Int32 id);
        List<CONTA_PAGAR_PARCELA> GetAllItens(Int32 idAss);
    }
}
