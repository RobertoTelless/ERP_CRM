using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComercialContatoRepository : IRepositoryBase<CRM_COMERCIAL_CONTATO>
    {
        List<CRM_COMERCIAL_CONTATO> GetAllItens();
        CRM_COMERCIAL_CONTATO GetItemById(Int32 id);
    }
}
