using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoMetadadoRepository : IRepositoryBase<TIPO_METADADO>
    {
        List<TIPO_METADADO> GetAllItens(Int32 idAss);
        TIPO_METADADO GetItemById(Int32 id);
    }
}
