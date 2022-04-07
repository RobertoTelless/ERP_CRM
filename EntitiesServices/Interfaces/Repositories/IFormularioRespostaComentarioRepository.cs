using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IFormularioRespostaComentarioRepository : IRepositoryBase<FORMULARIO_RESPOSTA_COMENTARIO>
    {
        List<FORMULARIO_RESPOSTA_COMENTARIO> GetAllItens();
        FORMULARIO_RESPOSTA_COMENTARIO GetItemById(Int32 id);
    }
}
