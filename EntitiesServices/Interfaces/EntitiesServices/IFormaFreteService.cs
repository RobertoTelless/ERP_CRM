using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFormaFreteService : IServiceBase<FORMA_FRETE>
    {
        List<FORMA_FRETE> GetAllItens(Int32 idAss);
    }
}
