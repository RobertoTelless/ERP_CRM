using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorBancoRepository : IRepositoryBase<PRESTADOR_BANCO>
    {
        List<PRESTADOR_BANCO> GetAllItens();
        PRESTADOR_BANCO GetItemById(Int32 id);
    }
}
