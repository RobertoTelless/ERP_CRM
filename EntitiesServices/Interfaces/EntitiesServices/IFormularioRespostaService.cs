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

        List<FORMULARIO_RESPOSTA> ExecuteFilter(String nome, String email, String celular, String cidade, Int32? uf);
    }
}
