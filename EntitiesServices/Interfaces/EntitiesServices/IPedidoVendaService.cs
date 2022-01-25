using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IPedidoVendaService : IServiceBase<PEDIDO_VENDA>
    {
        Int32 Create(PEDIDO_VENDA perfil, LOG log);
        Int32 Create(PEDIDO_VENDA perfil);
        Int32 Edit(PEDIDO_VENDA perfil, LOG log);
        Int32 Edit(PEDIDO_VENDA perfil);
        Int32 Delete(PEDIDO_VENDA perfil, LOG log);

        PEDIDO_VENDA CheckExist(PEDIDO_VENDA conta, Int32 idAss);
        PEDIDO_VENDA GetItemById(Int32 id);
        PEDIDO_VENDA GetByNome(String nome, Int32 idAss);
        List<PEDIDO_VENDA> GetByUser(Int32 id);
        List<PEDIDO_VENDA> GetAllItens(Int32 idAss);
        List<PEDIDO_VENDA> GetAllItensAdm(Int32 idAss);
        List<PEDIDO_VENDA> GetAllItensAdmUser(Int32 id, Int32 idAss);
        List<PEDIDO_VENDA> GetAtrasados(Int32 idAss);
        List<PEDIDO_VENDA> GetCancelados(Int32 idAss);
        List<PEDIDO_VENDA> GetEncerrados(Int32 idAss);

        List<FORMA_PAGAMENTO> GetAllFormas(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<FILIAL> GetAllFilial(Int32 idAss);
        List<FORMA_ENVIO> GetAllFormaEnvio(Int32 idAss);
        List<FORMA_FRETE> GetAllFormaFrete(Int32 idAss);
        PEDIDO_VENDA_ANEXO GetAnexoById(Int32 id);
        List<PEDIDO_VENDA> ExecuteFilter(Int32? usuaId, String nome, String numero, DateTime? data, Int32? status, Int32 idAss);
        
        ITEM_PEDIDO_VENDA GetItemVendaById(Int32 id);
        Int32 EditItemVenda(ITEM_PEDIDO_VENDA item);
        Int32 CreateItemVenda(ITEM_PEDIDO_VENDA item);

        Int32 CreateResumoVenda(RESUMO_VENDA item);
        Int32 DeleteResumoVenda(RESUMO_VENDA item);
        List<RESUMO_VENDA> GetResumos(Int32 idAss);
    }
}
