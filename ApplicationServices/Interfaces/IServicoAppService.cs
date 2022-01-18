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

        List<SERVICO> GetAllItens(Int32 idAss);
        List<SERVICO> GetAllItensAdm(Int32 idAss);
        SERVICO GetItemById(Int32 id);
        SERVICO CheckExist(SERVICO conta, Int32 idAss);
        List<CATEGORIA_SERVICO> GetAllTipos(Int32 idAss);
        List<NOMENCLATURA_BRAS_SERVICOS> GetAllNBSE();
        SERVICO_ANEXO GetAnexoById(Int32 id);
        Int32 ExecuteFilter(Int32? catId, String nome, String descricao, String referencia, Int32 idAss, out List<SERVICO> objeto);
    }
}
