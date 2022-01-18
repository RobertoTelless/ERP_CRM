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

        CATEGORIA_SERVICO CheckExist(CATEGORIA_SERVICO conta, Int32 idAss);
        List<CATEGORIA_SERVICO> GetAllItens(Int32 idAss);
        List<CATEGORIA_SERVICO> GetAllItensAdm(Int32 idAss);
        CATEGORIA_SERVICO GetItemById(Int32 id);
    }
}
