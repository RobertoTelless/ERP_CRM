using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IPedidoCompraRepository : IRepositoryBase<PEDIDO_COMPRA>
    {
        PEDIDO_COMPRA CheckExist(PEDIDO_COMPRA item, Int32 idAss);
        PEDIDO_COMPRA GetByNome(String nome, Int32 idAss);
        PEDIDO_COMPRA GetItemById(Int32 id);
        List<PEDIDO_COMPRA> GetByUser(Int32 id);
        List<PEDIDO_COMPRA> GetAllItens(Int32 idAss);
        List<PEDIDO_COMPRA> GetAllItensAdm(Int32 idAss);
        List<PEDIDO_COMPRA> GetAllItensAdmUser(Int32 id, Int32 idAss);
        List<PEDIDO_COMPRA> GetAtrasados(Int32 idAss);
        List<PEDIDO_COMPRA> GetEncerrados(Int32 idAss);
        List<PEDIDO_COMPRA> GetCancelados(Int32 idAss);
        List<PEDIDO_COMPRA> ExecuteFilter(Int32? usuaId, String nome, String numero, String nf, DateTime? data, DateTime? dataPrevista, Int32? status, Int32 idAss);
        List<PEDIDO_COMPRA> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss);
    }
}
