using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaServicoAppService : IAppServiceBase<CATEGORIA_SERVICO>
    {
        Int32 ValidateCreate(CATEGORIA_SERVICO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_SERVICO item, CATEGORIA_SERVICO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_SERVICO item, CATEGORIA_SERVICO itemAntes);
        Int32 ValidateDelete(CATEGORIA_SERVICO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_SERVICO item, USUARIO usuario);
        List<CATEGORIA_SERVICO> GetAllItens();
        List<CATEGORIA_SERVICO> GetAllItensAdm();
        CATEGORIA_SERVICO GetItemById(Int32 id);
    }
}
