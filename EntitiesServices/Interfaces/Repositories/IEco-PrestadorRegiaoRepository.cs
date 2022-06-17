using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorRegiaoRepository : IRepositoryBase<PRESTADOR_REGIAO>
    {
        List<PRESTADOR_REGIAO> GetAllItens();
        PRESTADOR_REGIAO GetItemById(Int32 id);
    }
}
