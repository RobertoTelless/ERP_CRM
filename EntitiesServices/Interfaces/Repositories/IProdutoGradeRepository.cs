using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IProdutoGradeRepository : IRepositoryBase<PRODUTO_GRADE>
    {
        List<PRODUTO_GRADE> GetAllItens();
        PRODUTO_GRADE GetItemById(Int32 id);
    }
}
