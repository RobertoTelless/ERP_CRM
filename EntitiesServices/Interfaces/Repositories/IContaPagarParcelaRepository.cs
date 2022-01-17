using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaPagarParcelaRepository : IRepositoryBase<CONTA_PAGAR_PARCELA>
    {
        List<CONTA_PAGAR_PARCELA> GetAllItens(Int32 idAss);
        CONTA_PAGAR_PARCELA GetItemById(Int32 id);
    }

}
