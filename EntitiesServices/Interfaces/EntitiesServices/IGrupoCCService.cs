using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IGrupoCCService : IServiceBase<GRUPO_CC>
    {
        Int32 Create(GRUPO_CC item, LOG log);
        Int32 Create(GRUPO_CC item);
        Int32 Edit(GRUPO_CC item, LOG log);
        Int32 Edit(GRUPO_CC item);
        Int32 Delete(GRUPO_CC item, LOG log);

        GRUPO_CC GetItemById(Int32 id);
        GRUPO_CC CheckExist(GRUPO_CC item, Int32 idAss);
        List<GRUPO_CC> GetAllItens(Int32 idAss);
        List<GRUPO_CC> GetAllItensAdm(Int32 idAss);
    }
}
