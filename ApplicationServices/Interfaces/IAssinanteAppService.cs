using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IAssinanteAppService : IAppServiceBase<ASSINANTE>
    {
        Int32 ValidateCreate(ASSINANTE perfil, USUARIO usuario);
        Int32 ValidateEdit(ASSINANTE perfil, ASSINANTE perfilAntes, USUARIO usuario);
        Int32 ValidateDelete(ASSINANTE perfil, USUARIO usuario);
        Int32 ValidateReativar(ASSINANTE perfil, USUARIO usuario);

        ASSINANTE CheckExist(ASSINANTE conta);
        List<ASSINANTE> GetAllItens();
        List<ASSINANTE> GetAllItensAdm();
        ASSINANTE GetItemById(Int32 id);
        Int32 ExecuteFilter(Int32 tipo, String nome, String cpf, String cnpj, Int32 status, out List<ASSINANTE> objeto);

        List<TIPO_PESSOA> GetAllTiposPessoa();
        List<PLANO> GetAllPlanos();
        List<UF> GetAllUF();
        ASSINANTE_ANEXO GetAnexoById(Int32 id);
        UF GetUFBySigla(String sigla);
        ASSINANTE_ANOTACAO GetAnotacaoById(Int32 id);

        ASSINANTE_PAGAMENTO GetPagtoById(Int32 id);
        Int32 ValidateEditPagto(ASSINANTE_PAGAMENTO item);
        Int32 ValidateCreatePagto(ASSINANTE_PAGAMENTO item);

    }
}
