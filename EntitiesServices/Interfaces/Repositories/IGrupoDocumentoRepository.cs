using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoDocumentoRepository : IRepositoryBase<GRUPO_DOCUMENTO>
    {
        GRUPO_DOCUMENTO CheckExist(GRUPO_DOCUMENTO item, Int32 idAss);
        List<GRUPO_DOCUMENTO> GetAllItens(Int32 idAss);
        GRUPO_DOCUMENTO GetItemById(Int32 id);
    }
}
