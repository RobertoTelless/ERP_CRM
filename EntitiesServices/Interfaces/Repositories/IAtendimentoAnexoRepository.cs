using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ModelServices.Interfaces.Repositories
{
    public interface IAtendimentoAnexoRepository : IRepositoryBase<ATENDIMENTO_ANEXO>
    {
        List<ATENDIMENTO_ANEXO> GetAllItens();
        ATENDIMENTO_ANEXO GetItemById(Int32 id);
    
    }
}
