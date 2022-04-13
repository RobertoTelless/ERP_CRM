using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFormularioRespostaAppService : IAppServiceBase<FORMULARIO_RESPOSTA>
    {
        Int32 ValidateCreate(FORMULARIO_RESPOSTA perfil, Int32 ? tipo);
        Int32 ValidateEdit(FORMULARIO_RESPOSTA perfil, FORMULARIO_RESPOSTA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(FORMULARIO_RESPOSTA item, FORMULARIO_RESPOSTA itemAntes);
        Int32 ValidateDelete(FORMULARIO_RESPOSTA perfil, USUARIO usuario);
        Int32 ValidateReativar(FORMULARIO_RESPOSTA perfil, USUARIO usuario);

        List<FORMULARIO_RESPOSTA> GetAllItens();
        List<FORMULARIO_RESPOSTA> GetAllItensTodos();
        FORMULARIO_RESPOSTA GetItemById(Int32 id);
        Int32 ExecuteFilter(Int32? status, String nome, String email, String celular, String cidade, Int32? uf, out List<FORMULARIO_RESPOSTA> objeto);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);

        List<FORMULARIO_RESPOSTA_ACAO> GetAllAcoes();
        FORMULARIO_RESPOSTA_ANEXO GetAnexoById(Int32 id);
        FORMULARIO_RESPOSTA_COMENTARIO GetComentarioById(Int32 id);
        FORMULARIO_RESPOSTA_ACAO GetAcaoById(Int32 id);
        Int32 ValidateEditAcao(FORMULARIO_RESPOSTA_ACAO item);
        Int32 ValidateCreateAcao(FORMULARIO_RESPOSTA_ACAO item, USUARIO usuario);

    }
}
