using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ICategoriaProdutoAppService : IAppServiceBase<CATEGORIA_PRODUTO>
    {
        Int32 ValidateCreate(CATEGORIA_PRODUTO item, USUARIO usuario);
        Int32 ValidateEdit(CATEGORIA_PRODUTO item, CATEGORIA_PRODUTO itemAntes, USUARIO usuario);
        Int32 ValidateDelete(CATEGORIA_PRODUTO item, USUARIO usuario);
        Int32 ValidateReativar(CATEGORIA_PRODUTO item, USUARIO usuario);

        CATEGORIA_PRODUTO CheckExist(CATEGORIA_PRODUTO conta, Int32 idAss);
        List<CATEGORIA_PRODUTO> GetAllItens(Int32 idAss);
        CATEGORIA_PRODUTO GetItemById(Int32 id);
        List<CATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss);
    }
}
