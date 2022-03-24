using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface ICRMComercialAcaoRepository : IRepositoryBase<CRM_COMERCIAL_ACAO>
    {
        List<CRM_COMERCIAL_ACAO> GetAllItens(Int32 idAss);
        CRM_COMERCIAL_ACAO GetItemById(Int32 id);
    }
}
