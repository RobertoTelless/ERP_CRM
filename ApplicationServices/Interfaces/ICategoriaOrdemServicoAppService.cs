using EntitiesServices.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaOrdemServicoAppService : IAppServiceBase<CATEGORIA_ORDEM_SERVICO>
    {
        Int32 ValidateCreate(CATEGORIA_ORDEM_SERVICO perfil, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_ORDEM_SERVICO perfil, CATEGORIA_ORDEM_SERVICO perfilAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_ORDEM_SERVICO item, CATEGORIA_ORDEM_SERVICO itemAntes);
        Int32 ValidateDelete(CATEGORIA_ORDEM_SERVICO perfil, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_ORDEM_SERVICO perfil, USUARIO usuario);

        CATEGORIA_ORDEM_SERVICO CheckExist(CATEGORIA_ORDEM_SERVICO item);
        CATEGORIA_ORDEM_SERVICO GetItemById(Int32 id);
        List<CATEGORIA_ORDEM_SERVICO> GetAllItens();
        List<CATEGORIA_ORDEM_SERVICO> GetAllItensAdm();
    }
}
