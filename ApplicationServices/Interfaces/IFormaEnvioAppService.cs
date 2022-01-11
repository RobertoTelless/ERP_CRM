using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IFormaEnvioAppService : IAppServiceBase<FORMA_ENVIO>
    {
        List<FORMA_ENVIO> GetAllItens();
    }
}
