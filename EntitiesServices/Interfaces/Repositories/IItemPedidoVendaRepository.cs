using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IItemPedidoVendaRepository : IRepositoryBase<ITEM_PEDIDO_VENDA>
    {
        List<ITEM_PEDIDO_VENDA> GetAllItens();
        ITEM_PEDIDO_VENDA GetItemById(Int32 id);

    }
}
