using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaReceberRateioRepository : IRepositoryBase<CONTA_RECEBER_RATEIO>
    {
        CONTA_RECEBER_RATEIO CheckExist(CONTA_RECEBER_RATEIO item);
        List<CONTA_RECEBER_RATEIO> GetAllItens();
        CONTA_RECEBER_RATEIO GetItemById(Int32 id);
    }
}
