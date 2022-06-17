using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorEnderecoRepository : IRepositoryBase<PRESTADOR_ENDERECO>
    {
        List<PRESTADOR_ENDERECO> GetAllItens();
        PRESTADOR_ENDERECO GetItemById(Int32 id);
    }
}
