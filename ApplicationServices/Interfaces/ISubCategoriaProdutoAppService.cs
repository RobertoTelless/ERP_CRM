using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ISubcategoriaProdutoAppService : IAppServiceBase<SUBCATEGORIA_PRODUTO>
    {
        Int32 ValidateCreate(SUBCATEGORIA_PRODUTO item, USUARIO usuario);
        Int32 ValidateEdit(SUBCATEGORIA_PRODUTO item, SUBCATEGORIA_PRODUTO itemAntes, USUARIO usuario);
        Int32 ValidateEdit(SUBCATEGORIA_PRODUTO item, SUBCATEGORIA_PRODUTO itemAntes);
        Int32 ValidateDelete(SUBCATEGORIA_PRODUTO item, USUARIO usuario);
        Int32 ValidateReativar(SUBCATEGORIA_PRODUTO item, USUARIO usuario);

        SUBCATEGORIA_PRODUTO CheckExist(SUBCATEGORIA_PRODUTO conta, Int32 idAss);
        List<SUBCATEGORIA_PRODUTO> GetAllItens(Int32 idAss);
        List<SUBCATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss);
        SUBCATEGORIA_PRODUTO GetItemById(Int32 id);
        List<CATEGORIA_PRODUTO> GetAllCategorias(Int32 idAss);
    }
}
