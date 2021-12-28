using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ITipoAcaoRepository : IRepositoryBase<TIPO_ACAO>
    {
        List<TIPO_ACAO> GetAllItens();
        TIPO_ACAO GetItemById(Int32 id);
        List<TIPO_ACAO> GetAllItensAdm();
    }
}
