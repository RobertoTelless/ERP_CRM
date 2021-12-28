using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMotivoCancelamentoRepository : IRepositoryBase<MOTIVO_CANCELAMENTO>
    {
        List<MOTIVO_CANCELAMENTO> GetAllItens();
        MOTIVO_CANCELAMENTO GetItemById(Int32 id);
        List<MOTIVO_CANCELAMENTO> GetAllItensAdm();
    }
}
