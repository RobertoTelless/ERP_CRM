using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaReceberParcelaRepository : IRepositoryBase<CONTA_RECEBER_PARCELA>
    {
        List<CONTA_RECEBER_PARCELA> GetAllItens();
        CONTA_RECEBER_PARCELA GetItemById(Int32 id);

    }
}
