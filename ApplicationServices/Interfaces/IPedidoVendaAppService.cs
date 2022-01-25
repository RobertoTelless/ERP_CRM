using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IPedidoVendaAppService : IAppServiceBase<PEDIDO_VENDA>
    {
        Int32 ValidateCreate(PEDIDO_VENDA perfil, USUARIO usuario);
        Int32 ValidateEdit(PEDIDO_VENDA perfil, PEDIDO_VENDA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(PEDIDO_VENDA item, PEDIDO_VENDA itemAntes);
        Int32 ValidateDelete(PEDIDO_VENDA perfil, USUARIO usuario);
        Int32 ValidateReativar(PEDIDO_VENDA perfil, USUARIO usuario);

        List<PEDIDO_VENDA> GetAllItens(Int32 idAss);
        List<PEDIDO_VENDA> GetAllItensAdm(Int32 idAss);
        List<PEDIDO_VENDA> GetAllItensAdmUser(Int32 id, Int32 idAss);
        PEDIDO_VENDA GetItemById(Int32 id);
        PEDIDO_VENDA GetByNome(String nome, Int32 idAss);
        List<PEDIDO_VENDA> GetByUser(Int32 id);
        PEDIDO_VENDA CheckExist(PEDIDO_VENDA conta, Int32 idAss);

        List<FORMA_PAGAMENTO> GetAllFormas(Int32 idAss);
        List<UNIDADE> GetAllUnidades(Int32 idAss);
        List<FILIAL> GetAllFilial(Int32 idAss);
        List<FORMA_ENVIO> GetAllFormaEnvio(Int32 idAss);
        List<FORMA_FRETE> GetAllFormaFrete(Int32 idAss);
        PEDIDO_VENDA_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? usuaId, String nome, String numero, DateTime? data, Int32? status, Int32 idAss, out List<PEDIDO_VENDA> objeto);
        
        List<PEDIDO_VENDA> GetEncerrados(Int32 idAss);
        List<PEDIDO_VENDA> GetAtrasados(Int32 idAss);
        List<PEDIDO_VENDA> GetCancelados(Int32 idAss);

        ITEM_PEDIDO_VENDA GetItemVendaById(Int32 id);
        Int32 ValidateEditItemVenda(ITEM_PEDIDO_VENDA item);
        Int32 ValidateCreateItemVenda(ITEM_PEDIDO_VENDA item);
        Int32 ValidateAprovacao(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateReprovacao(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateProcessamento(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateFaturamento(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateExpedicao(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateAprovacaoOportunidade(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateAprovacaoProposta(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateCancelamento(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateCancelamentoOportunidade(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateCancelamentoProposta(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateEnvioAprovacao(PEDIDO_VENDA item, USUARIO usuario);
        Int32 ValidateEncerramento(PEDIDO_VENDA item, USUARIO usuario);

        Int32 CreateResumoVenda(RESUMO_VENDA item, USUARIO usuario);
        Int32 DeleteResumoVenda(RESUMO_VENDA item);
        List<RESUMO_VENDA> GetResumos(USUARIO usuario);
    }
}
