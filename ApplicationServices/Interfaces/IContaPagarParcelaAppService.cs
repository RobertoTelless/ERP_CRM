using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IContaPagarParcelaAppService : IAppServiceBase<CONTA_PAGAR_PARCELA>
    {
        Int32 ValidateCreate(CONTA_PAGAR_PARCELA item, USUARIO usuario);
        Int32 ValidateEdit(CONTA_PAGAR_PARCELA item, CONTA_PAGAR_PARCELA itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CONTA_PAGAR_PARCELA item, USUARIO usuario);
        Int32 ValidateReativar(CONTA_PAGAR_PARCELA item, USUARIO usuario);

        CONTA_PAGAR_PARCELA GetItemById(Int32 id);
        List<CONTA_PAGAR_PARCELA> GetAllItens(Int32 idAss);
    }
}
