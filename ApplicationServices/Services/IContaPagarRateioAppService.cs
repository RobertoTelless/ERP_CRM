using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaPagarRateioAppService : IAppServiceBase<CONTA_PAGAR_RATEIO>
    {
        Int32 ValidateCreate(CONTA_PAGAR_RATEIO item, USUARIO usuario);
        Int32 ValidateEdit(CONTA_PAGAR_RATEIO item);
        Int32 ValidateDelete(CONTA_PAGAR_RATEIO item);
        Int32 ValidateReativar(CONTA_PAGAR_RATEIO item, USUARIO usuario);

        CONTA_PAGAR_RATEIO CheckExist(CONTA_PAGAR_RATEIO item);
        CONTA_PAGAR_RATEIO GetItemById(Int32 id);
        List<CONTA_PAGAR_RATEIO> GetAllItens();
    }

}
