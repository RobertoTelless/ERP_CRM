using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaReceberRateioAppService : IAppServiceBase<CONTA_RECEBER_RATEIO>
    {
        Int32 ValidateCreate(CONTA_RECEBER_RATEIO item, USUARIO usuario);
        Int32 ValidateEdit(CONTA_RECEBER_RATEIO item);
        Int32 ValidateDelete(CONTA_RECEBER_RATEIO item);
        Int32 ValidateReativar(CONTA_RECEBER_RATEIO item, USUARIO usuario);

        CONTA_RECEBER_RATEIO CheckExist(CONTA_RECEBER_RATEIO item);
        CONTA_RECEBER_RATEIO GetItemById(Int32 id);
        List<CONTA_RECEBER_RATEIO> GetAllItens();
    }
}
