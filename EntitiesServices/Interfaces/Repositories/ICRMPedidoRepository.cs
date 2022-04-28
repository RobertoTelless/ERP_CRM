using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMPedidoRepository : IRepositoryBase<CRM_PEDIDO_VENDA>
    {
        List<CRM_PEDIDO_VENDA> GetAllItens(Int32 idAss);
        CRM_PEDIDO_VENDA GetItemById(Int32 id);
        CRM_PEDIDO_VENDA GetItemByNumero(String num, Int32 idAss);
    }
}
