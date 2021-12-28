using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMOrigemRepository : IRepositoryBase<CRM_ORIGEM>
    {
        List<CRM_ORIGEM> GetAllItens();
        CRM_ORIGEM GetItemById(Int32 id);
    }
}
