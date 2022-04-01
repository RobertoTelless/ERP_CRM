using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.WorkClasses;

namespace ApplicationServices.Interfaces
{
    public interface ICRMComercialAppService : IAppServiceBase<CRM_COMERCIAL>
    {
        Int32 ValidateCreate(CRM_COMERCIAL item, USUARIO usuario);
        Int32 ValidateEdit(CRM_COMERCIAL item, CRM_COMERCIAL itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CRM_COMERCIAL item, CRM_COMERCIAL itemAntes);
        Int32 ValidateDelete(CRM_COMERCIAL item, USUARIO usuario);
        Int32 ValidateReativar(CRM_COMERCIAL item, USUARIO usuario);

        CRM_COMERCIAL CheckExist(CRM_COMERCIAL item, Int32 idUsu, Int32 idAss);
        List<CRM_COMERCIAL> GetByDate(DateTime data, Int32 idAss);
        List<CRM_COMERCIAL> GetByUser(Int32 user);
        List<CRM_COMERCIAL> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM_COMERCIAL GetItemById(Int32 id);
        List<CRM_COMERCIAL> GetAllItens(Int32 idAss);
        List<CRM_COMERCIAL> GetAllItensAdm(Int32 idAss);
        List<CRM_COMERCIAL> GetAllItensAdmUser(Int32 id, Int32 idAss);

        List<CRM_COMERCIAL> GetAtrasados(Int32 idAss);
        List<CRM_COMERCIAL> GetCancelados(Int32 idAss);
        List<CRM_COMERCIAL> GetEncerrados(Int32 idAss);

        List<USUARIO> GetAllUsers(Int32 idAss);
        List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss);
        List<CRM_ORIGEM> GetAllOrigens(Int32 idAss);
        List<FILIAL> GetAllFilial(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss);
        CRM_COMERCIAL_COMENTARIO_NOVA GetComentarioById(Int32 id);

        List<CRM_COMERCIAL_ACAO> GetAllAcoes(Int32 idAss);
        List<TIPO_CRM> GetAllTipos();
        USUARIO GetUserById(Int32 id);
        CRM_COMERCIAL_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? status, DateTime? inicio, DateTime? prevista, String numero, String nota, Int32? estrela,  String nome, String busca, Int32 idAss, out List<CRM_COMERCIAL> objeto);
        Int32 ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss, out List<CRM_COMERCIAL> objeto);

        CRM_COMERCIAL_CONTATO GetContatoById(Int32 id);
        CRM_COMERCIAL_ACAO GetAcaoById(Int32 id);

        Int32 ValidateEditContato(CRM_COMERCIAL_CONTATO item);
        Int32 ValidateCreateContato(CRM_COMERCIAL_CONTATO item);
        Int32 ValidateEditAcao(CRM_COMERCIAL_ACAO item);
        Int32 ValidateCreateAcao(CRM_COMERCIAL_ACAO item, USUARIO usuario);

        Int32 ValidateCreateAcompanhamento(CRM_COMERCIAL_COMENTARIO_NOVA item);

        CRM_COMERCIAL_ITEM GetItemCRMById(Int32 id);
        Int32 ValidateEditItemCRM(CRM_COMERCIAL_ITEM item);
        Int32 ValidateDeleteItemCRM(CRM_COMERCIAL_ITEM item);
        Int32 ValidateReativarItemCRM(CRM_COMERCIAL_ITEM item);
        Int32 ValidateCreateItemCRM(CRM_COMERCIAL_ITEM item);

        Int32 ValidateEnvioAprovacao(CRM_COMERCIAL item, String emailPersonalizado, USUARIO usuario);
        Int32 ValidateEnvioAprovacao(CRM_COMERCIAL item, List<AttachmentForn> anexo, String emailPersonalizado, USUARIO usuario, List<CLIENTE> forn);
        String ValidateCreateMensagem(CLIENTE item, USUARIO usuario, CRM_COMERCIAL ped, Int32? idAss);
        Int32 ValidateEditItemCRMAprovacao(CRM_COMERCIAL_ITEM item);
        Int32 ValidateAprovacao(CRM_COMERCIAL item);
        Int32 ValidateReprovacao(CRM_COMERCIAL item);
        Int32 ValidateCancelamento(CRM_COMERCIAL item);
        //Int32 ValidateEnvioAprovacao(CRM_COMERCIAL item);
        Int32 ValidateEfetuarVenda(CRM_COMERCIAL item);
        Int32 ValidateEncerrar(CRM_COMERCIAL item, USUARIO usuario);
        Int32 ValidateItemEntregue(CRM_COMERCIAL_ITEM item, USUARIO usuario);
        Int32 ValidateEntreguePorItem(CRM_COMERCIAL item);

    }
}
