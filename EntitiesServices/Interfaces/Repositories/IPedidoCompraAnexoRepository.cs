using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPedidoCompraAnexoRepository : IRepositoryBase<PEDIDO_COMPRA_ANEXO>
    {
        List<PEDIDO_COMPRA_ANEXO> GetAllItens();
        PEDIDO_COMPRA_ANEXO GetItemById(Int32 id);
    }
}
