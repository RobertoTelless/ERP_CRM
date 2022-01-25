using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPedidoVendaParcelaAppService : IAppServiceBase<PEDIDO_VENDA_PARCELA>
    {
        Int32 ValidateCreate(PEDIDO_VENDA_PARCELA item, USUARIO usuario);
    }

}
