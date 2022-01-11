using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoTabelaPrecoRepository : IRepositoryBase<PRODUTO_TABELA_PRECO>
    {
        PRODUTO_TABELA_PRECO CheckExist(PRODUTO_TABELA_PRECO item, Int32 idAss);
        PRODUTO_TABELA_PRECO GetItemById(Int32 id);
        PRODUTO_TABELA_PRECO GetByProdFilial(Int32 prod, Int32 fili);
    }
}
