using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IClienteTagRepository : IRepositoryBase<CLIENTE_TAG>
    {
        List<CLIENTE_TAG> GetAllItens();
        CLIENTE_TAG GetItemById(Int32 id);
    }
}
