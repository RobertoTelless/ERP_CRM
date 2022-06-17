using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IEco_PrestadorAjudanteAnotacoesRepository : IRepositoryBase<PRESTADOR_AJUDANTE_ANOTACOES>
    {
        List<PRESTADOR_AJUDANTE_ANOTACOES> GetAllItens();
        PRESTADOR_AJUDANTE_ANOTACOES GetItemById(Int32 id);
    }
}
