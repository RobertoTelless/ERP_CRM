using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IContaReceberAnexoRepository : IRepositoryBase<CONTA_RECEBER_ANEXO>
    {
        List<CONTA_RECEBER_ANEXO> GetAllItens();
        CONTA_RECEBER_ANEXO GetItemById(Int32 id);
    }
}
