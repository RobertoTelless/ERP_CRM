using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IGrupoCCRepository : IRepositoryBase<GRUPO_CC>
    {
        GRUPO_CC CheckExist(GRUPO_CC item, Int32 idAss);
        GRUPO_CC GetItemById(Int32 id);
        List<GRUPO_CC> GetAllItens(Int32 idAss);
        List<GRUPO_CC> GetAllItensAdm(Int32 idAss);

    }
}
