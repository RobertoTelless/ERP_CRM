using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoOrigemRepository : IRepositoryBase<PRODUTO_ORIGEM>
    {
        List<PRODUTO_ORIGEM> GetAllItens(Int32 idAss);
        PRODUTO_ORIGEM GetItemById(Int32 id);
        List<PRODUTO_ORIGEM> GetAllItensAdm(Int32 idAss);
    }
}
