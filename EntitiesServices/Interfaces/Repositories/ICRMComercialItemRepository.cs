using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComercialItemRepository : IRepositoryBase<CRM_COMERCIAL_ITEM>
    {
        List<CRM_COMERCIAL_ITEM> GetAllItens();
        CRM_COMERCIAL_ITEM GetItemById(Int32 id);
        CRM_COMERCIAL_ITEM GetItemByProduto(Int32 id);
    
    }
}
