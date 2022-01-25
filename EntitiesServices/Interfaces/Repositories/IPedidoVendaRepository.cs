using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPedidoVendaRepository : IRepositoryBase<PEDIDO_VENDA>
    {
        PEDIDO_VENDA CheckExist(PEDIDO_VENDA item, Int32 idAss);
        PEDIDO_VENDA GetByNome(String nome, Int32 idAss);
        PEDIDO_VENDA GetItemById(Int32 id);
        List<PEDIDO_VENDA> GetByUser(Int32 id);
        List<PEDIDO_VENDA> GetAllItens(Int32 idAss);
        List<PEDIDO_VENDA> GetAllItensAdm(Int32 idAss);
        List<PEDIDO_VENDA> GetAllItensAdmUser(Int32 id, Int32 idAss);
        List<PEDIDO_VENDA> GetAtrasados(Int32 idAss);
        List<PEDIDO_VENDA> GetEncerrados(Int32 idAss);
        List<PEDIDO_VENDA> GetCancelados(Int32 idAss);
        List<PEDIDO_VENDA> ExecuteFilter(Int32? usuaId, String nome, String numero, DateTime? data, Int32? status, Int32 idAss);
    }
}
