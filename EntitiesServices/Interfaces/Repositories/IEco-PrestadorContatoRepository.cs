using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorContatoRepository : IRepositoryBase<PRESTADOR_CONTATO>
    {
        List<PRESTADOR_CONTATO> GetAllItens();
        PRESTADOR_CONTATO GetItemById(Int32 id);
    }
}
