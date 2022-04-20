using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IDocumentoHistoricoRepository : IRepositoryBase<DOCUMENTO_HISTORICO>
    {
        List<DOCUMENTO_HISTORICO> GetAllItens(Int32 idAss);
        DOCUMENTO_HISTORICO GetItemById(Int32 id);
    }
}
