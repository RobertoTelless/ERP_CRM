using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICategoriaProdutoService : IServiceBase<CATEGORIA_PRODUTO>
    {
        Int32 Create(CATEGORIA_PRODUTO perfil, LOG log);
        Int32 Create(CATEGORIA_PRODUTO perfil);
        Int32 Edit(CATEGORIA_PRODUTO perfil, LOG log);
        Int32 Edit(CATEGORIA_PRODUTO perfil);
        Int32 Delete(CATEGORIA_PRODUTO perfil, LOG log);

        CATEGORIA_PRODUTO CheckExist(CATEGORIA_PRODUTO conta, Int32 idAss);
        CATEGORIA_PRODUTO GetItemById(Int32 id);
        List<CATEGORIA_PRODUTO> GetAllItens(Int32 idAss);
        List<CATEGORIA_PRODUTO> GetAllItensAdm(Int32 idAss);
    }
}
