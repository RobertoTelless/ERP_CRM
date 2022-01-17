using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaReceberTagAppService : IAppServiceBase<CONTA_RECEBER_TAG>
    {
        Int32 ValidateCreate(CONTA_RECEBER_TAG item, USUARIO usuario);
        Int32 ValidateEdit(CONTA_RECEBER_TAG item, CONTA_RECEBER_TAG itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONTA_RECEBER_TAG item, USUARIO usuario);
        Int32 ValidateReativar(CONTA_RECEBER_TAG item, USUARIO usuario);

        CONTA_RECEBER_TAG GetItemById(Int32 id);
        List<CONTA_RECEBER_TAG> GetAllItens();

    }
}
