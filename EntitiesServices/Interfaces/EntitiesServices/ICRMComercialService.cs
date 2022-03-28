using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICRMComercialService : IServiceBase<CRM_COMERCIAL>
    {
        Int32 Create(CRM_COMERCIAL tarefa, LOG log);
        Int32 Create(CRM_COMERCIAL tarefa);
        Int32 Edit(CRM_COMERCIAL tarefa, LOG log);
        Int32 Edit(CRM_COMERCIAL tarefa);
        Int32 Delete(CRM_COMERCIAL tarefa, LOG log);

        CRM_COMERCIAL CheckExist(CRM_COMERCIAL item, Int32 idUsu, Int32 idAss);
        List<CRM_COMERCIAL> GetByDate(DateTime data, Int32 idAss);
        List<CRM_COMERCIAL> GetByUser(Int32 user);
        List<CRM_COMERCIAL> GetTarefaStatus(Int32 tipo, Int32 idAss);
        CRM_COMERCIAL GetItemById(Int32 id);
        List<CRM_COMERCIAL> GetAllItens(Int32 idAss);
        List<CRM_COMERCIAL> GetAllItensAdm(Int32 idAss);
        List<CRM_COMERCIAL> GetAllItensAdmUser(Int32 id, Int32 idAss);
        List<CRM_COMERCIAL> ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32 idAss);
        List<CRM_COMERCIAL> ExecuteFilterDash(String nmr, DateTime? dtFinal, String nome, Int32? usu, Int32? status, Int32 idAss);

        List<CRM_COMERCIAL> GetAtrasados(Int32 idAss);
        List<CRM_COMERCIAL> GetCancelados(Int32 idAss);
        List<CRM_COMERCIAL> GetEncerrados(Int32 idAss);

        List<USUARIO> GetAllUsers(Int32 idAss);
        List<TIPO_CRM> GetAllTipos();
        List<TIPO_ACAO> GetAllTipoAcao(Int32 idAss);
        List<CRM_ORIGEM> GetAllOrigens(Int32 idAss);
        List<FILIAL> GetAllFilial(Int32 idAss);
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento(Int32 idAss);
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento(Int32 idAss);
        CRM_COMERCIAL_ANEXO GetAnexoById(Int32 id);
        USUARIO GetUserById(Int32 id);
        CRM_COMERCIAL_COMENTARIO_NOVA GetComentarioById(Int32 id);

        List<CRM_COMERCIAL_ACAO> GetAllAcoes(Int32 idAss);
        CRM_COMERCIAL_CONTATO GetContatoById(Int32 id);
        CRM_COMERCIAL_ACAO GetAcaoById(Int32 id);

        Int32 EditContato(CRM_COMERCIAL_CONTATO item);
        Int32 CreateContato(CRM_COMERCIAL_CONTATO item);
        Int32 EditAcao(CRM_COMERCIAL_ACAO item);
        Int32 CreateAcao(CRM_COMERCIAL_ACAO item);

        Int32 CreateAcompanhamento(CRM_COMERCIAL_COMENTARIO_NOVA item);

        CRM_COMERCIAL_ITEM GetItemCRMById(Int32 id);
        Int32 EditItemCRM(CRM_COMERCIAL_ITEM item);
        Int32 CreateItemCRM(CRM_COMERCIAL_ITEM item);

    }
}
