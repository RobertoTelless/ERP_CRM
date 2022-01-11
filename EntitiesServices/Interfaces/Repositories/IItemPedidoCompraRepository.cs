using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IItemPedidoCompraRepository : IRepositoryBase<ITEM_PEDIDO_COMPRA>
    {
        List<ITEM_PEDIDO_COMPRA> GetAllItens();
        ITEM_PEDIDO_COMPRA GetItemById(Int32 id);
        ITEM_PEDIDO_COMPRA GetItemByProduto(Int32 id);
    }
}
