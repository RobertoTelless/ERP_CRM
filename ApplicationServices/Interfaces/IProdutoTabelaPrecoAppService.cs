using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IProdutotabelaPrecoAppService : IAppServiceBase<PRODUTO_TABELA_PRECO>
    {
        Int32 ValidateCreate(PRODUTO_TABELA_PRECO item);
        Int32 ValidateEdit(PRODUTO_TABELA_PRECO item, PRODUTO_TABELA_PRECO itemAntes);
        Int32 ValidateDelete(PRODUTO_TABELA_PRECO item);

        PRODUTO_TABELA_PRECO GetItemById(Int32 id);
        PRODUTO_TABELA_PRECO CheckExist(PRODUTO_TABELA_PRECO item, Int32 idAss);
        PRODUTO_TABELA_PRECO GetByProdFilial(Int32 prod, Int32 fili);
    }
}
