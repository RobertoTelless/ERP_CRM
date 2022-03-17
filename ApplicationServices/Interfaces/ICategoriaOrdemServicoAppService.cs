using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaOrdemServicoAppService : IAppServiceBase<CATEGORIA_ORDEM_SERVICO>
    {
        Int32 ValidateCreate(CATEGORIA_ORDEM_SERVICO perfil, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_ORDEM_SERVICO perfil, CATEGORIA_ORDEM_SERVICO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_ORDEM_SERVICO item, CATEGORIA_ORDEM_SERVICO itemAntes);
        Int32 ValidateDelete(CATEGORIA_ORDEM_SERVICO perfil, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_ORDEM_SERVICO perfil, USUARIO usuario);

        CATEGORIA_ORDEM_SERVICO CheckExist(CATEGORIA_ORDEM_SERVICO item, Int32 idAss);
        CATEGORIA_ORDEM_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItens(Int32 idAss);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItensAdm(Int32 idAss);
    }
}
