using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormularioRespostaAnexoRepository : IRepositoryBase<FORMULARIO_RESPOSTA_ANEXO>
    {
        List<FORMULARIO_RESPOSTA_ANEXO> GetAllItens();
        FORMULARIO_RESPOSTA_ANEXO GetItemById(Int32 id);
    }
}
