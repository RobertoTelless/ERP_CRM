using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComercialComentarioRepository : IRepositoryBase<CRM_COMERCIAL_COMENTARIO_NOVA>
    {
        List<CRM_COMERCIAL_COMENTARIO_NOVA> GetAllItens();
        CRM_COMERCIAL_COMENTARIO_NOVA GetItemById(Int32 id);
    }
}
