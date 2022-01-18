using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IServicoAppService : IAppServiceBase<SERVICO>
    {
        Int32 ValidateCreate(SERVICO perfil, USUARIO usuario);
        Int32 ValidateEdit(SERVICO perfil, SERVICO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(SERVICO item, SERVICO itemAntes);
        Int32 ValidateDelete(SERVICO perfil, USUARIO usuario);
        Int32 ValidateReativar(SERVICO perfil, USUARIO usuario);

        List<SERVICO> GetAllItens();
        List<SERVICO> GetAllItensAdm();
        SERVICO GetItemById(Int32 id);
        SERVICO CheckExist(SERVICO conta);
        List<CATEGORIA_SERVICO> GetAllTipos();
        List<NOMENCLATURA_BRAS_SERVICOS> GetAllNBSE();
        SERVICO_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? catId, String nome, String descricao, String referencia, out List<SERVICO> objeto);
    }
}
