using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPedidoVendaAnexoRepository : IRepositoryBase<PEDIDO_VENDA_ANEXO>
    {
        List<PEDIDO_VENDA_ANEXO> GetAllItens();
        PEDIDO_VENDA_ANEXO GetItemById(Int32 id);

    }
}
