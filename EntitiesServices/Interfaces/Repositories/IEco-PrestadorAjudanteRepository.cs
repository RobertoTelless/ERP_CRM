using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorAjudanteRepository : IRepositoryBase<PRESTADOR_AJUDANTE>
    {
        List<PRESTADOR_AJUDANTE> GetAllItens();
        PRESTADOR_AJUDANTE GetItemById(Int32 id);
    }
}
