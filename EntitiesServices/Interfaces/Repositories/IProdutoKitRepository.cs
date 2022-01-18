using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoKitRepository : IRepositoryBase<PRODUTO_KIT>
    {
        List<PRODUTO_KIT> GetAllItens();
        PRODUTO_KIT GetItemById(Int32 id);
        PRODUTO_KIT GetByProd(Int32 prod);
    }
}
