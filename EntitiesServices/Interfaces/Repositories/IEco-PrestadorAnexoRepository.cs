using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorAnexoRepository : IRepositoryBase<PRESTADOR_ANEXO>
    {
        List<PRESTADOR_ANEXO> GetAllItens();
        PRESTADOR_ANEXO GetItemById(Int32 id);
    }
}
