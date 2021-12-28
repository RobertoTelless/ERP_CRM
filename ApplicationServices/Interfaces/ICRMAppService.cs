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
        List<TIPO_ACAO> GetAllTipoAcao();
        List<CRM_ORIGEM> GetAllOrigens();
        List<MOTIVO_CANCELAMENTO> GetAllMotivoCancelamento();
        List<MOTIVO_ENCERRAMENTO> GetAllMotivoEncerramento();
        CRM_COMENTARIO GetComentarioById(Int32 id);

        List<CRM_ACAO> GetAllAcoes(Int32 idAss);
        List<TIPO_CRM> GetAllTipos();
        USUARIO GetUserById(Int32 id);
        CRM_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? status, DateTime? inicio, DateTime? final, Int32? origem, Int32? adic, String nome, String busca, Int32? estrela, Int32 idAss, out List<CRM> objeto);

        CRM_CONTATO GetContatoById(Int32 id);
        CRM_ACAO GetAcaoById(Int32 id);
        Int32 ValidateEditContato(CRM_CONTATO item);
        Int32 ValidateCreateContato(CRM_CONTATO item);
        Int32 ValidateEditAcao(CRM_ACAO item);
        Int32 ValidateCreateAcao(CRM_ACAO item, USUARIO usuario);

    }
}
