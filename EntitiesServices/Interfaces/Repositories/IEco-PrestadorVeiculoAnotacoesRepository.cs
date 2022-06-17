using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorVeiculoAnotacoesRepository : IRepositoryBase<PRESTADOR_VEICULO_ANOTACOES>
    {
        List<PRESTADOR_VEICULO_ANOTACOES> GetAllItens();
        PRESTADOR_VEICULO_ANOTACOES GetItemById(Int32 id);
    }
}
