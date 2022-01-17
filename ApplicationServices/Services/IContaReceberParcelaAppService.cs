using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaReceberParcelaAppService : IAppServiceBase<CONTA_RECEBER_PARCELA>
    {
        Int32 ValidateCreate(CONTA_RECEBER_PARCELA item, USUARIO usuario);
        Int32 ValidateEdit(CONTA_RECEBER_PARCELA item, CONTA_RECEBER_PARCELA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONTA_RECEBER_PARCELA item, USUARIO usuario);
        Int32 ValidateReativar(CONTA_RECEBER_PARCELA item, USUARIO usuario);

        CONTA_RECEBER_PARCELA GetItemById(Int32 id);
        List<CONTA_RECEBER_PARCELA> GetAllItens();

    }
}
