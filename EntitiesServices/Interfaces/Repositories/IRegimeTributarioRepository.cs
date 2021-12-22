using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IRegimeTributarioRepository : IRepositoryBase<REGIME_TRIBUTARIO>
    {
        List<REGIME_TRIBUTARIO> GetAllItens(Int32 idAss);
        REGIME_TRIBUTARIO GetItemById(Int32 id);
        List<REGIME_TRIBUTARIO> GetAllItensAdm(Int32 idAss);
    }
}
