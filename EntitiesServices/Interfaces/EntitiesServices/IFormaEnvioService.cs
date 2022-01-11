using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface IFormaEnvioService : IServiceBase<FORMA_ENVIO>
    {
        List<FORMA_ENVIO> GetAllItens(Int32 idAss);
    }
}
