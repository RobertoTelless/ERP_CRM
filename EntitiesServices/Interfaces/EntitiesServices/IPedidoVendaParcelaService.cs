using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPedidoVendaParcelaService : IServiceBase<PEDIDO_VENDA_PARCELA>
    {
        Int32 Create(PEDIDO_VENDA_PARCELA item);
    }
}
