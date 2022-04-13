using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFormularioRespostaService : IServiceBase<FORMULARIO_RESPOSTA>
    {
        Int32 Create(FORMULARIO_RESPOSTA perfil, LOG log);
        Int32 Create(FORMULARIO_RESPOSTA perfil);
        Int32 Edit(FORMULARIO_RESPOSTA perfil, LOG log);
        Int32 Edit(FORMULARIO_RESPOSTA perfil);
        Int32 Delete(FORMULARIO_RESPOSTA perfil, LOG log);

        FORMULARIO_RESPOSTA GetItemById(Int32 id);
        List<FORMULARIO_RESPOSTA> GetAllItens();
        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
        List<FORMULARIO_RESPOSTA> GetAllItensTodos();

        List<FORMULARIO_RESPOSTA> ExecuteFilter(Int32? status, String nome, String email, String celular, String cidade, Int32? uf);

        List<FORMULARIO_RESPOSTA_ACAO> GetAllAcoes();
        FORMULARIO_RESPOSTA_ANEXO GetAnexoById(Int32 id);
        FORMULARIO_RESPOSTA_COMENTARIO GetComentarioById(Int32 id);
        FORMULARIO_RESPOSTA_ACAO GetAcaoById(Int32 id);
        Int32 EditAcao(FORMULARIO_RESPOSTA_ACAO item);
        Int32 CreateAcao(FORMULARIO_RESPOSTA_ACAO item);
    }
}
