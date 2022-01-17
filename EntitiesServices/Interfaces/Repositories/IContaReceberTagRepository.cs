using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaReceberTagRepository : IRepositoryBase<CONTA_RECEBER_TAG>
    {
        List<CONTA_RECEBER_TAG> GetAllItens();
        CONTA_RECEBER_TAG GetItemById(Int32 id);

    }
}
