using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface INivelSegurancaRepository : IRepositoryBase<NIVEL_SEGURANCA>
    {
        List<NIVEL_SEGURANCA> GetAllItens(Int32 idAss);
        NIVEL_SEGURANCA GetItemById(Int32 id);
    }
}
