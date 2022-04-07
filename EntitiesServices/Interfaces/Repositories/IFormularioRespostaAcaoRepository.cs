using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormularioRespostaAcaoRepository : IRepositoryBase<FORMULARIO_RESPOSTA_ACAO>
    {
        List<FORMULARIO_RESPOSTA_ACAO> GetAllItens();
        FORMULARIO_RESPOSTA_ACAO GetItemById(Int32 id);
    }
}
