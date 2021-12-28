using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IMotivoEncerramentoRepository : IRepositoryBase<MOTIVO_ENCERRAMENTO>
    {
        List<MOTIVO_ENCERRAMENTO> GetAllItens();
        MOTIVO_ENCERRAMENTO GetItemById(Int32 id);
        List<MOTIVO_ENCERRAMENTO> GetAllItensAdm();
    }
}
