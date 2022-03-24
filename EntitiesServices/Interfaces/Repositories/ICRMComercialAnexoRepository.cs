using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComercialAnexoRepository : IRepositoryBase<CRM_COMERCIAL_ANEXO>
    {
        List<CRM_COMERCIAL_ANEXO> GetAllItens();
        CRM_COMERCIAL_ANEXO GetItemById(Int32 id);
    }
}
