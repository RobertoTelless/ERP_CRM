using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IProdutoEstoqueFilialService : IServiceBase<PRODUTO_ESTOQUE_FILIAL>
    {
        Int32 Create(PRODUTO_ESTOQUE_FILIAL item);
        Int32 Edit(PRODUTO_ESTOQUE_FILIAL item);
        Int32 Delete(PRODUTO_ESTOQUE_FILIAL item);

        List<PRODUTO_ESTOQUE_FILIAL> GetAllItens(Int32 idAss);
        PRODUTO_ESTOQUE_FILIAL GetByProdFilial(Int32 prod, Int32 fili);
        List<PRODUTO_ESTOQUE_FILIAL> GetByProd(Int32 id);
        PRODUTO_ESTOQUE_FILIAL CheckExist(PRODUTO_ESTOQUE_FILIAL item, Int32 idAss);
        PRODUTO_ESTOQUE_FILIAL GetItemById(Int32 id);
        PRODUTO_ESTOQUE_FILIAL GetItemById(PRODUTO item);
    }
}
