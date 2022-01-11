using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormaFreteRepository : IRepositoryBase<FORMA_FRETE>
    {
        List<FORMA_FRETE> GetAllItens(Int32 idAss);
    }
}
