using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorMotoristaRepository : IRepositoryBase<PRESTADOR_MOTORISTA>
    {
        List<PRESTADOR_MOTORISTA> GetAllItens();
        PRESTADOR_MOTORISTA GetItemById(Int32 id);
    }
}
