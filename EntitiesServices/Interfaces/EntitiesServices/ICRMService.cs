using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICRMService : IServiceBase<CRM>
    {
        Int32 Create(CRM tarefa, LOG log);
        Int32 Create(CRM tarefa);
        Int32 Edit(CRM tarefa, LOG log);
        Int32 Edit(CRM tarefa);
        Int32 Delete(CRM tarefa, LOG log);

        CRM CheckExist(CRM item, Int32 idUsu, Int32 idAss);
        List<CRM> GetByDate(DateTime data, Int32 idAss);
        List<CRM> GetByUser(Int32 user);
        List<CRM> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM GetItemById(Int32 id);
        List<CRM> GetAllItens(Int32 idAss);
        List<CRM> GetAllItensAdm(Int32 idAss);
        List<CRM> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32? temperatura, Int32 idAss);


        List<USUARIO> GetAllUsers(Int32 idAss);
        List<TIPO_CRM> GetAllTipos();
        List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss);
        List<CRM_ORIGEM> GetAllOrigens(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss);
        CRM_ANEXO GetAnexoById(Int32 id);
        USUARIO GetUserById(Int32 id);
        CRM_COMENTARIO GetComentarioById(Int32 id);

        List<CRM_ACAO> GetAllAcoes(Int32 idAss);
        CRM_CONTATO GetContatoById(Int32 id);
        CRM_ACAO GetAcaoById(Int32 id);
        Int32 EditContato(CRM_CONTATO item);
        Int32 CreateContato(CRM_CONTATO item);
        Int32 EditAcao(CRM_ACAO item);
        Int32 CreateAcao(CRM_ACAO item);

        CRM_PROPOSTA_ACOMPANHAMENTO GetPropostaComentarioById(Int32 id);
        List<CRM_PROPOSTA> GetAllPropostas(Int32 idAss);
        CRM_PROPOSTA GetPropostaById(Int32 id);
        Int32 EditProposta(CRM_PROPOSTA item);
        Int32 CreateProposta(CRM_PROPOSTA item);
        List<TEMPLATE_PROPOSTA> GetAllTemplateProposta(Int32 idAss);
        CRM_PROPOSTA_ANEXO GetAnexoPropostaById(Int32 id);
        TEMPLATE_PROPOSTA GetTemplateById(Int32 id);

        CRM_PEDIDO_VENDA_ACOMPANHAMENTO GetPedidoComentarioById(Int32 id);
        List<CRM_PEDIDO_VENDA> GetAllPedidos(Int32 idAss);
        CRM_PEDIDO_VENDA GetPedidoById(Int32 id);
        Int32 EditPedido(CRM_PEDIDO_VENDA item);
        Int32 CreatePedido(CRM_PEDIDO_VENDA item);
        CRM_PEDIDO_VENDA_ANEXO GetAnexoPedidoById(Int32 id);
        List<FILIAL> GetAllFilial(Int32 idAss);
        CRM_PEDIDO_VENDA GetPedidoByNumero(String num, Int32 idAss);
        List<FORMA_ENVIO> GetAllFormasEnvio(Int32 idAss);
        List<FORMA_FRETE> GetAllFormasFrete(Int32 idAss);

        CRM_PEDIDO_VENDA_ITEM GetItemPedidoById(Int32 id);
        Int32 EditItemPedido(CRM_PEDIDO_VENDA_ITEM item);
        Int32 CreateItemPedido(CRM_PEDIDO_VENDA_ITEM item);

    }
}
