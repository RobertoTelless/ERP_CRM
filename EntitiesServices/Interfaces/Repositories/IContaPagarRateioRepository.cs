using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaPagarRateioRepository : IRepositoryBase<CONTA_PAGAR_RATEIO>
    {
        CONTA_PAGAR_RATEIO CheckExist(CONTA_PAGAR_RATEIO item);
        List<CONTA_PAGAR_RATEIO> GetAllItens();
        CONTA_PAGAR_RATEIO GetItemById(Int32 id);
    }
}
