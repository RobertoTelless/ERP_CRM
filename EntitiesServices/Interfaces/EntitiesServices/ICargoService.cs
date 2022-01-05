using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ICargoService : IServiceBase<CARGO>
    {
        Int32 Create(CARGO perfil, LOG log);
        Int32 Create(CARGO perfil);
        Int32 Edit(CARGO perfil, LOG log);
        Int32 Edit(CARGO perfil);
        Int32 Delete(CARGO perfil, LOG log);

        CARGO CheckExist(CARGO item, Int32 idAss);
        CARGO GetItemById(Int32 id);
        List<CARGO> GetAllItens(Int32 idAss);
        List<CARGO> GetAllItensAdm(Int32 idAss);
    }
}
