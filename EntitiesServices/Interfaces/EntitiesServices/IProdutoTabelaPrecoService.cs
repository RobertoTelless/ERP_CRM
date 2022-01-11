using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IProdutoTabelaPrecoService : IServiceBase<PRODUTO_TABELA_PRECO>
    {
        Int32 Create(PRODUTO_TABELA_PRECO item);
        Int32 Edit(PRODUTO_TABELA_PRECO item);
        Int32 Delete(PRODUTO_TABELA_PRECO item);

        PRODUTO_TABELA_PRECO CheckExist(PRODUTO_TABELA_PRECO item, Int32 idAss);
        PRODUTO_TABELA_PRECO GetItemById(Int32 id);
        PRODUTO_TABELA_PRECO GetByProdFilial(Int32 prod, Int32 fili);
    }
}
