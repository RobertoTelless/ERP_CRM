using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICRMAppService : IAppServiceBase<CRM>
    {
        Int32 ValidateCreate(CRM item, USUARIO usuario);
        Int32 ValidateEdit(CRM item, CRM itemAntes, USUARIO usuario);
        Int32 ValidateEditSimples(CRM item, CRM itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CRM item, CRM itemAntes);
        Int32 ValidateDelete(CRM item, USUARIO usuario);
        Int32 ValidateReativar(CRM item, USUARIO usuario);

        CRM CheckExist(CRM item, Int32 idUsu, Int32 idAss);
        List<CRM> GetByDate(DateTime data, Int32 idAss);
        List<CRM> GetByUser(Int32 user);
        List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM GetItemById(Int32 id);
        List<CRM> GetAllItens(Int32 idAss);
        List<CRM> GetAllItensAdm(Int32 idAss);
        List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss);
        List<CRM_ORIGEM> GetAllOrigens(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss);
        CRM_COMENTARIO GetComentarioById(Int32 id);

        List<CRM_ACAO> GetAllAcoes(Int32 idAss);
        List<TIPO_CRM> GetAllTipos();
        USUARIO GetUserById(Int32 id);
        CRM_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32 idAss, out List<CRM> objeto);

        CRM_CONTATO GetContatoById(Int32 id);
        CRM_ACAO GetAcaoById(Int32 id);
        Int32 ValidateEditContato(CRM_CONTATO item);
        Int32 ValidateCreateContato(CRM_CONTATO item);
        Int32 ValidateEditAcao(CRM_ACAO item);
        Int32 ValidateCreateAcao(CRM_ACAO item, USUARIO usuario);

        CRM_PROPOSTA_ACOMPANHAMENTO GetPropostaComentarioById(Int32 id);
        List<CRM_PROPOSTA> GetAllPropostas(Int32 idAss);
        CRM_PROPOSTA GetPropostaById(Int32 id);
        Int32 ValidateEditProposta(CRM_PROPOSTA item);
        Int32 ValidateCreateProposta(CRM_PROPOSTA item);
        List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 idAss);
        Int32 ValidateCancelarProposta(CRM_PROPOSTA item);
        Int32 ValidateReprovarProposta(CRM_PROPOSTA item);
        Int32 ValidateAprovarProposta(CRM_PROPOSTA item);
        Int32 ValidateEnviarProposta(CRM_PROPOSTA item);
        CRM_PROPOSTA_ANEXO GetAnexoPropostaById(Int32 id);
        TEMPLATE_PROPOSTA GetTemplateById(Int32 id);

        CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id);
        List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss);
        CRM_PEDIDO_VENDA GetPedidoById(Int32 id);
        Int32 ValidateEditPedido(CRM_PEDIDO_VENDA item);
        Int32 ValidateCreatePedido(CRM_PEDIDO_VENDA item);
        CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id);
        List<FILIAL> GetAllFilial(Int32 idAss);
        CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss);
        List<FORMA_ENVIO> GetAllFormasEnvio(Int32 idAss);
        List<FORMA_FRETE> GetAllFormasFrete(Int32 idAss);

        CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id);
        Int32 ValidateEditItemPedido(CRM_PEDIDO_VENDA_ITEM item);
        Int32 ValidateCreateItemPedido(CRM_PEDIDO_VENDA_ITEM item);

    }
}
