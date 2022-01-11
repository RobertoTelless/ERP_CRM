using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPedidoCompraAcompanhamentoRepository : IRepositoryBase<PEDIDO_COMPRA_ACOMPANHAMENTO>
    {
        PEDIDO_COMPRA_ACOMPANHAMENTO CheckExist(PEDIDO_COMPRA_ACOMPANHAMENTO item, Int32 idAss);
    }
}
