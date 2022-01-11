using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ISubcategoriaProdutoService : IServiceBase<SUBCATEGORIA_PRODUTO>
    {
        Int32 Create(SUBCATEGORIA_PRODUTO item, LOG log);
        Int32 Create(SUBCATEGORIA_PRODUTO item);
        Int32 Edit(SUBCATEGORIA_PRODUTO item, LOG log);
        Int32 Edit(SUBCATEGORIA_PRODUTO item);
        Int32 Delete(SUBCATEGORIA_PRODUTO item, LOG log);

        SUBCATEGORIA_PRODUTO CheckExist(SUBCATEGORIA_PRODUTO conta, Int32 idAss);
        SUBCATEGORIA_PRODUTO GetItemById(Int32 id);
        List<SUBCATEGORIA_PRODUTO> GetAllItens(Int32 idAss);
        List<SUBCATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss);
        List<CATEGORIA_PRODUTO> GetAllCategorias(Int32 idAss);
    }
}
