using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaPagarTagAppService : IAppServiceBase<CONTA_PAGAR_TAG>
    {
        Int32 ValidateCreate(CONTA_PAGAR_TAG item, USUARIO usuario);
        Int32 ValidateEdit(CONTA_PAGAR_TAG item, CONTA_PAGAR_TAG itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONTA_PAGAR_TAG item, USUARIO usuario);
        Int32 ValidateReativar(CONTA_PAGAR_TAG item, USUARIO usuario);

        CONTA_PAGAR_TAG GetItemById(Int32 id);
        List<CONTA_PAGAR_TAG> GetAllItens();
    }

}
