using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INomenclaturaRepository : IRepositoryBase<NOMENCLATURA_BRAS_SERVICOS>
    {
        List<NOMENCLATURA_BRAS_SERVICOS> GetAllItens();
        NOMENCLATURA_BRAS_SERVICOS GetItemById(Int32 id);
    }
}
