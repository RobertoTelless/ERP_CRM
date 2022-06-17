using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorVeiculoFuncaoRepository : IRepositoryBase<PRESTADOR_VEICULO_FUNCAO>
    {
        List<PRESTADOR_VEICULO_FUNCAO> GetAllItens();
        PRESTADOR_VEICULO_FUNCAO GetItemById(Int32 id);
    }
}
