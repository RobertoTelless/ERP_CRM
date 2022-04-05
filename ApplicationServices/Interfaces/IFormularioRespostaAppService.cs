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
        Int32 ValidateCreate(FORMULARIO_RESPOSTA perfil);
        Int32 ValidateEdit(FORMULARIO_RESPOSTA perfil, FORMULARIO_RESPOSTA perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(FORMULARIO_RESPOSTA item, FORMULARIO_RESPOSTA itemAntes);
        Int32 ValidateDelete(FORMULARIO_RESPOSTA perfil, USUARIO usuario);
        Int32 ValidateReativar(FORMULARIO_RESPOSTA perfil, USUARIO usuario);

        List<FORMULARIO_RESPOSTA> GetAllItens();
        FORMULARIO_RESPOSTA GetItemById(Int32 id);
        Int32 ExecuteFilter(String nome, String email, String celular, String cidade, Int32? uf, out List<FORMULARIO_RESPOSTA> objeto);

        List<UF> GetAllUF();
        UF GetUFbySigla(String sigla);
    }
}
